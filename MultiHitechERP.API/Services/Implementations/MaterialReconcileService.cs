using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MultiHitechERP.API.DTOs.Request;
using MultiHitechERP.API.DTOs.Response;
using MultiHitechERP.API.Repositories.Interfaces;
using MultiHitechERP.API.Services.Interfaces;

namespace MultiHitechERP.API.Services.Implementations
{
    public class MaterialReconcileService : IMaterialReconcileService
    {
        private readonly IMaterialPieceRepository _pieceRepo;
        private readonly IMaterialReconcileRepository _reconcileRepo;
        private readonly IMaterialRepository _materialRepo;

        public MaterialReconcileService(IMaterialPieceRepository pieceRepo, IMaterialReconcileRepository reconcileRepo, IMaterialRepository materialRepo)
        {
            _pieceRepo = pieceRepo;
            _reconcileRepo = reconcileRepo;
            _materialRepo = materialRepo;
        }

        public async Task<ApiResponse<MaterialPiecesByLengthResponse>> GetPiecesByLengthAsync(int materialId)
        {
            try
            {
                var pieces = (await _pieceRepo.GetByMaterialIdAsync(materialId))
                    .Where(p => p.Status == "Available")
                    .ToList();
                var material = await _materialRepo.GetByIdAsync(materialId);

                var resp = new MaterialPiecesByLengthResponse
                {
                    MaterialId = materialId,
                    MaterialCode = pieces.FirstOrDefault()?.MaterialCode ?? material?.MaterialCode,
                    MaterialName = pieces.FirstOrDefault()?.MaterialName ?? material?.MaterialName,
                    MinUsableLengthMM = material?.MinLengthMM ?? 300,
                    TotalPieces = pieces.Count,
                    TotalLengthMM = pieces.Sum(p => p.CurrentLengthMM),
                    TotalWeightKG = pieces.Sum(p => p.CurrentWeightKG),
                    Groups = pieces
                        .GroupBy(p => p.CurrentLengthMM)
                        .OrderByDescending(g => g.Key)
                        .Select(g => new LengthGroupResponse
                        {
                            LengthMM = g.Key,
                            Count = g.Count(),
                            TotalWeightKG = g.Sum(p => p.CurrentWeightKG),
                            Pieces = g.Select(p => new PieceInfoResponse
                            {
                                Id = p.Id,
                                PieceNo = p.PieceNo,
                                LengthMM = p.CurrentLengthMM,
                                WeightKG = p.CurrentWeightKG,
                            }).ToList()
                        }).ToList()
                };
                return ApiResponse<MaterialPiecesByLengthResponse>.SuccessResponse(resp);
            }
            catch (Exception ex)
            {
                return ApiResponse<MaterialPiecesByLengthResponse>.ErrorResponse($"Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<ReconcileLogResponse>>> GetHistoryAsync(int? materialId)
        {
            try
            {
                return ApiResponse<IEnumerable<ReconcileLogResponse>>.SuccessResponse(await _reconcileRepo.GetHistoryAsync(materialId));
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<ReconcileLogResponse>>.ErrorResponse($"Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<ReconcileResultResponse>> ReconcileAsync(ReconcilePiecesRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Remarks))
                    return ApiResponse<ReconcileResultResponse>.ErrorResponse("Reason is required for reconciliation");

                var available = (await _pieceRepo.GetByMaterialIdAsync(request.MaterialId))
                    .Where(p => p.Status == "Available")
                    .ToList();
                if (!available.Any())
                    return ApiResponse<ReconcileResultResponse>.ErrorResponse("No available pieces for this material");

                var code = available.First().MaterialCode;
                var name = available.First().MaterialName;
                var by = request.PerformedBy ?? "Admin";
                var remarks = request.Remarks;

                // Scrap threshold from the material master — a reconciled piece shorter than this becomes scrap
                var material = await _materialRepo.GetByIdAsync(request.MaterialId);
                var minLen = material?.MinLengthMM ?? 300;

                int barsRemoved = 0, lengthsAdjusted = 0, movedToScrap = 0;
                decimal totalRemoved = 0;
                var usedPieceIds = new HashSet<int>();

                // 1) Whole-bar removals (count correction) — delete N available bars of the given length
                foreach (var rem in request.Removals.Where(r => r.Count > 0))
                {
                    var candidates = available
                        .Where(p => p.CurrentLengthMM == rem.LengthMM && !usedPieceIds.Contains(p.Id))
                        .Take(rem.Count)
                        .ToList();

                    foreach (var p in candidates)
                    {
                        await _reconcileRepo.InsertLogAsync(request.MaterialId, code, name, p.Id, p.PieceNo,
                            "RemoveBar", p.CurrentLengthMM, 0, p.CurrentLengthMM, p.CurrentWeightKG,
                            "Correction", remarks, by);
                        await _pieceRepo.DeleteAsync(p.Id);
                        usedPieceIds.Add(p.Id);
                        barsRemoved++;
                        totalRemoved += p.CurrentLengthMM;
                    }
                }

                // 2) Length reductions (reconcile consumption) — a bar is physically shorter
                foreach (var ch in request.LengthChanges)
                {
                    var piece = available.FirstOrDefault(p => p.Id == ch.PieceId && !usedPieceIds.Contains(p.Id));
                    if (piece == null) continue;
                    if (ch.NewLengthMM < 0 || ch.NewLengthMM >= piece.CurrentLengthMM) continue; // only reductions

                    var removed = piece.CurrentLengthMM - ch.NewLengthMM;
                    var weightPerMM = piece.OriginalLengthMM > 0
                        ? piece.OriginalWeightKG / piece.OriginalLengthMM
                        : (piece.CurrentLengthMM > 0 ? piece.CurrentWeightKG / piece.CurrentLengthMM : 0);
                    var newWeight = ch.NewLengthMM * weightPerMM;

                    await _pieceRepo.AdjustLengthAsync(piece.Id, ch.NewLengthMM, newWeight, by);
                    usedPieceIds.Add(piece.Id);
                    lengthsAdjusted++;
                    totalRemoved += removed;

                    // If the reconciled piece is now shorter than the material's minimum usable length → scrap it
                    var scrapped = ch.NewLengthMM < minLen;
                    if (scrapped)
                    {
                        await _pieceRepo.MarkAsWastageAsync(piece.Id,
                            $"Reconcile: length {ch.NewLengthMM:F0}mm below minimum usable {minLen}mm", null);
                        movedToScrap++;
                    }

                    await _reconcileRepo.InsertLogAsync(request.MaterialId, code, name, piece.Id, piece.PieceNo,
                        "ReduceLength", piece.CurrentLengthMM, ch.NewLengthMM, removed, piece.CurrentWeightKG - newWeight,
                        scrapped ? "Reconcile-Scrap" : "Reconcile",
                        scrapped ? $"Moved to scrap (< {minLen}mm min usable). {remarks}" : remarks, by);
                }

                // 3) Recompute the aggregate raw-material stock from the remaining available pieces
                var (_, newTotalLength, newTotalWeight) = await _pieceRepo.GetStockSummaryByMaterialIdAsync(request.MaterialId);
                var uom = available.First() is { } sp ? "mm" : "mm";
                await _reconcileRepo.SetMaterialAggregateAsync(request.MaterialId, newTotalLength, uom);

                return ApiResponse<ReconcileResultResponse>.SuccessResponse(new ReconcileResultResponse
                {
                    BarsRemoved = barsRemoved,
                    LengthsAdjusted = lengthsAdjusted,
                    MovedToScrap = movedToScrap,
                    TotalLengthRemovedMM = totalRemoved,
                    NewTotalLengthMM = newTotalLength,
                    NewTotalWeightKG = newTotalWeight,
                }, $"Reconciled: {barsRemoved} bar(s) removed, {lengthsAdjusted} length(s) reduced" +
                   (movedToScrap > 0 ? $", {movedToScrap} moved to scrap (below min usable)." : "."));
            }
            catch (Exception ex)
            {
                return ApiResponse<ReconcileResultResponse>.ErrorResponse($"Failed to reconcile: {ex.Message}");
            }
        }
    }
}

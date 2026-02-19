using MultiHitechERP.API.DTOs.Request;
using MultiHitechERP.API.DTOs.Response;
using MultiHitechERP.API.Repositories.Interfaces;
using MultiHitechERP.API.Services.Interfaces;

namespace MultiHitechERP.API.Services.Implementations
{
    public class StoresIssueWindowService : IStoresIssueWindowService
    {
        private readonly IMaterialRequisitionRepository _reqRepo;
        private readonly IMaterialPieceRepository _pieceRepo;
        private readonly IMaterialRequisitionService _reqService;
        private readonly IIssueWindowDraftRepository _draftRepo;
        private const int MinUsableLengthMM = 300;

        public StoresIssueWindowService(
            IMaterialRequisitionRepository reqRepo,
            IMaterialPieceRepository pieceRepo,
            IMaterialRequisitionService reqService,
            IIssueWindowDraftRepository draftRepo)
        {
            _reqRepo = reqRepo;
            _pieceRepo = pieceRepo;
            _reqService = reqService;
            _draftRepo = draftRepo;
        }

        // ── Get approved requisitions ─────────────────────────────────────────────

        public async Task<IEnumerable<IssueWindowRequisitionResponse>> GetApprovedRequisitionsAsync()
        {
            var reqs = await _reqRepo.GetByStatusAsync("Approved");
            var result = new List<IssueWindowRequisitionResponse>();

            foreach (var r in reqs)
            {
                var items = await _reqRepo.GetRequisitionItemsAsync(r.Id);
                var materialItemCount = items.Count(i => i.MaterialId.HasValue && i.LengthRequiredMM.HasValue);
                if (materialItemCount == 0) continue;

                result.Add(new IssueWindowRequisitionResponse
                {
                    Id = r.Id,
                    RequisitionNo = r.RequisitionNo,
                    OrderNo = r.OrderNo,
                    JobCardNo = r.JobCardNo,
                    CustomerName = r.CustomerName,
                    Priority = r.Priority,
                    DueDate = r.DueDate,
                    CreatedAt = r.CreatedAt,
                    ItemCount = materialItemCount
                });
            }

            return result.OrderByDescending(r => r.Priority == "High").ThenBy(r => r.DueDate);
        }

        // ── Get material groups (qty-expanded cut rows) ───────────────────────────

        public async Task<IEnumerable<MaterialGroupResponse>> GetMaterialGroupsAsync(IEnumerable<int> requisitionIds)
        {
            var idList = requisitionIds.ToList();
            if (!idList.Any()) return Enumerable.Empty<MaterialGroupResponse>();

            var allItems = (await _reqRepo.GetItemsByRequisitionIdsAsync(idList)).ToList();

            // Lookup requisition info for display
            var reqInfo = new Dictionary<int, (string No, string? JobCardNo)>();
            foreach (var rid in idList)
            {
                var req = await _reqRepo.GetByIdAsync(rid);
                if (req != null)
                    reqInfo[rid] = (req.RequisitionNo, req.JobCardNo);
            }

            // Group by material + grade + diameter
            var groupKey = (int? matId, string? grade, decimal? diam) =>
                $"{matId ?? 0}_{grade ?? ""}_{diam ?? 0}";

            var groups = allItems
                .Where(i => i.MaterialId.HasValue && i.LengthRequiredMM.HasValue && i.LengthRequiredMM > 0)
                .GroupBy(i => groupKey(i.MaterialId, i.MaterialGrade, i.DiameterMM))
                .ToList();

            var result = new List<MaterialGroupResponse>();

            foreach (var group in groups)
            {
                var first = group.First();
                var cuts = new List<MaterialGroupCutItem>();

                foreach (var item in group)
                {
                    var reqNo = reqInfo.TryGetValue(item.RequisitionId, out var ri) ? ri.No : "";
                    var jobCardNo = item.JobCardNo ?? (reqInfo.TryGetValue(item.RequisitionId, out var ri2) ? ri2.JobCardNo : null);
                    var qty = item.NumberOfPieces ?? 1;

                    // Expand each unit into a separate cut row
                    for (int i = 0; i < qty; i++)
                    {
                        cuts.Add(new MaterialGroupCutItem
                        {
                            RequisitionItemId = item.Id,
                            RequisitionId = item.RequisitionId,
                            CutIndex = i,
                            CutLengthMM = item.LengthRequiredMM!.Value,
                            PartName = jobCardNo,
                            JobCardNo = jobCardNo,
                            RequisitionNo = reqNo,
                            MaterialId = item.MaterialId
                        });
                    }
                }

                var totalNeeded = cuts.Sum(c => c.CutLengthMM);

                result.Add(new MaterialGroupResponse
                {
                    MaterialId = first.MaterialId,
                    MaterialName = first.MaterialName ?? "Unknown Material",
                    MaterialCode = first.MaterialCode,
                    Grade = first.MaterialGrade,
                    DiameterMM = first.DiameterMM,
                    TotalLengthNeededMM = totalNeeded,
                    Cuts = cuts.OrderBy(c => c.RequisitionNo).ThenBy(c => c.JobCardNo).ThenBy(c => c.CutIndex).ToList()
                });
            }

            return result.OrderBy(g => g.MaterialName).ThenBy(g => g.Grade).ThenBy(g => g.DiameterMM);
        }

        // ── Available pieces for a material (for bar-select dropdown) ─────────────

        public async Task<IEnumerable<IssueWindowAvailablePieceResponse>> GetAvailablePiecesAsync(
            int materialId, string? grade, decimal? diameterMM)
        {
            var pieces = await _pieceRepo.GetAvailablePiecesByMaterialAsync(materialId, grade, diameterMM);

            return pieces.Select(p => new IssueWindowAvailablePieceResponse
            {
                Id = p.Id,
                PieceNo = p.PieceNo,
                CurrentLengthMM = p.CurrentLengthMM,
                StorageLocation = p.StorageLocation ?? p.WarehouseName,
                GRNNo = p.GRNNo
            }).OrderBy(p => p.CurrentLengthMM);
        }

        // ── Save draft ────────────────────────────────────────────────────────────

        public async Task<IssueWindowDraftDetailResponse> SaveDraftAsync(SaveDraftRequest request)
        {
            var draftId = await _draftRepo.SaveDraftAsync(request);
            var draft = await _draftRepo.GetDraftByIdAsync(draftId);
            return draft!;
        }

        // ── List drafts (Cutting Planning page — Draft status only) ──────────────

        public async Task<IEnumerable<IssueWindowDraftSummaryResponse>> GetDraftsAsync()
        {
            return await _draftRepo.GetDraftsAsync();
        }

        // ── List finalized drafts (Issue List page) ───────────────────────────────

        public async Task<IEnumerable<IssueWindowDraftSummaryResponse>> GetFinalizedDraftsAsync()
        {
            return await _draftRepo.GetFinalizedDraftsAsync();
        }

        // ── Get draft detail ──────────────────────────────────────────────────────

        public async Task<IssueWindowDraftDetailResponse?> GetDraftByIdAsync(int id)
        {
            return await _draftRepo.GetDraftByIdAsync(id);
        }

        // ── Finalize a draft (lock it, move to Issue List) ────────────────────────

        public async Task<bool> FinalizeDraftAsync(int id)
        {
            return await _draftRepo.FinalizeDraftAsync(id);
        }

        // ── Issue a finalized draft ───────────────────────────────────────────────

        public async Task<IEnumerable<IssueWindowIssueResultResponse>> IssueDraftAsync(int draftId, IssueDraftRequest request)
        {
            var draft = await _draftRepo.GetDraftByIdAsync(draftId);
            if (draft == null)
                throw new Exception($"Draft {draftId} not found");

            if (draft.Status == "Issued")
                throw new Exception("Draft has already been issued");

            if (draft.Status != "Finalized")
                throw new Exception("Draft must be Finalized before issuing. Please finalize it first.");

            // Build a map: requisitionItemId → (pieceIds list, cutLengths list) from all cuts in draft
            var itemPieceMap = new Dictionary<int, (List<int> PieceIds, List<decimal> CutLengths)>();

            foreach (var bar in draft.BarAssignments)
            {
                if (!bar.PieceId.HasValue) continue;

                foreach (var cut in bar.Cuts)
                {
                    if (!itemPieceMap.ContainsKey(cut.RequisitionItemId))
                        itemPieceMap[cut.RequisitionItemId] = (new List<int>(), new List<decimal>());

                    itemPieceMap[cut.RequisitionItemId].PieceIds.Add(bar.PieceId.Value);
                    itemPieceMap[cut.RequisitionItemId].CutLengths.Add(cut.CutLengthMM);
                }
            }

            // Update each requisition item's SelectedPieceIds + SelectedPieceQuantities
            foreach (var (itemId, (pieceIds, cutLengths)) in itemPieceMap)
            {
                var pieceIdsStr = string.Join(",", pieceIds);
                var cutLengthsStr = string.Join(",", cutLengths.Select(l => l.ToString("F2")));
                await _reqRepo.UpdateItemPiecesAndQuantitiesAsync(itemId, pieceIdsStr, cutLengthsStr);
            }

            // Get requisition IDs from draft
            var requisitionIds = draft.RequisitionIds
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(s => int.TryParse(s.Trim(), out var id) ? id : 0)
                .Where(id => id > 0)
                .ToList();

            // Issue each requisition using existing service method
            var results = new List<IssueWindowIssueResultResponse>();

            foreach (var reqId in requisitionIds)
            {
                // Only issue requisitions that have at least one item with assigned pieces
                var reqItems = await _reqRepo.GetRequisitionItemsAsync(reqId);
                var hasAssignedItems = reqItems.Any(i => itemPieceMap.ContainsKey(i.Id));
                if (!hasAssignedItems) continue;

                var req = await _reqRepo.GetByIdAsync(reqId);
                if (req == null) continue;

                // Skip requisitions already issued — mark as success so the draft can still finalize
                if (req.Status == "Issued")
                {
                    results.Add(new IssueWindowIssueResultResponse
                    {
                        RequisitionId = reqId,
                        RequisitionNo = req.RequisitionNo,
                        Success = true,
                        Message = "Already issued"
                    });
                    continue;
                }

                try
                {
                    var issueResult = await _reqService.IssueMaterialsAsync(
                        reqId,
                        req.JobCardId ?? 0,
                        request.IssuedBy,
                        request.ReceivedBy
                    );

                    results.Add(new IssueWindowIssueResultResponse
                    {
                        RequisitionId = reqId,
                        RequisitionNo = req.RequisitionNo,
                        Success = issueResult.Success,
                        Message = issueResult.Message,
                        IssueId = issueResult.Success ? issueResult.Data : null
                    });
                }
                catch (Exception ex)
                {
                    results.Add(new IssueWindowIssueResultResponse
                    {
                        RequisitionId = reqId,
                        RequisitionNo = req.RequisitionNo,
                        Success = false,
                        Message = ex.Message
                    });
                }
            }

            // Mark draft as issued
            if (results.Any(r => r.Success))
                await _draftRepo.MarkIssuedAsync(draftId, request.IssuedBy, request.ReceivedBy);

            return results;
        }

        // ── Suggest Cutting Plans (3 strategies) ─────────────────────────────────

        public async Task<IEnumerable<CuttingPlanResponse>> SuggestCuttingPlanAsync(SuggestCuttingPlanRequest request)
        {
            if (!request.Cuts.Any())
                return Enumerable.Empty<CuttingPlanResponse>();

            // Load available inventory pieces for this material
            var rawPieces = request.MaterialId.HasValue
                ? await _pieceRepo.GetAvailablePiecesByMaterialAsync(request.MaterialId.Value, request.Grade, request.DiameterMM)
                : Enumerable.Empty<Models.Stores.MaterialPiece>();

            var pool = rawPieces
                .Select(p => (p.Id, p.PieceNo, LengthMM: p.CurrentLengthMM))
                .ToList();

            var plans = new List<CuttingPlanResponse>
            {
                BuildPlan(request.Cuts, pool, PlanStrategy.MinWaste,    1, "Min Waste",    "Best-fit — minimizes scrap, uses bars most efficiently"),
                BuildPlan(request.Cuts, pool, PlanStrategy.FewestBars,  2, "Fewest Bars",  "First-fit — packs more cuts per bar, fewer saw setups"),
                BuildPlan(request.Cuts, pool, PlanStrategy.GroupBySize, 3, "Group by Size", "Same-length cuts on same bar — one saw setting per cut length"),
            };

            return plans;
        }

        private enum PlanStrategy { MinWaste, FewestBars, GroupBySize }

        private class ActiveBar
        {
            public int PieceId { get; set; }
            public string PieceNo { get; set; } = string.Empty;
            public decimal BarLengthMM { get; set; }
            public decimal RemainingMM { get; set; }
            public List<SuggestCutItem> Cuts { get; set; } = new();
        }

        private CuttingPlanResponse BuildPlan(
            List<SuggestCutItem> inputCuts,
            List<(int Id, string PieceNo, decimal LengthMM)> allPieces,
            PlanStrategy strategy,
            int planIndex,
            string label,
            string description)
        {
            // Independent copy of the pool for each plan simulation
            var pool = strategy == PlanStrategy.FewestBars
                ? allPieces.OrderByDescending(p => p.LengthMM).ToList()
                : allPieces.OrderBy(p => p.LengthMM).ToList();

            // Sort cuts: largest first for BFD/FFD; by size descending for GroupBySize too
            var sortedCuts = inputCuts
                .OrderByDescending(c => c.CutLengthMM)
                .ThenBy(c => c.RequisitionItemId)
                .ThenBy(c => c.CutIndex)
                .ToList();

            var openBars = new List<ActiveBar>();
            var unassigned = new List<SuggestCutItem>();

            foreach (var cut in sortedCuts)
            {
                ActiveBar? chosen = strategy switch
                {
                    // Best Fit: bar with least remaining that still fits
                    PlanStrategy.MinWaste => openBars
                        .Where(b => b.RemainingMM >= cut.CutLengthMM)
                        .OrderBy(b => b.RemainingMM)
                        .FirstOrDefault(),

                    // First Fit: first open bar that fits
                    _ => openBars.FirstOrDefault(b => b.RemainingMM >= cut.CutLengthMM),
                };

                if (chosen == null)
                {
                    // Open a new bar from inventory pool
                    // MinWaste/GroupBySize: smallest bar that fits; FewestBars: largest (already sorted desc)
                    int idx = pool.FindIndex(p => p.LengthMM >= cut.CutLengthMM);
                    if (idx < 0) { unassigned.Add(cut); continue; }

                    var piece = pool[idx];
                    pool.RemoveAt(idx);
                    chosen = new ActiveBar
                    {
                        PieceId = piece.Id,
                        PieceNo = piece.PieceNo,
                        BarLengthMM = piece.LengthMM,
                        RemainingMM = piece.LengthMM,
                    };
                    openBars.Add(chosen);
                }

                chosen.RemainingMM -= cut.CutLengthMM;
                chosen.Cuts.Add(cut);
            }

            var bars = openBars.Select(b => new BarCutPlanResponse
            {
                PieceId = b.PieceId,
                PieceNo = b.PieceNo,
                BarLengthMM = b.BarLengthMM,
                TotalCutMM = b.Cuts.Sum(c => c.CutLengthMM),
                RemainingMM = b.RemainingMM,
                WillBeScrap = b.RemainingMM < MinUsableLengthMM,
                Cuts = b.Cuts.Select(c => new PlanCutItemResponse
                {
                    RequisitionItemId = c.RequisitionItemId,
                    RequisitionId = c.RequisitionId,
                    CutIndex = c.CutIndex,
                    CutLengthMM = c.CutLengthMM,
                    PartName = c.PartName,
                    JobCardNo = c.JobCardNo,
                    RequisitionNo = c.RequisitionNo,
                    MaterialId = c.MaterialId,
                }).ToList(),
            }).ToList();

            return new CuttingPlanResponse
            {
                PlanIndex = planIndex,
                PlanLabel = label,
                PlanDescription = description,
                TotalBars = bars.Count,
                TotalScrapMM = bars.Where(b => b.WillBeScrap).Sum(b => b.RemainingMM),
                TotalStockReturnMM = bars.Where(b => !b.WillBeScrap).Sum(b => b.RemainingMM),
                TotalBarLengthUsedMM = bars.Sum(b => b.TotalCutMM),
                IsComplete = !unassigned.Any(),
                Bars = bars,
            };
        }

        public async Task<bool> DeleteDraftAsync(int id)
        {
            return await _draftRepo.DeleteDraftAsync(id);
        }
    }
}

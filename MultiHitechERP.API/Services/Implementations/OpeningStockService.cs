using MultiHitechERP.API.DTOs.Request;
using MultiHitechERP.API.DTOs.Response;
using MultiHitechERP.API.Models.Stores;
using MultiHitechERP.API.Repositories.Interfaces;
using MultiHitechERP.API.Services.Interfaces;

namespace MultiHitechERP.API.Services.Implementations
{
    public class OpeningStockService : IOpeningStockService
    {
        private readonly IOpeningStockRepository _repo;
        private readonly IMaterialPieceRepository _pieceRepo;
        private readonly IInventoryRepository _inventoryRepo;
        private readonly IWarehouseRepository _warehouseRepo;

        public OpeningStockService(
            IOpeningStockRepository repo,
            IMaterialPieceRepository pieceRepo,
            IInventoryRepository inventoryRepo,
            IWarehouseRepository warehouseRepo)
        {
            _repo = repo;
            _pieceRepo = pieceRepo;
            _inventoryRepo = inventoryRepo;
            _warehouseRepo = warehouseRepo;
        }

        public async Task<OpeningStockDetailResponse> CreateAsync(CreateOpeningStockRequest request)
        {
            var entryNo = await _repo.GenerateEntryNoAsync();
            var entry = new OpeningStockEntry
            {
                EntryNo = entryNo,
                EntryDate = request.EntryDate,
                Remarks = request.Remarks,
                CreatedBy = request.CreatedBy
            };

            var items = MapRequestItems(request.Items);
            var id = await _repo.CreateAsync(entry, items);
            return (await _repo.GetByIdAsync(id))!;
        }

        public async Task<IEnumerable<OpeningStockSummaryResponse>> GetAllAsync() =>
            await _repo.GetAllAsync();

        public async Task<OpeningStockDetailResponse?> GetByIdAsync(int id) =>
            await _repo.GetByIdAsync(id);

        public async Task<OpeningStockDetailResponse> UpdateAsync(int id, CreateOpeningStockRequest request)
        {
            var items = MapRequestItems(request.Items);
            await _repo.UpdateItemsAsync(id, items);
            return (await _repo.GetByIdAsync(id))!;
        }

        public async Task<OpeningStockDetailResponse> ConfirmAsync(int id, ConfirmOpeningStockRequest request)
        {
            var entry = await _repo.GetByIdAsync(id);
            if (entry == null) throw new Exception($"Opening stock entry {id} not found");
            if (entry.Status == "Confirmed") throw new Exception("Entry is already confirmed");

            int totalPieces = 0;
            int totalComponents = 0;

            // Pre-load all warehouses so we can resolve names without per-item DB calls
            var allWarehouses = (await _warehouseRepo.GetAllAsync())
                .ToDictionary(w => w.Id, w => $"{w.Name} — {w.Rack}/{w.RackNo}");

            foreach (var item in entry.Items)
            {
                // Resolve warehouse name for location string
                var warehouseName = item.WarehouseId.HasValue && allWarehouses.TryGetValue(item.WarehouseId.Value, out var wName)
                    ? wName : "";

                if (item.ItemType == "RawMaterial")
                {
                    var (calcLength, weightPerMeter) = CalculateLengthFromWeight(item);
                    var pieces = item.NumberOfPieces ?? 1;
                    var lengthMM = item.LengthPerPieceMM ?? (pieces > 0 && calcLength > 0 ? calcLength / pieces : 0);
                    var weightPerPiece = item.TotalWeightKG.HasValue ? item.TotalWeightKG.Value / pieces : 0;

                    for (int i = 1; i <= pieces; i++)
                    {
                        var pieceNo = $"{entry.EntryNo}-{item.SequenceNo:D2}-{i:D3}";
                        await _pieceRepo.CreateAsync(new MaterialPiece
                        {
                            PieceNo = pieceNo,
                            MaterialId = item.MaterialId ?? 0,
                            MaterialName = item.MaterialName,
                            Grade = item.Grade,
                            Diameter = item.Diameter,
                            OriginalLengthMM = lengthMM,
                            CurrentLengthMM = lengthMM,
                            OriginalWeightKG = weightPerPiece,
                            CurrentWeightKG = weightPerPiece,
                            Status = "Available",
                            GRNId = null,
                            GRNNo = entry.EntryNo,     // OS-202602-001 — marks as opening stock
                            ReceivedDate = entry.EntryDate,
                            WarehouseId = item.WarehouseId,
                            UnitCost = item.UnitCost,
                            CreatedBy = entry.CreatedBy
                        });
                        totalPieces++;
                    }

                    // Update Inventory_Stock (aggregate mm) — pass warehouse name + ID
                    if (item.MaterialId.HasValue && calcLength > 0)
                    {
                        var uom = (item.MaterialType == "Rod" || item.MaterialType == "Pipe" || item.MaterialType == "Forged") ? "mm" : "kg";
                        var qtyToAdd = uom == "mm" ? calcLength : (item.TotalWeightKG ?? 0);
                        await _inventoryRepo.UpsertFromGRNAsync(
                            item.MaterialId.Value,
                            "",
                            item.MaterialName ?? "",
                            qtyToAdd,
                            uom,
                            warehouseName,
                            entry.CreatedBy ?? "",
                            "RawMaterial",
                            entry.EntryNo,
                            item.WarehouseId);
                    }
                }
                else if (item.ItemType == "Component" && item.ComponentId.HasValue)
                {
                    await _inventoryRepo.UpsertFromGRNAsync(
                        item.ComponentId.Value,
                        item.PartNumber ?? "",
                        item.ComponentName ?? "",
                        item.Quantity ?? 0,
                        item.UOM ?? "Pcs",
                        warehouseName,
                        entry.CreatedBy ?? "",
                        "Component",
                        entry.EntryNo,
                        item.WarehouseId);
                    totalComponents++;
                }
            }

            await _repo.ConfirmAsync(id, request.ConfirmedBy, totalPieces, totalComponents);
            return (await _repo.GetByIdAsync(id))!;
        }

        public async Task<bool> DeleteAsync(int id) =>
            await _repo.DeleteAsync(id);

        // ── Helpers ────────────────────────────────────────────────────────────

        private static List<OpeningStockItem> MapRequestItems(List<OpeningStockItemRequest> requests)
        {
            return requests.Select((r, idx) => new OpeningStockItem
            {
                SequenceNo = r.SequenceNo > 0 ? r.SequenceNo : idx + 1,
                ItemType = r.ItemType,
                MaterialId = r.MaterialId,
                MaterialName = r.MaterialName,
                Grade = r.Grade,
                MaterialType = r.MaterialType,
                Diameter = r.Diameter,
                OuterDiameter = r.OuterDiameter,
                InnerDiameter = r.InnerDiameter,
                Width = r.Width,
                Thickness = r.Thickness,
                MaterialDensity = r.MaterialDensity,
                TotalWeightKG = r.TotalWeightKG,
                NumberOfPieces = r.NumberOfPieces,
                LengthPerPieceMM = r.LengthPerPieceMM,
                WarehouseId = r.WarehouseId,
                ComponentId = r.ComponentId,
                ComponentName = r.ComponentName,
                PartNumber = r.PartNumber,
                Quantity = r.Quantity,
                UOM = r.UOM,
                UnitCost = r.UnitCost,
                LineTotal = r.UnitCost.HasValue
                    ? (r.ItemType == "Component"
                        ? r.UnitCost.Value * (r.Quantity ?? 0)
                        : r.UnitCost.Value * (r.TotalWeightKG ?? 0))
                    : null,
                Remarks = r.Remarks,
                SortOrder = idx
            }).ToList();
        }

        // Same formula as GRNService.CalculateLengthFromWeight
        private static (decimal calculatedLength, decimal? weightPerMeter) CalculateLengthFromWeight(OpeningStockItemResponse item)
        {
            if (!item.TotalWeightKG.HasValue || item.TotalWeightKG <= 0 ||
                !item.MaterialDensity.HasValue || item.MaterialDensity <= 0)
                return (0, null);

            decimal area = 0;

            if (item.MaterialType == "Rod")
            {
                if (!item.Diameter.HasValue || item.Diameter.Value <= 0) return (0, null);
                var dCm = item.Diameter.Value / 10;
                area = (decimal)(Math.PI / 4) * dCm * dCm;
            }
            else if (item.MaterialType == "Pipe")
            {
                if (!item.OuterDiameter.HasValue || item.OuterDiameter.Value <= 0) return (0, null);
                var odCm = item.OuterDiameter.Value / 10;
                var idCm = (item.InnerDiameter ?? 0) / 10;
                area = (decimal)(Math.PI / 4) * (odCm * odCm - idCm * idCm);
            }

            if (area <= 0) return (item.CalculatedLengthMM ?? 0, item.WeightPerMeterKG);

            var weightPerMeter = (area * item.MaterialDensity!.Value * 100) / 1000;
            var lengthMM = (item.TotalWeightKG!.Value / weightPerMeter) * 1000;
            return (lengthMM, weightPerMeter);
        }
    }
}

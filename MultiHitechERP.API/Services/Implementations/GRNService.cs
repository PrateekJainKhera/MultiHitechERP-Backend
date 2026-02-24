using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MultiHitechERP.API.DTOs.Request;
using MultiHitechERP.API.DTOs.Response;
using MultiHitechERP.API.Models.Stores;
using MultiHitechERP.API.Repositories.Interfaces;
using MultiHitechERP.API.Services.Interfaces;

namespace MultiHitechERP.API.Services.Implementations
{
    public class GRNService : IGRNService
    {
        private readonly IGRNRepository _grnRepo;
        private readonly IMaterialPieceRepository _pieceRepo;
        private readonly IInventoryRepository _inventoryRepo;

        public GRNService(IGRNRepository grnRepo, IMaterialPieceRepository pieceRepo, IInventoryRepository inventoryRepo)
        {
            _grnRepo = grnRepo;
            _pieceRepo = pieceRepo;
            _inventoryRepo = inventoryRepo;
        }

        public async Task<GRNResponse> CreateGRNAsync(CreateGRNRequest request)
        {
            // Create GRN header
            var grn = new GRN
            {
                GRNNo = request.GRNNo,
                GRNDate = request.GRNDate,
                SupplierId = request.SupplierId,
                SupplierName = request.SupplierName,
                SupplierBatchNo = request.SupplierBatchNo,
                PONo = request.PONo,
                PODate = request.PODate,
                InvoiceNo = request.InvoiceNo,
                InvoiceDate = request.InvoiceDate,
                Status = "Received",
                Remarks = request.Remarks,
                CreatedBy = request.CreatedBy
            };

            var grnId = await _grnRepo.CreateAsync(grn);
            grn.Id = grnId;

            // Calculate totals and create lines + pieces
            int totalPieces = 0;
            decimal totalWeight = 0;
            decimal totalValue = 0;

            foreach (var lineReq in request.Lines)
            {
                // Calculate length from weight
                var (calculatedLength, weightPerMeter) = CalculateLengthFromWeight(lineReq);

                // Create GRN Line
                var grnLine = new GRNLine
                {
                    GRNId = grnId,
                    SequenceNo = lineReq.SequenceNo,
                    MaterialId = lineReq.MaterialId,
                    MaterialName = lineReq.MaterialName,
                    Grade = lineReq.Grade,
                    MaterialType = lineReq.MaterialType,
                    Diameter = lineReq.Diameter,
                    OuterDiameter = lineReq.OuterDiameter,
                    InnerDiameter = lineReq.InnerDiameter,
                    Width = lineReq.Width,
                    Thickness = lineReq.Thickness,
                    MaterialDensity = lineReq.MaterialDensity,
                    TotalWeightKG = lineReq.TotalWeightKG,
                    CalculatedLengthMM = calculatedLength,
                    WeightPerMeterKG = weightPerMeter,
                    NumberOfPieces = lineReq.NumberOfPieces,
                    LengthPerPieceMM = lineReq.LengthPerPieceMM ?? (calculatedLength / lineReq.NumberOfPieces),
                    UnitPrice = lineReq.UnitPrice,
                    LineTotal = lineReq.UnitPrice.HasValue ? lineReq.UnitPrice.Value * lineReq.TotalWeightKG : null,
                    Remarks = lineReq.Remarks
                };

                await _grnRepo.CreateLineAsync(grnLine);

                // Create individual material pieces
                for (int i = 1; i <= lineReq.NumberOfPieces; i++)
                {
                    var pieceNo = $"{request.GRNNo}-{lineReq.SequenceNo:D2}-{i:D3}";
                    var lengthMM = grnLine.LengthPerPieceMM ?? 0;
                    var weightKG = weightPerMeter.HasValue && lengthMM > 0
                        ? (weightPerMeter.Value * lengthMM / 1000)
                        : (lineReq.TotalWeightKG / lineReq.NumberOfPieces);

                    var piece = new MaterialPiece
                    {
                        PieceNo = pieceNo,
                        MaterialId = lineReq.MaterialId,
                        MaterialName = lineReq.MaterialName,
                        Grade = lineReq.Grade,
                        Diameter = lineReq.Diameter,
                        OriginalLengthMM = lengthMM,
                        CurrentLengthMM = lengthMM,
                        OriginalWeightKG = weightKG,
                        CurrentWeightKG = weightKG,
                        Status = "Available",
                        WarehouseId = lineReq.WarehouseId,
                        StorageLocation = lineReq.WarehouseId.HasValue ? null : "Main Warehouse",
                        GRNId = grnId,
                        GRNNo = request.GRNNo,
                        ReceivedDate = request.GRNDate,
                        SupplierBatchNo = request.SupplierBatchNo,
                        SupplierId = request.SupplierId,
                        UnitCost = grnLine.LineTotal.HasValue && lineReq.NumberOfPieces > 0
                            ? grnLine.LineTotal.Value / lineReq.NumberOfPieces
                            : null,
                        CreatedBy = request.CreatedBy
                    };

                    await _pieceRepo.CreateAsync(piece);
                    totalPieces++;
                }

                totalWeight += lineReq.TotalWeightKG;
                totalValue += grnLine.LineTotal ?? 0;
            }

            // Update GRN totals
            grn.TotalPieces = totalPieces;
            grn.TotalWeight = totalWeight;
            grn.TotalValue = totalValue;
            await _grnRepo.UpdateAsync(grn);

            // Update Inventory_Stock for each material line
            foreach (var lineReq in request.Lines)
            {
                // Calculate length from weight (same logic as above)
                var (calculatedLength, weightPerMeter) = CalculateLengthFromWeight(lineReq);

                // Determine UOM based on material type
                string uom = lineReq.MaterialType == "Rod" || lineReq.MaterialType == "Pipe" || lineReq.MaterialType == "Forged"
                    ? "mm"
                    : "kg";

                // Determine what to add to inventory
                decimal quantityToAdd = uom == "mm" ? calculatedLength : lineReq.TotalWeightKG;

                // Upsert inventory stock (MaterialCode will be fetched from Masters_Materials)
                await _inventoryRepo.UpsertFromGRNAsync(
                    lineReq.MaterialId,
                    null, // Will be fetched from Masters_Materials
                    lineReq.MaterialName,
                    quantityToAdd,
                    uom,
                    "Main Warehouse",
                    request.CreatedBy,
                    "RawMaterial",
                    request.GRNNo   // e.g. GRN-202602-001 — marks as GRN inward
                );
            }

            return MapToResponse(grn);
        }

        public async Task<GRNResponse?> GetByIdAsync(int id)
        {
            var grn = await _grnRepo.GetByIdAsync(id);
            return grn == null ? null : MapToResponse(grn);
        }

        public async Task<GRNResponse?> GetByGRNNoAsync(string grnNo)
        {
            var grn = await _grnRepo.GetByGRNNoAsync(grnNo);
            return grn == null ? null : MapToResponse(grn);
        }

        public async Task<IEnumerable<GRNResponse>> GetAllAsync()
        {
            var grns = await _grnRepo.GetAllAsync();
            return grns.Select(MapToResponse);
        }

        public async Task<IEnumerable<GRNResponse>> GetBySupplierId(int supplierId)
        {
            var grns = await _grnRepo.GetBySupplierId(supplierId);
            return grns.Select(MapToResponse);
        }

        public async Task<bool> UpdateStatusAsync(int id, string status, string updatedBy)
        {
            var grn = await _grnRepo.GetByIdAsync(id);
            if (grn == null) return false;

            grn.Status = status;
            grn.UpdatedBy = updatedBy;
            return await _grnRepo.UpdateAsync(grn);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _grnRepo.DeleteAsync(id);
        }

        public async Task<GRNResponse?> GetGRNWithLinesAsync(int id)
        {
            var grn = await _grnRepo.GetByIdAsync(id);
            if (grn == null) return null;

            var lines = await _grnRepo.GetLinesByGRNIdAsync(id);
            var response = MapToResponse(grn);
            response.Lines = lines.Select(MapLineToResponse).ToList();

            return response;
        }

        private (decimal calculatedLength, decimal? weightPerMeter) CalculateLengthFromWeight(CreateGRNLineRequest line)
        {
            if (line.TotalWeightKG <= 0 || line.MaterialDensity <= 0)
                return (0, null);

            decimal area = 0;

            if (line.MaterialType == "Rod")
            {
                if (!line.Diameter.HasValue || line.Diameter.Value <= 0) return (0, null);
                var diameterCm = line.Diameter.Value / 10;
                area = (decimal)(Math.PI / 4) * diameterCm * diameterCm;
            }
            else if (line.MaterialType == "Pipe")
            {
                if (!line.OuterDiameter.HasValue || line.OuterDiameter.Value <= 0) return (0, null);
                var odCm = line.OuterDiameter.Value / 10;
                var idCm = (line.InnerDiameter ?? 0) / 10;
                area = (decimal)(Math.PI / 4) * (odCm * odCm - idCm * idCm);
            }

            if (area <= 0) return (0, null);

            var weightPerMeter = (area * line.MaterialDensity * 100) / 1000;
            var lengthMeters = line.TotalWeightKG / weightPerMeter;
            var lengthMM = lengthMeters * 1000;

            return (lengthMM, weightPerMeter);
        }

        private GRNResponse MapToResponse(GRN grn)
        {
            return new GRNResponse
            {
                Id = grn.Id,
                GRNNo = grn.GRNNo,
                GRNDate = grn.GRNDate,
                SupplierId = grn.SupplierId,
                SupplierName = grn.SupplierName,
                SupplierBatchNo = grn.SupplierBatchNo,
                PONo = grn.PONo,
                PODate = grn.PODate,
                InvoiceNo = grn.InvoiceNo,
                InvoiceDate = grn.InvoiceDate,
                TotalPieces = grn.TotalPieces,
                TotalWeight = grn.TotalWeight,
                TotalValue = grn.TotalValue,
                Status = grn.Status,
                QualityCheckStatus = grn.QualityCheckStatus,
                QualityCheckedBy = grn.QualityCheckedBy,
                QualityCheckedAt = grn.QualityCheckedAt,
                QualityRemarks = grn.QualityRemarks,
                Remarks = grn.Remarks,
                CreatedAt = grn.CreatedAt,
                CreatedBy = grn.CreatedBy,
                UpdatedAt = grn.UpdatedAt,
                UpdatedBy = grn.UpdatedBy
            };
        }

        private GRNLineResponse MapLineToResponse(GRNLine line)
        {
            return new GRNLineResponse
            {
                Id = line.Id,
                GRNId = line.GRNId,
                SequenceNo = line.SequenceNo,
                MaterialId = line.MaterialId,
                MaterialName = line.MaterialName,
                Grade = line.Grade,
                MaterialType = line.MaterialType,
                Diameter = line.Diameter,
                OuterDiameter = line.OuterDiameter,
                InnerDiameter = line.InnerDiameter,
                Width = line.Width,
                Thickness = line.Thickness,
                MaterialDensity = line.MaterialDensity,
                TotalWeightKG = line.TotalWeightKG,
                CalculatedLengthMM = line.CalculatedLengthMM,
                WeightPerMeterKG = line.WeightPerMeterKG,
                NumberOfPieces = line.NumberOfPieces,
                LengthPerPieceMM = line.LengthPerPieceMM,
                UnitPrice = line.UnitPrice,
                LineTotal = line.LineTotal,
                Remarks = line.Remarks
            };
        }
    }
}

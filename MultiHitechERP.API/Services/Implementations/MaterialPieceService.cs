using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MultiHitechERP.API.DTOs.Response;
using MultiHitechERP.API.Models.Stores;
using MultiHitechERP.API.Repositories.Interfaces;
using MultiHitechERP.API.Services.Interfaces;

namespace MultiHitechERP.API.Services.Implementations
{
    public class MaterialPieceService : IMaterialPieceService
    {
        private readonly IMaterialPieceRepository _pieceRepo;

        public MaterialPieceService(IMaterialPieceRepository pieceRepo)
        {
            _pieceRepo = pieceRepo;
        }

        public async Task<MaterialPieceResponse?> GetByIdAsync(int id)
        {
            var piece = await _pieceRepo.GetByIdAsync(id);
            return piece == null ? null : MapToResponse(piece);
        }

        public async Task<MaterialPieceResponse?> GetByPieceNoAsync(string pieceNo)
        {
            var piece = await _pieceRepo.GetByPieceNoAsync(pieceNo);
            return piece == null ? null : MapToResponse(piece);
        }

        public async Task<IEnumerable<MaterialPieceResponse>> GetAllAsync()
        {
            var pieces = await _pieceRepo.GetAllAsync();
            return pieces.Select(MapToResponse);
        }

        public async Task<IEnumerable<MaterialPieceResponse>> GetByMaterialIdAsync(int materialId)
        {
            var pieces = await _pieceRepo.GetByMaterialIdAsync(materialId);
            return pieces.Select(MapToResponse);
        }

        public async Task<IEnumerable<MaterialPieceResponse>> GetByStatusAsync(string status)
        {
            var pieces = await _pieceRepo.GetByStatusAsync(status);
            return pieces.Select(MapToResponse);
        }

        public async Task<IEnumerable<MaterialPieceResponse>> GetByGRNIdAsync(int grnId)
        {
            var pieces = await _pieceRepo.GetByGRNIdAsync(grnId);
            return pieces.Select(MapToResponse);
        }

        public async Task<IEnumerable<MaterialPieceResponse>> GetAvailablePiecesAsync()
        {
            var pieces = await _pieceRepo.GetAvailablePiecesAsync();
            return pieces.Select(MapToResponse);
        }

        public async Task<IEnumerable<MaterialPieceResponse>> GetWastagePiecesAsync()
        {
            var pieces = await _pieceRepo.GetWastagePiecesAsync();
            return pieces.Select(MapToResponse);
        }

        public async Task<bool> UpdateLengthAsync(int pieceId, decimal newLengthMM, string updatedBy)
        {
            return await _pieceRepo.UpdateLengthAsync(pieceId, newLengthMM);
        }

        public async Task<bool> AdjustLengthAsync(int pieceId, decimal newLengthMM, string remark, string adjustedBy)
        {
            var piece = await _pieceRepo.GetByIdAsync(pieceId);
            if (piece == null)
                throw new System.Exception("Material piece not found");

            if (piece.Status != "Available" && piece.Status != "Reserved")
                throw new System.Exception($"Cannot adjust length of a piece with status '{piece.Status}'. Only Available or Reserved pieces can be adjusted.");

            if (newLengthMM <= 0)
                throw new System.Exception("New length must be greater than 0");

            // Recalculate weight proportionally based on original
            decimal newWeightKG = piece.OriginalLengthMM > 0
                ? (newLengthMM / piece.OriginalLengthMM) * piece.OriginalWeightKG
                : piece.CurrentWeightKG;

            var updatedBy = $"{adjustedBy}: {remark}";
            return await _pieceRepo.AdjustLengthAsync(pieceId, newLengthMM, newWeightKG, updatedBy);
        }

        public async Task<bool> MarkAsWastageAsync(int pieceId, string reason, decimal? scrapValue, string updatedBy)
        {
            return await _pieceRepo.MarkAsWastageAsync(pieceId, reason, scrapValue);
        }

        public async Task<decimal> GetTotalStockByMaterialIdAsync(int materialId)
        {
            return await _pieceRepo.GetTotalStockByMaterialIdAsync(materialId);
        }

        public async Task<decimal> GetAvailableStockByMaterialIdAsync(int materialId)
        {
            return await _pieceRepo.GetAvailableStockByMaterialIdAsync(materialId);
        }

        public async Task<bool> CheckMaterialAvailability(int materialId, decimal requiredLengthMM)
        {
            var availableStock = await _pieceRepo.GetAvailableStockByMaterialIdAsync(materialId);
            return availableStock >= requiredLengthMM;
        }

        private MaterialPieceResponse MapToResponse(MaterialPiece piece)
        {
            return new MaterialPieceResponse
            {
                Id = piece.Id,
                PieceNo = piece.PieceNo,
                MaterialId = piece.MaterialId,
                MaterialCode = piece.MaterialCode,
                MaterialName = piece.MaterialName,
                Grade = piece.Grade,
                Diameter = piece.Diameter,
                OriginalLengthMM = piece.OriginalLengthMM,
                CurrentLengthMM = piece.CurrentLengthMM,
                OriginalWeightKG = piece.OriginalWeightKG,
                CurrentWeightKG = piece.CurrentWeightKG,
                Status = piece.Status,
                AllocatedToRequisitionId = piece.AllocatedToRequisitionId,
                IssuedToJobCardId = piece.IssuedToJobCardId,
                StorageLocation = piece.StorageLocation,
                BinNumber = piece.BinNumber,
                RackNumber = piece.RackNumber,
                GRNId = piece.GRNId,
                GRNNo = piece.GRNNo,
                ReceivedDate = piece.ReceivedDate,
                SupplierBatchNo = piece.SupplierBatchNo,
                SupplierId = piece.SupplierId,
                UnitCost = piece.UnitCost,
                IsWastage = piece.IsWastage,
                WastageReason = piece.WastageReason,
                ScrapValue = piece.ScrapValue,
                CreatedAt = piece.CreatedAt,
                CreatedBy = piece.CreatedBy,
                UpdatedAt = piece.UpdatedAt,
                UpdatedBy = piece.UpdatedBy
            };
        }
    }
}

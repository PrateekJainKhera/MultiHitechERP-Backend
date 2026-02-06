using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using MultiHitechERP.API.DTOs.Response;
using MultiHitechERP.API.Models.Stores;
using MultiHitechERP.API.Repositories.Interfaces;
using MultiHitechERP.API.Services.Interfaces;

namespace MultiHitechERP.API.Services.Implementations
{
    public class MaterialUsageService : IMaterialUsageService
    {
        private readonly IMaterialUsageHistoryRepository _usageRepo;
        private readonly IMaterialPieceRepository _pieceRepo;

        public MaterialUsageService(
            IMaterialUsageHistoryRepository usageRepo,
            IMaterialPieceRepository pieceRepo)
        {
            _usageRepo = usageRepo;
            _pieceRepo = pieceRepo;
        }

        public async Task<int> RecordUsageAsync(
            int pieceId,
            decimal lengthUsedMM,
            int? orderId,
            int? jobCardId,
            string? cutByOperator,
            string? machineUsed,
            string? notes,
            string createdBy)
        {
            // Get the piece
            var piece = await _pieceRepo.GetByIdAsync(pieceId);
            if (piece == null)
                throw new Exception($"Material piece {pieceId} not found");

            // Calculate new length
            var newLength = piece.CurrentLengthMM - lengthUsedMM;
            if (newLength < 0)
                throw new Exception("Insufficient material length in piece");

            // Update piece length
            await _pieceRepo.UpdateLengthAsync(pieceId, newLength);

            // Create usage history record
            var usage = new MaterialUsageHistory
            {
                MaterialPieceId = pieceId,
                PieceNo = piece.PieceNo,
                OrderId = orderId,
                JobCardId = jobCardId,
                LengthUsedMM = lengthUsedMM,
                LengthRemainingMM = newLength,
                CuttingDate = DateTime.UtcNow,
                CutByOperator = cutByOperator,
                MachineUsed = machineUsed,
                Notes = notes,
                CreatedBy = createdBy
            };

            return await _usageRepo.CreateAsync(usage);
        }

        public async Task<MaterialUsageHistoryResponse?> GetByIdAsync(int id)
        {
            var usage = await _usageRepo.GetByIdAsync(id);
            return usage == null ? null : MapToResponse(usage);
        }

        public async Task<IEnumerable<MaterialUsageHistoryResponse>> GetByPieceIdAsync(int pieceId)
        {
            var usages = await _usageRepo.GetByPieceIdAsync(pieceId);
            return usages.Select(MapToResponse);
        }

        public async Task<IEnumerable<MaterialUsageHistoryResponse>> GetByOrderIdAsync(int orderId)
        {
            var usages = await _usageRepo.GetByOrderIdAsync(orderId);
            return usages.Select(MapToResponse);
        }

        public async Task<IEnumerable<MaterialUsageHistoryResponse>> GetByJobCardIdAsync(int jobCardId)
        {
            var usages = await _usageRepo.GetByJobCardIdAsync(jobCardId);
            return usages.Select(MapToResponse);
        }

        public async Task<IEnumerable<MaterialUsageHistoryResponse>> GetAllAsync()
        {
            var usages = await _usageRepo.GetAllAsync();
            return usages.Select(MapToResponse);
        }

        private MaterialUsageHistoryResponse MapToResponse(MaterialUsageHistory usage)
        {
            return new MaterialUsageHistoryResponse
            {
                Id = usage.Id,
                MaterialPieceId = usage.MaterialPieceId,
                PieceNo = usage.PieceNo,
                OrderId = usage.OrderId,
                OrderNo = usage.OrderNo,
                ChildPartId = usage.ChildPartId,
                ChildPartName = usage.ChildPartName,
                ProductName = usage.ProductName,
                JobCardId = usage.JobCardId,
                JobCardNo = usage.JobCardNo,
                LengthUsedMM = usage.LengthUsedMM,
                LengthRemainingMM = usage.LengthRemainingMM,
                WastageGeneratedMM = usage.WastageGeneratedMM,
                CuttingDate = usage.CuttingDate,
                CutByOperator = usage.CutByOperator,
                CutByOperatorId = usage.CutByOperatorId,
                MachineUsed = usage.MachineUsed,
                MachineId = usage.MachineId,
                Notes = usage.Notes,
                CreatedAt = usage.CreatedAt,
                CreatedBy = usage.CreatedBy
            };
        }
    }
}

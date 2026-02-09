using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MultiHitechERP.API.DTOs.Response;
using MultiHitechERP.API.Models.Stores;
using MultiHitechERP.API.Repositories.Interfaces;
using MultiHitechERP.API.Services.Interfaces;

namespace MultiHitechERP.API.Services.Implementations
{
    /// <summary>
    /// MaterialRequisitionService implementation with FIFO allocation logic
    /// Handles material requisition workflow and physical material allocation
    /// </summary>
    public class MaterialRequisitionService : IMaterialRequisitionService
    {
        private readonly IMaterialRequisitionRepository _requisitionRepository;
        private readonly IMaterialPieceRepository _pieceRepository;
        private readonly IMaterialIssueRepository _issueRepository;

        public MaterialRequisitionService(
            IMaterialRequisitionRepository requisitionRepository,
            IMaterialPieceRepository pieceRepository,
            IMaterialIssueRepository issueRepository)
        {
            _requisitionRepository = requisitionRepository;
            _pieceRepository = pieceRepository;
            _issueRepository = issueRepository;
        }

        public async Task<ApiResponse<MaterialRequisition>> GetByIdAsync(int id)
        {
            var requisition = await _requisitionRepository.GetByIdAsync(id);
            if (requisition == null)
                return ApiResponse<MaterialRequisition>.ErrorResponse("Material requisition not found");

            return ApiResponse<MaterialRequisition>.SuccessResponse(requisition);
        }

        public async Task<ApiResponse<MaterialRequisition>> GetByRequisitionNoAsync(string requisitionNo)
        {
            if (string.IsNullOrWhiteSpace(requisitionNo))
                return ApiResponse<MaterialRequisition>.ErrorResponse("Requisition number is required");

            var requisition = await _requisitionRepository.GetByRequisitionNoAsync(requisitionNo);
            if (requisition == null)
                return ApiResponse<MaterialRequisition>.ErrorResponse("Material requisition not found");

            return ApiResponse<MaterialRequisition>.SuccessResponse(requisition);
        }

        public async Task<ApiResponse<IEnumerable<MaterialRequisition>>> GetAllAsync()
        {
            var requisitions = await _requisitionRepository.GetAllAsync();
            return ApiResponse<IEnumerable<MaterialRequisition>>.SuccessResponse(requisitions);
        }

        public async Task<ApiResponse<IEnumerable<MaterialRequisition>>> GetByJobCardIdAsync(int jobCardId)
        {
            var requisitions = await _requisitionRepository.GetByJobCardIdAsync(jobCardId);
            return ApiResponse<IEnumerable<MaterialRequisition>>.SuccessResponse(requisitions);
        }

        public async Task<ApiResponse<IEnumerable<MaterialRequisition>>> GetByOrderIdAsync(int orderId)
        {
            var requisitions = await _requisitionRepository.GetByOrderIdAsync(orderId);
            return ApiResponse<IEnumerable<MaterialRequisition>>.SuccessResponse(requisitions);
        }

        public async Task<ApiResponse<IEnumerable<MaterialRequisition>>> GetByStatusAsync(string status)
        {
            if (string.IsNullOrWhiteSpace(status))
                return ApiResponse<IEnumerable<MaterialRequisition>>.ErrorResponse("Status is required");

            var requisitions = await _requisitionRepository.GetByStatusAsync(status);
            return ApiResponse<IEnumerable<MaterialRequisition>>.SuccessResponse(requisitions);
        }

        public async Task<ApiResponse<int>> CreateRequisitionAsync(MaterialRequisition requisition)
        {
            // Validate required fields
            if (string.IsNullOrWhiteSpace(requisition.RequisitionNo))
                return ApiResponse<int>.ErrorResponse("Requisition number is required");

            if (requisition.RequisitionDate == default)
                return ApiResponse<int>.ErrorResponse("Requisition date is required");

            // Check for duplicate requisition number
            var existing = await _requisitionRepository.GetByRequisitionNoAsync(requisition.RequisitionNo);
            if (existing != null)
                return ApiResponse<int>.ErrorResponse($"Requisition number '{requisition.RequisitionNo}' already exists");

            // Set default values
            if (string.IsNullOrWhiteSpace(requisition.Status))
                requisition.Status = "Pending";

            if (string.IsNullOrWhiteSpace(requisition.Priority))
                requisition.Priority = "Medium";

            var id = await _requisitionRepository.InsertAsync(requisition);
            return ApiResponse<int>.SuccessResponse(id, "Material requisition created successfully");
        }

        public async Task<ApiResponse<int>> CreateRequisitionWithItemsAsync(MaterialRequisition requisition, List<MaterialRequisitionItem> items)
        {
            // Validate required fields
            if (string.IsNullOrWhiteSpace(requisition.RequisitionNo))
                return ApiResponse<int>.ErrorResponse("Requisition number is required");

            if (requisition.RequisitionDate == default)
                return ApiResponse<int>.ErrorResponse("Requisition date is required");

            // Check for duplicate requisition number
            var existing = await _requisitionRepository.GetByRequisitionNoAsync(requisition.RequisitionNo);
            if (existing != null)
                return ApiResponse<int>.ErrorResponse($"Requisition number '{requisition.RequisitionNo}' already exists");

            // Set default values
            if (string.IsNullOrWhiteSpace(requisition.Status))
                requisition.Status = "Pending";

            if (string.IsNullOrWhiteSpace(requisition.Priority))
                requisition.Priority = "Medium";

            // Create requisition
            var requisitionId = await _requisitionRepository.InsertAsync(requisition);

            // Create requisition items if provided
            if (items != null && items.Count > 0)
            {
                int lineNo = 1;
                foreach (var item in items)
                {
                    item.RequisitionId = requisitionId;
                    item.LineNo = lineNo++;
                    await _requisitionRepository.InsertRequisitionItemAsync(item);
                }
            }

            return ApiResponse<int>.SuccessResponse(requisitionId, $"Material requisition created successfully with {items?.Count ?? 0} item(s)");
        }

        public async Task<ApiResponse<IEnumerable<MaterialRequisitionItem>>> GetRequisitionItemsAsync(int requisitionId)
        {
            // Validate requisition exists
            var requisition = await _requisitionRepository.GetByIdAsync(requisitionId);
            if (requisition == null)
                return ApiResponse<IEnumerable<MaterialRequisitionItem>>.ErrorResponse("Material requisition not found");

            var items = await _requisitionRepository.GetRequisitionItemsAsync(requisitionId);
            return ApiResponse<IEnumerable<MaterialRequisitionItem>>.SuccessResponse(items);
        }

        public async Task<ApiResponse<bool>> UpdateRequisitionAsync(MaterialRequisition requisition)
        {
            // Check if requisition exists
            var existing = await _requisitionRepository.GetByIdAsync(requisition.Id);
            if (existing == null)
                return ApiResponse<bool>.ErrorResponse("Material requisition not found");

            // Cannot update if already completed or cancelled
            if (existing.Status == "Completed" || existing.Status == "Cancelled")
                return ApiResponse<bool>.ErrorResponse($"Cannot update requisition with status '{existing.Status}'");

            var success = await _requisitionRepository.UpdateAsync(requisition);
            if (!success)
                return ApiResponse<bool>.ErrorResponse("Failed to update material requisition");

            return ApiResponse<bool>.SuccessResponse(true, "Material requisition updated successfully");
        }

        public async Task<ApiResponse<bool>> DeleteRequisitionAsync(int id)
        {
            // Check if requisition exists
            var existing = await _requisitionRepository.GetByIdAsync(id);
            if (existing == null)
                return ApiResponse<bool>.ErrorResponse("Material requisition not found");

            // Cannot delete if approved or completed
            if (existing.Status == "Approved" || existing.Status == "Completed" || existing.Status == "Issued")
                return ApiResponse<bool>.ErrorResponse($"Cannot delete requisition with status '{existing.Status}'");

            // Deallocate any allocated materials
            await DeallocateMaterialsAsync(id);

            var success = await _requisitionRepository.DeleteAsync(id);
            if (!success)
                return ApiResponse<bool>.ErrorResponse("Failed to delete material requisition");

            return ApiResponse<bool>.SuccessResponse(true, "Material requisition deleted successfully");
        }

        public async Task<ApiResponse<bool>> ApproveRequisitionAsync(int id, string approvedBy)
        {
            if (string.IsNullOrWhiteSpace(approvedBy))
                return ApiResponse<bool>.ErrorResponse("Approver information is required");

            // Check if requisition exists
            var existing = await _requisitionRepository.GetByIdAsync(id);
            if (existing == null)
                return ApiResponse<bool>.ErrorResponse("Material requisition not found");

            // Can only approve pending requisitions
            if (existing.Status != "Pending")
                return ApiResponse<bool>.ErrorResponse($"Cannot approve requisition with status '{existing.Status}'");

            var success = await _requisitionRepository.ApproveRequisitionAsync(id, approvedBy);
            if (!success)
                return ApiResponse<bool>.ErrorResponse("Failed to approve material requisition");

            return ApiResponse<bool>.SuccessResponse(true, "Material requisition approved successfully");
        }

        public async Task<ApiResponse<bool>> RejectRequisitionAsync(int id, string rejectedBy, string? reason)
        {
            if (string.IsNullOrWhiteSpace(rejectedBy))
                return ApiResponse<bool>.ErrorResponse("Rejector information is required");

            // Check if requisition exists
            var existing = await _requisitionRepository.GetByIdAsync(id);
            if (existing == null)
                return ApiResponse<bool>.ErrorResponse("Material requisition not found");

            // Can only reject pending requisitions
            if (existing.Status != "Pending")
                return ApiResponse<bool>.ErrorResponse($"Cannot reject requisition with status '{existing.Status}'");

            // Deallocate any allocated materials
            await DeallocateMaterialsAsync(id);

            var success = await _requisitionRepository.RejectRequisitionAsync(id, rejectedBy, reason);
            if (!success)
                return ApiResponse<bool>.ErrorResponse("Failed to reject material requisition");

            return ApiResponse<bool>.SuccessResponse(true, "Material requisition rejected successfully");
        }

        public async Task<ApiResponse<bool>> UpdateStatusAsync(int id, string status)
        {
            if (string.IsNullOrWhiteSpace(status))
                return ApiResponse<bool>.ErrorResponse("Status is required");

            var success = await _requisitionRepository.UpdateStatusAsync(id, status);
            if (!success)
                return ApiResponse<bool>.ErrorResponse("Failed to update status");

            return ApiResponse<bool>.SuccessResponse(true, "Status updated successfully");
        }

        public async Task<ApiResponse<bool>> AllocateMaterialsAsync(int requisitionId, int materialId, decimal requiredQuantityMM)
        {
            // Check if requisition exists and is approved
            var requisition = await _requisitionRepository.GetByIdAsync(requisitionId);
            if (requisition == null)
                return ApiResponse<bool>.ErrorResponse("Material requisition not found");

            if (requisition.Status != "Approved")
                return ApiResponse<bool>.ErrorResponse($"Cannot allocate materials for requisition with status '{requisition.Status}'");

            // Get available pieces using FIFO (oldest first)
            var availablePieces = await _pieceRepository.GetAvailablePiecesByFIFOAsync(materialId, requiredQuantityMM);
            var piecesList = availablePieces.ToList();

            if (!piecesList.Any())
                return ApiResponse<bool>.ErrorResponse("No available material pieces found");

            // Allocate pieces using FIFO logic
            decimal allocatedQuantity = 0;
            int allocatedCount = 0;

            foreach (var piece in piecesList)
            {
                if (allocatedQuantity >= requiredQuantityMM)
                    break;

                var success = await _pieceRepository.AllocatePieceAsync(piece.Id, requisitionId);
                if (success)
                {
                    allocatedQuantity += piece.CurrentLengthMM;
                    allocatedCount++;
                }
            }

            if (allocatedQuantity < requiredQuantityMM)
            {
                // Insufficient material - deallocate what was allocated
                await DeallocateMaterialsAsync(requisitionId);
                return ApiResponse<bool>.ErrorResponse(
                    $"Insufficient material available. Required: {requiredQuantityMM}mm, Available: {allocatedQuantity}mm");
            }

            return ApiResponse<bool>.SuccessResponse(true,
                $"Successfully allocated {allocatedCount} piece(s) totaling {allocatedQuantity}mm");
        }

        public async Task<ApiResponse<bool>> DeallocateMaterialsAsync(int requisitionId)
        {
            // Get all allocated pieces for this requisition
            var allocatedPieces = await _pieceRepository.GetAllocatedPiecesAsync(requisitionId);
            var piecesList = allocatedPieces.ToList();

            if (!piecesList.Any())
                return ApiResponse<bool>.SuccessResponse(true, "No allocated materials to deallocate");

            // Return all pieces to available status
            foreach (var piece in piecesList)
            {
                await _pieceRepository.ReturnPieceAsync(piece.Id);
            }

            return ApiResponse<bool>.SuccessResponse(true,
                $"Successfully deallocated {piecesList.Count} piece(s)");
        }

        public async Task<ApiResponse<int>> IssueMaterialsAsync(int requisitionId, int jobCardId, string issuedBy, string receivedBy)
        {
            // Validate inputs
            if (string.IsNullOrWhiteSpace(issuedBy))
                return ApiResponse<int>.ErrorResponse("Issuer information is required");

            if (string.IsNullOrWhiteSpace(receivedBy))
                return ApiResponse<int>.ErrorResponse("Receiver information is required");

            // Check if requisition exists and is approved
            var requisition = await _requisitionRepository.GetByIdAsync(requisitionId);
            if (requisition == null)
                return ApiResponse<int>.ErrorResponse("Material requisition not found");

            if (requisition.Status != "Approved")
                return ApiResponse<int>.ErrorResponse($"Cannot issue materials for requisition with status '{requisition.Status}'");

            // Get allocated pieces
            var allocatedPieces = await _pieceRepository.GetAllocatedPiecesAsync(requisitionId);
            var piecesList = allocatedPieces.ToList();

            if (!piecesList.Any())
                return ApiResponse<int>.ErrorResponse("No allocated materials found for this requisition");

            // Calculate totals
            int totalPieces = piecesList.Count;
            decimal totalLengthMM = piecesList.Sum(p => p.CurrentLengthMM);
            decimal totalWeightKG = piecesList.Sum(p => p.CurrentWeightKG);

            // Issue all pieces to job card
            foreach (var piece in piecesList)
            {
                await _pieceRepository.IssuePieceAsync(piece.Id, jobCardId, DateTime.UtcNow, issuedBy);
            }

            // Create material issue record
            var materialIssue = new MaterialIssue
            {
                IssueNo = GenerateIssueNo(),
                IssueDate = DateTime.UtcNow,
                RequisitionId = requisitionId,
                JobCardNo = requisition.JobCardNo,
                OrderNo = requisition.OrderNo,
                MaterialName = piecesList.First().MaterialId.ToString(), // Would need to join with Materials table
                TotalPieces = totalPieces,
                TotalIssuedLengthMM = totalLengthMM,
                TotalIssuedWeightKG = totalWeightKG,
                Status = "Issued",
                IssuedById = issuedBy,
                ReceivedById = receivedBy
            };

            var issueId = await _issueRepository.InsertAsync(materialIssue);

            // Update requisition status to "Issued"
            await _requisitionRepository.UpdateStatusAsync(requisitionId, "Issued");

            return ApiResponse<int>.SuccessResponse(issueId,
                $"Successfully issued {totalPieces} piece(s) totaling {totalLengthMM}mm");
        }

        public async Task<ApiResponse<IEnumerable<MaterialPiece>>> GetAllocatedPiecesAsync(int requisitionId)
        {
            var pieces = await _pieceRepository.GetAllocatedPiecesAsync(requisitionId);
            return ApiResponse<IEnumerable<MaterialPiece>>.SuccessResponse(pieces);
        }

        public async Task<ApiResponse<IEnumerable<MaterialIssue>>> GetIssuanceHistoryAsync(int requisitionId)
        {
            var issues = await _issueRepository.GetByRequisitionIdAsync(requisitionId);
            return ApiResponse<IEnumerable<MaterialIssue>>.SuccessResponse(issues);
        }

        public async Task<ApiResponse<IEnumerable<MaterialRequisition>>> GetPendingRequisitionsAsync()
        {
            var requisitions = await _requisitionRepository.GetPendingRequisitionsAsync();
            return ApiResponse<IEnumerable<MaterialRequisition>>.SuccessResponse(requisitions);
        }

        public async Task<ApiResponse<IEnumerable<MaterialRequisition>>> GetApprovedRequisitionsAsync()
        {
            var requisitions = await _requisitionRepository.GetApprovedRequisitionsAsync();
            return ApiResponse<IEnumerable<MaterialRequisition>>.SuccessResponse(requisitions);
        }

        public async Task<ApiResponse<IEnumerable<MaterialRequisition>>> GetByPriorityAsync(string priority)
        {
            if (string.IsNullOrWhiteSpace(priority))
                return ApiResponse<IEnumerable<MaterialRequisition>>.ErrorResponse("Priority is required");

            var requisitions = await _requisitionRepository.GetByPriorityAsync(priority);
            return ApiResponse<IEnumerable<MaterialRequisition>>.SuccessResponse(requisitions);
        }

        public async Task<ApiResponse<IEnumerable<MaterialRequisition>>> GetOverdueRequisitionsAsync()
        {
            var requisitions = await _requisitionRepository.GetOverdueRequisitionsAsync();
            return ApiResponse<IEnumerable<MaterialRequisition>>.SuccessResponse(requisitions);
        }

        // Helper Methods
        private string GenerateIssueNo()
        {
            return $"ISS-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..8].ToUpper()}";
        }
    }
}

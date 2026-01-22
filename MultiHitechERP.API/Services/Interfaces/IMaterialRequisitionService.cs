using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MultiHitechERP.API.DTOs.Response;
using MultiHitechERP.API.Models.Stores;

namespace MultiHitechERP.API.Services.Interfaces
{
    /// <summary>
    /// Service interface for Material Requisition business logic
    /// Includes FIFO allocation and material issuance workflows
    /// </summary>
    public interface IMaterialRequisitionService
    {
        // Basic CRUD Operations
        Task<ApiResponse<MaterialRequisition>> GetByIdAsync(int id);
        Task<ApiResponse<MaterialRequisition>> GetByRequisitionNoAsync(string requisitionNo);
        Task<ApiResponse<IEnumerable<MaterialRequisition>>> GetAllAsync();
        Task<ApiResponse<IEnumerable<MaterialRequisition>>> GetByJobCardIdAsync(int jobCardId);
        Task<ApiResponse<IEnumerable<MaterialRequisition>>> GetByOrderIdAsync(int orderId);
        Task<ApiResponse<IEnumerable<MaterialRequisition>>> GetByStatusAsync(string status);

        // Create, Update, Delete
        Task<ApiResponse<int>> CreateRequisitionAsync(MaterialRequisition requisition);
        Task<ApiResponse<bool>> UpdateRequisitionAsync(MaterialRequisition requisition);
        Task<ApiResponse<bool>> DeleteRequisitionAsync(int id);

        // Approval Workflow
        Task<ApiResponse<bool>> ApproveRequisitionAsync(int id, string approvedBy);
        Task<ApiResponse<bool>> RejectRequisitionAsync(int id, string rejectedBy, string? reason);
        Task<ApiResponse<bool>> UpdateStatusAsync(int id, string status);

        // Material Allocation (FIFO Logic)
        Task<ApiResponse<bool>> AllocateMaterialsAsync(int requisitionId, int materialId, decimal requiredQuantityMM);
        Task<ApiResponse<bool>> DeallocateMaterialsAsync(int requisitionId);

        // Material Issuance
        Task<ApiResponse<int>> IssueMaterialsAsync(int requisitionId, int jobCardId, string issuedBy, string receivedBy);
        Task<ApiResponse<IEnumerable<MaterialPiece>>> GetAllocatedPiecesAsync(int requisitionId);
        Task<ApiResponse<IEnumerable<MaterialIssue>>> GetIssuanceHistoryAsync(int requisitionId);

        // Queries
        Task<ApiResponse<IEnumerable<MaterialRequisition>>> GetPendingRequisitionsAsync();
        Task<ApiResponse<IEnumerable<MaterialRequisition>>> GetApprovedRequisitionsAsync();
        Task<ApiResponse<IEnumerable<MaterialRequisition>>> GetByPriorityAsync(string priority);
        Task<ApiResponse<IEnumerable<MaterialRequisition>>> GetOverdueRequisitionsAsync();
    }
}

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MultiHitechERP.API.Common;
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
        Task<ApiResponse<MaterialRequisition>> GetByIdAsync(Guid id);
        Task<ApiResponse<MaterialRequisition>> GetByRequisitionNoAsync(string requisitionNo);
        Task<ApiResponse<IEnumerable<MaterialRequisition>>> GetAllAsync();
        Task<ApiResponse<IEnumerable<MaterialRequisition>>> GetByJobCardIdAsync(Guid jobCardId);
        Task<ApiResponse<IEnumerable<MaterialRequisition>>> GetByOrderIdAsync(Guid orderId);
        Task<ApiResponse<IEnumerable<MaterialRequisition>>> GetByStatusAsync(string status);

        // Create, Update, Delete
        Task<ApiResponse<Guid>> CreateRequisitionAsync(MaterialRequisition requisition);
        Task<ApiResponse<bool>> UpdateRequisitionAsync(MaterialRequisition requisition);
        Task<ApiResponse<bool>> DeleteRequisitionAsync(Guid id);

        // Approval Workflow
        Task<ApiResponse<bool>> ApproveRequisitionAsync(Guid id, string approvedBy);
        Task<ApiResponse<bool>> RejectRequisitionAsync(Guid id, string rejectedBy, string? reason);
        Task<ApiResponse<bool>> UpdateStatusAsync(Guid id, string status);

        // Material Allocation (FIFO Logic)
        Task<ApiResponse<bool>> AllocateMaterialsAsync(Guid requisitionId, Guid materialId, decimal requiredQuantityMM);
        Task<ApiResponse<bool>> DeallocateMaterialsAsync(Guid requisitionId);

        // Material Issuance
        Task<ApiResponse<Guid>> IssueMaterialsAsync(Guid requisitionId, Guid jobCardId, string issuedBy, string receivedBy);
        Task<ApiResponse<IEnumerable<MaterialPiece>>> GetAllocatedPiecesAsync(Guid requisitionId);
        Task<ApiResponse<IEnumerable<MaterialIssue>>> GetIssuanceHistoryAsync(Guid requisitionId);

        // Queries
        Task<ApiResponse<IEnumerable<MaterialRequisition>>> GetPendingRequisitionsAsync();
        Task<ApiResponse<IEnumerable<MaterialRequisition>>> GetApprovedRequisitionsAsync();
        Task<ApiResponse<IEnumerable<MaterialRequisition>>> GetByPriorityAsync(string priority);
        Task<ApiResponse<IEnumerable<MaterialRequisition>>> GetOverdueRequisitionsAsync();
    }
}

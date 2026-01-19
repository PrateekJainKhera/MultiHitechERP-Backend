using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MultiHitechERP.API.Models.Stores;

namespace MultiHitechERP.API.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface for Material Requisition operations
    /// </summary>
    public interface IMaterialRequisitionRepository
    {
        // Basic CRUD Operations
        Task<MaterialRequisition?> GetByIdAsync(Guid id);
        Task<MaterialRequisition?> GetByRequisitionNoAsync(string requisitionNo);
        Task<IEnumerable<MaterialRequisition>> GetAllAsync();
        Task<IEnumerable<MaterialRequisition>> GetByJobCardIdAsync(Guid jobCardId);
        Task<IEnumerable<MaterialRequisition>> GetByOrderIdAsync(Guid orderId);
        Task<IEnumerable<MaterialRequisition>> GetByStatusAsync(string status);

        // Create, Update, Delete
        Task<Guid> InsertAsync(MaterialRequisition requisition);
        Task<bool> UpdateAsync(MaterialRequisition requisition);
        Task<bool> DeleteAsync(Guid id);

        // Status Operations
        Task<bool> UpdateStatusAsync(Guid id, string status);
        Task<bool> ApproveRequisitionAsync(Guid id, string approvedBy);
        Task<bool> RejectRequisitionAsync(Guid id, string rejectedBy, string? reason);

        // Queries
        Task<IEnumerable<MaterialRequisition>> GetPendingRequisitionsAsync();
        Task<IEnumerable<MaterialRequisition>> GetApprovedRequisitionsAsync();
        Task<IEnumerable<MaterialRequisition>> GetByPriorityAsync(string priority);
        Task<IEnumerable<MaterialRequisition>> GetOverdueRequisitionsAsync();
    }
}

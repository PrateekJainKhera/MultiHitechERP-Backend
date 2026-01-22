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
        Task<MaterialRequisition?> GetByIdAsync(int id);
        Task<MaterialRequisition?> GetByRequisitionNoAsync(string requisitionNo);
        Task<IEnumerable<MaterialRequisition>> GetAllAsync();
        Task<IEnumerable<MaterialRequisition>> GetByJobCardIdAsync(int jobCardId);
        Task<IEnumerable<MaterialRequisition>> GetByOrderIdAsync(int orderId);
        Task<IEnumerable<MaterialRequisition>> GetByStatusAsync(string status);

        // Create, Update, Delete
        Task<int> InsertAsync(MaterialRequisition requisition);
        Task<bool> UpdateAsync(MaterialRequisition requisition);
        Task<bool> DeleteAsync(int id);

        // Status Operations
        Task<bool> UpdateStatusAsync(int id, string status);
        Task<bool> ApproveRequisitionAsync(int id, string approvedBy);
        Task<bool> RejectRequisitionAsync(int id, string rejectedBy, string? reason);

        // Queries
        Task<IEnumerable<MaterialRequisition>> GetPendingRequisitionsAsync();
        Task<IEnumerable<MaterialRequisition>> GetApprovedRequisitionsAsync();
        Task<IEnumerable<MaterialRequisition>> GetByPriorityAsync(string priority);
        Task<IEnumerable<MaterialRequisition>> GetOverdueRequisitionsAsync();
    }
}

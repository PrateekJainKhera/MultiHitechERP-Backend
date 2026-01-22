using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MultiHitechERP.API.Models.Masters;

namespace MultiHitechERP.API.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface for Supplier operations
    /// </summary>
    public interface ISupplierRepository
    {
        // Basic CRUD
        Task<Supplier?> GetByIdAsync(Guid id);
        Task<Supplier?> GetByCodeAsync(string supplierCode);
        Task<IEnumerable<Supplier>> GetAllAsync();
        Task<Guid> InsertAsync(Supplier supplier);
        Task<bool> UpdateAsync(Supplier supplier);
        Task<bool> DeleteAsync(Guid id);

        // Query Methods
        Task<IEnumerable<Supplier>> GetByTypeAsync(string supplierType);
        Task<IEnumerable<Supplier>> GetByCategoryAsync(string category);
        Task<IEnumerable<Supplier>> GetActiveAsync();
        Task<IEnumerable<Supplier>> GetApprovedAsync();
        Task<IEnumerable<Supplier>> GetByStatusAsync(string status);
        Task<IEnumerable<Supplier>> GetByApprovalStatusAsync(string approvalStatus);
        Task<IEnumerable<Supplier>> GetByProcessCapabilityAsync(string processCapability);

        // Performance Tracking
        Task<bool> UpdatePerformanceMetricsAsync(Guid id, decimal onTimeDeliveryRate, decimal qualityRating, int totalOrders, int rejectedOrders);
        Task<IEnumerable<Supplier>> GetTopPerformingAsync(int count);
        Task<IEnumerable<Supplier>> GetLowPerformingAsync(int count);

        // Status Updates
        Task<bool> UpdateStatusAsync(Guid id, string status);
        Task<bool> ApproveSupplierAsync(Guid id, string approvedBy);
        Task<bool> RejectSupplierAsync(Guid id, string reason);
    }
}

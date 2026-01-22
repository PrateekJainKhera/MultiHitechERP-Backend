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
        Task<Supplier?> GetByIdAsync(int id);
        Task<Supplier?> GetByCodeAsync(string supplierCode);
        Task<IEnumerable<Supplier>> GetAllAsync();
        Task<int> InsertAsync(Supplier supplier);
        Task<bool> UpdateAsync(Supplier supplier);
        Task<bool> DeleteAsync(int id);

        // Query Methods
        Task<IEnumerable<Supplier>> GetByTypeAsync(string supplierType);
        Task<IEnumerable<Supplier>> GetByCategoryAsync(string category);
        Task<IEnumerable<Supplier>> GetActiveAsync();
        Task<IEnumerable<Supplier>> GetApprovedAsync();
        Task<IEnumerable<Supplier>> GetByStatusAsync(string status);
        Task<IEnumerable<Supplier>> GetByApprovalStatusAsync(string approvalStatus);
        Task<IEnumerable<Supplier>> GetByProcessCapabilityAsync(string processCapability);

        // Performance Tracking
        Task<bool> UpdatePerformanceMetricsAsync(int id, decimal onTimeDeliveryRate, decimal qualityRating, int totalOrders, int rejectedOrders);
        Task<IEnumerable<Supplier>> GetTopPerformingAsync(int count);
        Task<IEnumerable<Supplier>> GetLowPerformingAsync(int count);

        // Status Updates
        Task<bool> UpdateStatusAsync(int id, string status);
        Task<bool> ApproveSupplierAsync(int id, string approvedBy);
        Task<bool> RejectSupplierAsync(int id, string reason);
    }
}

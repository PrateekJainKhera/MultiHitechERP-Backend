using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MultiHitechERP.API.DTOs.Response;
using MultiHitechERP.API.Models.Masters;

namespace MultiHitechERP.API.Services.Interfaces
{
    /// <summary>
    /// Service interface for Supplier business logic
    /// </summary>
    public interface ISupplierService
    {
        // Basic CRUD
        Task<ApiResponse<Supplier>> GetByIdAsync(Guid id);
        Task<ApiResponse<Supplier>> GetByCodeAsync(string supplierCode);
        Task<ApiResponse<IEnumerable<Supplier>>> GetAllAsync();
        Task<ApiResponse<Guid>> CreateSupplierAsync(Supplier supplier);
        Task<ApiResponse<bool>> UpdateSupplierAsync(Supplier supplier);
        Task<ApiResponse<bool>> DeleteSupplierAsync(Guid id);

        // Query Methods
        Task<ApiResponse<IEnumerable<Supplier>>> GetByTypeAsync(string supplierType);
        Task<ApiResponse<IEnumerable<Supplier>>> GetByCategoryAsync(string category);
        Task<ApiResponse<IEnumerable<Supplier>>> GetActiveAsync();
        Task<ApiResponse<IEnumerable<Supplier>>> GetApprovedAsync();
        Task<ApiResponse<IEnumerable<Supplier>>> GetByStatusAsync(string status);
        Task<ApiResponse<IEnumerable<Supplier>>> GetByApprovalStatusAsync(string approvalStatus);
        Task<ApiResponse<IEnumerable<Supplier>>> GetByProcessCapabilityAsync(string processCapability);

        // Performance Tracking
        Task<ApiResponse<bool>> UpdatePerformanceMetricsAsync(Guid id, decimal onTimeDeliveryRate, decimal qualityRating, int totalOrders, int rejectedOrders);
        Task<ApiResponse<IEnumerable<Supplier>>> GetTopPerformingAsync(int count);
        Task<ApiResponse<IEnumerable<Supplier>>> GetLowPerformingAsync(int count);

        // Approval Workflow
        Task<ApiResponse<bool>> ApproveSupplierAsync(Guid id, string approvedBy);
        Task<ApiResponse<bool>> RejectSupplierAsync(Guid id, string reason);
        Task<ApiResponse<bool>> UpdateStatusAsync(Guid id, string status);
    }
}

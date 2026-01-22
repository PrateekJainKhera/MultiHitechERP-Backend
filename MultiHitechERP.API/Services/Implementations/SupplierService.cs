using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MultiHitechERP.API.DTOs.Response;
using MultiHitechERP.API.Models.Masters;
using MultiHitechERP.API.Repositories.Interfaces;
using MultiHitechERP.API.Services.Interfaces;

namespace MultiHitechERP.API.Services.Implementations
{
    /// <summary>
    /// Service implementation for Supplier business logic
    /// </summary>
    public class SupplierService : ISupplierService
    {
        private readonly ISupplierRepository _supplierRepository;

        public SupplierService(ISupplierRepository supplierRepository)
        {
            _supplierRepository = supplierRepository;
        }

        public async Task<ApiResponse<Supplier>> GetByIdAsync(Guid id)
        {
            var supplier = await _supplierRepository.GetByIdAsync(id);
            if (supplier == null)
                return ApiResponse<Supplier>.ErrorResponse("Supplier not found");

            return ApiResponse<Supplier>.SuccessResponse(supplier);
        }

        public async Task<ApiResponse<Supplier>> GetByCodeAsync(string supplierCode)
        {
            if (string.IsNullOrWhiteSpace(supplierCode))
                return ApiResponse<Supplier>.ErrorResponse("Supplier code is required");

            var supplier = await _supplierRepository.GetByCodeAsync(supplierCode);
            if (supplier == null)
                return ApiResponse<Supplier>.ErrorResponse("Supplier not found");

            return ApiResponse<Supplier>.SuccessResponse(supplier);
        }

        public async Task<ApiResponse<IEnumerable<Supplier>>> GetAllAsync()
        {
            var suppliers = await _supplierRepository.GetAllAsync();
            return ApiResponse<IEnumerable<Supplier>>.SuccessResponse(suppliers);
        }

        public async Task<ApiResponse<Guid>> CreateSupplierAsync(Supplier supplier)
        {
            // Validate required fields
            if (string.IsNullOrWhiteSpace(supplier.SupplierCode))
                return ApiResponse<Guid>.ErrorResponse("Supplier code is required");

            if (string.IsNullOrWhiteSpace(supplier.SupplierName))
                return ApiResponse<Guid>.ErrorResponse("Supplier name is required");

            // Check for duplicate code
            var existing = await _supplierRepository.GetByCodeAsync(supplier.SupplierCode);
            if (existing != null)
                return ApiResponse<Guid>.ErrorResponse($"Supplier with code '{supplier.SupplierCode}' already exists");

            var id = await _supplierRepository.InsertAsync(supplier);
            return ApiResponse<Guid>.SuccessResponse(id, "Supplier created successfully");
        }

        public async Task<ApiResponse<bool>> UpdateSupplierAsync(Supplier supplier)
        {
            var existing = await _supplierRepository.GetByIdAsync(supplier.Id);
            if (existing == null)
                return ApiResponse<bool>.ErrorResponse("Supplier not found");

            // Validate required fields
            if (string.IsNullOrWhiteSpace(supplier.SupplierCode))
                return ApiResponse<bool>.ErrorResponse("Supplier code is required");

            if (string.IsNullOrWhiteSpace(supplier.SupplierName))
                return ApiResponse<bool>.ErrorResponse("Supplier name is required");

            // Check for duplicate code (if code is being changed)
            if (supplier.SupplierCode != existing.SupplierCode)
            {
                var duplicate = await _supplierRepository.GetByCodeAsync(supplier.SupplierCode);
                if (duplicate != null)
                    return ApiResponse<bool>.ErrorResponse($"Supplier with code '{supplier.SupplierCode}' already exists");
            }

            var success = await _supplierRepository.UpdateAsync(supplier);
            if (!success)
                return ApiResponse<bool>.ErrorResponse("Failed to update supplier");

            return ApiResponse<bool>.SuccessResponse(true, "Supplier updated successfully");
        }

        public async Task<ApiResponse<bool>> DeleteSupplierAsync(Guid id)
        {
            var existing = await _supplierRepository.GetByIdAsync(id);
            if (existing == null)
                return ApiResponse<bool>.ErrorResponse("Supplier not found");

            var success = await _supplierRepository.DeleteAsync(id);
            if (!success)
                return ApiResponse<bool>.ErrorResponse("Failed to delete supplier");

            return ApiResponse<bool>.SuccessResponse(true, "Supplier deleted successfully");
        }

        public async Task<ApiResponse<IEnumerable<Supplier>>> GetByTypeAsync(string supplierType)
        {
            if (string.IsNullOrWhiteSpace(supplierType))
                return ApiResponse<IEnumerable<Supplier>>.ErrorResponse("Supplier type is required");

            var suppliers = await _supplierRepository.GetByTypeAsync(supplierType);
            return ApiResponse<IEnumerable<Supplier>>.SuccessResponse(suppliers);
        }

        public async Task<ApiResponse<IEnumerable<Supplier>>> GetByCategoryAsync(string category)
        {
            if (string.IsNullOrWhiteSpace(category))
                return ApiResponse<IEnumerable<Supplier>>.ErrorResponse("Category is required");

            var suppliers = await _supplierRepository.GetByCategoryAsync(category);
            return ApiResponse<IEnumerable<Supplier>>.SuccessResponse(suppliers);
        }

        public async Task<ApiResponse<IEnumerable<Supplier>>> GetActiveAsync()
        {
            var suppliers = await _supplierRepository.GetActiveAsync();
            return ApiResponse<IEnumerable<Supplier>>.SuccessResponse(suppliers);
        }

        public async Task<ApiResponse<IEnumerable<Supplier>>> GetApprovedAsync()
        {
            var suppliers = await _supplierRepository.GetApprovedAsync();
            return ApiResponse<IEnumerable<Supplier>>.SuccessResponse(suppliers);
        }

        public async Task<ApiResponse<IEnumerable<Supplier>>> GetByStatusAsync(string status)
        {
            if (string.IsNullOrWhiteSpace(status))
                return ApiResponse<IEnumerable<Supplier>>.ErrorResponse("Status is required");

            var suppliers = await _supplierRepository.GetByStatusAsync(status);
            return ApiResponse<IEnumerable<Supplier>>.SuccessResponse(suppliers);
        }

        public async Task<ApiResponse<IEnumerable<Supplier>>> GetByApprovalStatusAsync(string approvalStatus)
        {
            if (string.IsNullOrWhiteSpace(approvalStatus))
                return ApiResponse<IEnumerable<Supplier>>.ErrorResponse("Approval status is required");

            var suppliers = await _supplierRepository.GetByApprovalStatusAsync(approvalStatus);
            return ApiResponse<IEnumerable<Supplier>>.SuccessResponse(suppliers);
        }

        public async Task<ApiResponse<IEnumerable<Supplier>>> GetByProcessCapabilityAsync(string processCapability)
        {
            if (string.IsNullOrWhiteSpace(processCapability))
                return ApiResponse<IEnumerable<Supplier>>.ErrorResponse("Process capability is required");

            var suppliers = await _supplierRepository.GetByProcessCapabilityAsync(processCapability);
            return ApiResponse<IEnumerable<Supplier>>.SuccessResponse(suppliers);
        }

        public async Task<ApiResponse<bool>> UpdatePerformanceMetricsAsync(
            Guid id,
            decimal onTimeDeliveryRate,
            decimal qualityRating,
            int totalOrders,
            int rejectedOrders)
        {
            var supplier = await _supplierRepository.GetByIdAsync(id);
            if (supplier == null)
                return ApiResponse<bool>.ErrorResponse("Supplier not found");

            // Validate metrics
            if (onTimeDeliveryRate < 0 || onTimeDeliveryRate > 100)
                return ApiResponse<bool>.ErrorResponse("On-time delivery rate must be between 0 and 100");

            if (qualityRating < 0 || qualityRating > 5)
                return ApiResponse<bool>.ErrorResponse("Quality rating must be between 0 and 5");

            if (totalOrders < 0 || rejectedOrders < 0)
                return ApiResponse<bool>.ErrorResponse("Order counts cannot be negative");

            if (rejectedOrders > totalOrders)
                return ApiResponse<bool>.ErrorResponse("Rejected orders cannot exceed total orders");

            var success = await _supplierRepository.UpdatePerformanceMetricsAsync(
                id, onTimeDeliveryRate, qualityRating, totalOrders, rejectedOrders);

            if (!success)
                return ApiResponse<bool>.ErrorResponse("Failed to update performance metrics");

            return ApiResponse<bool>.SuccessResponse(true, "Performance metrics updated successfully");
        }

        public async Task<ApiResponse<IEnumerable<Supplier>>> GetTopPerformingAsync(int count)
        {
            if (count <= 0 || count > 100)
                return ApiResponse<IEnumerable<Supplier>>.ErrorResponse("Count must be between 1 and 100");

            var suppliers = await _supplierRepository.GetTopPerformingAsync(count);
            return ApiResponse<IEnumerable<Supplier>>.SuccessResponse(suppliers);
        }

        public async Task<ApiResponse<IEnumerable<Supplier>>> GetLowPerformingAsync(int count)
        {
            if (count <= 0 || count > 100)
                return ApiResponse<IEnumerable<Supplier>>.ErrorResponse("Count must be between 1 and 100");

            var suppliers = await _supplierRepository.GetLowPerformingAsync(count);
            return ApiResponse<IEnumerable<Supplier>>.SuccessResponse(suppliers);
        }

        public async Task<ApiResponse<bool>> ApproveSupplierAsync(Guid id, string approvedBy)
        {
            var supplier = await _supplierRepository.GetByIdAsync(id);
            if (supplier == null)
                return ApiResponse<bool>.ErrorResponse("Supplier not found");

            if (supplier.IsApproved)
                return ApiResponse<bool>.ErrorResponse("Supplier is already approved");

            if (string.IsNullOrWhiteSpace(approvedBy))
                return ApiResponse<bool>.ErrorResponse("Approver name is required");

            var success = await _supplierRepository.ApproveSupplierAsync(id, approvedBy);
            if (!success)
                return ApiResponse<bool>.ErrorResponse("Failed to approve supplier");

            return ApiResponse<bool>.SuccessResponse(true, "Supplier approved successfully");
        }

        public async Task<ApiResponse<bool>> RejectSupplierAsync(Guid id, string reason)
        {
            var supplier = await _supplierRepository.GetByIdAsync(id);
            if (supplier == null)
                return ApiResponse<bool>.ErrorResponse("Supplier not found");

            if (string.IsNullOrWhiteSpace(reason))
                return ApiResponse<bool>.ErrorResponse("Rejection reason is required");

            var success = await _supplierRepository.RejectSupplierAsync(id, reason);
            if (!success)
                return ApiResponse<bool>.ErrorResponse("Failed to reject supplier");

            return ApiResponse<bool>.SuccessResponse(true, "Supplier rejected successfully");
        }

        public async Task<ApiResponse<bool>> UpdateStatusAsync(Guid id, string status)
        {
            var supplier = await _supplierRepository.GetByIdAsync(id);
            if (supplier == null)
                return ApiResponse<bool>.ErrorResponse("Supplier not found");

            if (string.IsNullOrWhiteSpace(status))
                return ApiResponse<bool>.ErrorResponse("Status is required");

            var success = await _supplierRepository.UpdateStatusAsync(id, status);
            if (!success)
                return ApiResponse<bool>.ErrorResponse("Failed to update supplier status");

            return ApiResponse<bool>.SuccessResponse(true, "Supplier status updated successfully");
        }
    }
}

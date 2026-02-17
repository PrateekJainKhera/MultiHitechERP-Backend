using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MultiHitechERP.API.DTOs.Request;
using MultiHitechERP.API.DTOs.Response;
using MultiHitechERP.API.Models.Masters;
using MultiHitechERP.API.Repositories.Interfaces;
using MultiHitechERP.API.Services.Interfaces;

namespace MultiHitechERP.API.Services.Implementations
{
    public class WarehouseService : IWarehouseService
    {
        private readonly IWarehouseRepository _repository;

        public WarehouseService(IWarehouseRepository repository)
        {
            _repository = repository;
        }

        public async Task<ApiResponse<IEnumerable<WarehouseResponse>>> GetAllAsync()
        {
            try
            {
                var warehouses = await _repository.GetAllAsync();
                return ApiResponse<IEnumerable<WarehouseResponse>>.SuccessResponse(
                    warehouses.Select(MapToResponse));
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<WarehouseResponse>>.ErrorResponse(
                    $"Error retrieving warehouses: {ex.Message}");
            }
        }

        public async Task<ApiResponse<WarehouseResponse>> GetByIdAsync(int id)
        {
            try
            {
                var warehouse = await _repository.GetByIdAsync(id);
                if (warehouse == null)
                    return ApiResponse<WarehouseResponse>.ErrorResponse($"Warehouse with ID {id} not found");
                return ApiResponse<WarehouseResponse>.SuccessResponse(MapToResponse(warehouse));
            }
            catch (Exception ex)
            {
                return ApiResponse<WarehouseResponse>.ErrorResponse(
                    $"Error retrieving warehouse: {ex.Message}");
            }
        }

        public async Task<ApiResponse<int>> CreateAsync(CreateWarehouseRequest request)
        {
            try
            {
                if (!IsValidMaterialType(request.MaterialType))
                    return ApiResponse<int>.ErrorResponse(
                        "MaterialType must be 'RawMaterial' or 'Component'");

                var warehouse = new Warehouse
                {
                    Name         = request.Name.Trim(),
                    Rack         = request.Rack.Trim(),
                    RackNo       = request.RackNo.Trim(),
                    MaterialType = request.MaterialType,
                    MinStockPieces   = request.MinStockPieces,
                    MinStockLengthMM = request.MinStockLengthMM,
                    IsActive     = true,
                    CreatedBy    = request.CreatedBy
                };

                var id = await _repository.CreateAsync(warehouse);
                return ApiResponse<int>.SuccessResponse(id, "Warehouse created successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<int>.ErrorResponse($"Error creating warehouse: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> UpdateAsync(UpdateWarehouseRequest request)
        {
            try
            {
                var existing = await _repository.GetByIdAsync(request.Id);
                if (existing == null)
                    return ApiResponse<bool>.ErrorResponse($"Warehouse with ID {request.Id} not found");

                if (!IsValidMaterialType(request.MaterialType))
                    return ApiResponse<bool>.ErrorResponse(
                        "MaterialType must be 'RawMaterial' or 'Component'");

                existing.Name         = request.Name.Trim();
                existing.Rack         = request.Rack.Trim();
                existing.RackNo       = request.RackNo.Trim();
                existing.MaterialType = request.MaterialType;
                existing.MinStockPieces   = request.MinStockPieces;
                existing.MinStockLengthMM = request.MinStockLengthMM;
                existing.IsActive     = request.IsActive;
                existing.UpdatedBy    = request.UpdatedBy;

                var success = await _repository.UpdateAsync(existing);
                return success
                    ? ApiResponse<bool>.SuccessResponse(true, "Warehouse updated successfully")
                    : ApiResponse<bool>.ErrorResponse("Failed to update warehouse");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error updating warehouse: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> DeleteAsync(int id)
        {
            try
            {
                var existing = await _repository.GetByIdAsync(id);
                if (existing == null)
                    return ApiResponse<bool>.ErrorResponse($"Warehouse with ID {id} not found");

                var success = await _repository.DeleteAsync(id);
                return success
                    ? ApiResponse<bool>.SuccessResponse(true, "Warehouse deleted successfully")
                    : ApiResponse<bool>.ErrorResponse("Failed to delete warehouse");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error deleting warehouse: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<LowStockAlertResponse>>> GetLowStockStatusAsync()
        {
            try
            {
                var alerts = await _repository.GetLowStockStatusAsync();
                return ApiResponse<IEnumerable<LowStockAlertResponse>>.SuccessResponse(alerts);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<LowStockAlertResponse>>.ErrorResponse(
                    $"Error retrieving low stock status: {ex.Message}");
            }
        }

        private static bool IsValidMaterialType(string materialType) =>
            materialType == "RawMaterial" || materialType == "Component";

        private static WarehouseResponse MapToResponse(Warehouse w) => new()
        {
            Id           = w.Id,
            Name         = w.Name,
            Rack         = w.Rack,
            RackNo       = w.RackNo,
            MaterialType     = w.MaterialType,
            MinStockPieces   = w.MinStockPieces,
            MinStockLengthMM = w.MinStockLengthMM,
            IsActive         = w.IsActive,
            CreatedAt    = w.CreatedAt,
            CreatedBy    = w.CreatedBy,
            UpdatedAt    = w.UpdatedAt,
            UpdatedBy    = w.UpdatedBy,
        };
    }
}

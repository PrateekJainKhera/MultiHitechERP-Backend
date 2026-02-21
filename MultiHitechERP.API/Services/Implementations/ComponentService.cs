using MultiHitechERP.API.DTOs.Request;
using MultiHitechERP.API.DTOs.Response;
using MultiHitechERP.API.Models;
using MultiHitechERP.API.Models.Masters;
using MultiHitechERP.API.Repositories.Interfaces;
using MultiHitechERP.API.Services.Interfaces;

namespace MultiHitechERP.API.Services.Implementations
{
    public class ComponentService : IComponentService
    {
        private readonly IComponentRepository _componentRepository;

        public ComponentService(IComponentRepository componentRepository)
        {
            _componentRepository = componentRepository;
        }

        public async Task<ApiResponse<IEnumerable<ComponentResponse>>> GetAllComponentsAsync()
        {
            try
            {
                var components = await _componentRepository.GetAllAsync();
                var response = components.Select(MapToResponse);
                return ApiResponse<IEnumerable<ComponentResponse>>.SuccessResponse(response, "Components retrieved successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<ComponentResponse>>.ErrorResponse($"Error retrieving components: {ex.Message}");
            }
        }

        public async Task<ApiResponse<ComponentResponse>> GetComponentByIdAsync(int id)
        {
            try
            {
                var component = await _componentRepository.GetByIdAsync(id);
                if (component == null)
                    return ApiResponse<ComponentResponse>.ErrorResponse("Component not found");

                return ApiResponse<ComponentResponse>.SuccessResponse(MapToResponse(component), "Component retrieved successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<ComponentResponse>.ErrorResponse($"Error retrieving component: {ex.Message}");
            }
        }

        public async Task<ApiResponse<ComponentResponse>> GetComponentByPartNumberAsync(string partNumber)
        {
            try
            {
                var component = await _componentRepository.GetByPartNumberAsync(partNumber);
                if (component == null)
                    return ApiResponse<ComponentResponse>.ErrorResponse("Component not found");

                return ApiResponse<ComponentResponse>.SuccessResponse(MapToResponse(component), "Component retrieved successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<ComponentResponse>.ErrorResponse($"Error retrieving component: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<ComponentResponse>>> GetComponentsByCategoryAsync(string category)
        {
            try
            {
                // Validate category
                var validCategories = new[] { "Bearing", "Gear", "Seal", "Coupling", "Shaft", "Bushing", "Fastener", "Other" };
                if (!validCategories.Contains(category))
                    return ApiResponse<IEnumerable<ComponentResponse>>.ErrorResponse($"Invalid category. Must be one of: {string.Join(", ", validCategories)}");

                var components = await _componentRepository.GetByCategoryAsync(category);
                var response = components.Select(MapToResponse);
                return ApiResponse<IEnumerable<ComponentResponse>>.SuccessResponse(response, "Components retrieved successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<ComponentResponse>>.ErrorResponse($"Error retrieving components: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<ComponentResponse>>> SearchComponentsByNameAsync(string searchTerm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                    return ApiResponse<IEnumerable<ComponentResponse>>.ErrorResponse("Search term cannot be empty");

                var components = await _componentRepository.SearchByNameAsync(searchTerm);
                var response = components.Select(MapToResponse);
                return ApiResponse<IEnumerable<ComponentResponse>>.SuccessResponse(response, "Components retrieved successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<ComponentResponse>>.ErrorResponse($"Error searching components: {ex.Message}");
            }
        }

        public async Task<ApiResponse<int>> CreateComponentAsync(CreateComponentRequest request)
        {
            try
            {
                // Validate Category
                var validCategories = new[] { "Bearing", "Gear", "Seal", "Coupling", "Shaft", "Bushing", "Fastener", "Other" };
                if (!validCategories.Contains(request.Category))
                    return ApiResponse<int>.ErrorResponse($"Invalid category. Must be one of: {string.Join(", ", validCategories)}");

                // Auto-generate PartNumber
                // Format: CATEGORY-SEQ (e.g., BRG-0001, SFT-0002)
                string prefix = request.Category.Length >= 3 ? request.Category.Substring(0, 3).ToUpper() : request.Category.ToUpper();
                int sequence = await _componentRepository.GetNextSequenceNumberAsync(request.Category);
                string partNumber = $"{prefix}-{sequence:D4}";

                var component = new Component
                {
                    PartNumber = partNumber,
                    ComponentName = request.ComponentName,
                    Category = request.Category,
                    Manufacturer = request.Manufacturer,
                    SupplierName = request.SupplierName,
                    Specifications = request.Specifications,
                    LeadTimeDays = request.LeadTimeDays ?? 0,
                    Unit = request.Unit,
                    Notes = request.Notes,
                    MinimumStock = request.MinimumStock,
                    IsActive = request.IsActive,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = request.CreatedBy?.Trim() ?? "System",
                    UpdatedAt = DateTime.UtcNow
                };

                var id = await _componentRepository.CreateAsync(component);
                return ApiResponse<int>.SuccessResponse(id, $"Component '{partNumber}' created successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<int>.ErrorResponse($"Error creating component: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> UpdateComponentAsync(int id, UpdateComponentRequest request)
        {
            try
            {
                // Check if component exists
                var existingComponent = await _componentRepository.GetByIdAsync(id);
                if (existingComponent == null)
                    return ApiResponse<bool>.ErrorResponse("Component not found");

                // Validate Category
                var validCategories = new[] { "Bearing", "Gear", "Seal", "Coupling", "Shaft", "Bushing", "Fastener", "Other" };
                if (!validCategories.Contains(request.Category))
                    return ApiResponse<bool>.ErrorResponse($"Invalid category. Must be one of: {string.Join(", ", validCategories)}");

                // Check for duplicate part number (excluding current component)
                if (await _componentRepository.PartNumberExistsAsync(request.PartNumber, id))
                    return ApiResponse<bool>.ErrorResponse($"Part number '{request.PartNumber}' already exists");

                var component = new Component
                {
                    Id = id,
                    PartNumber = request.PartNumber,
                    ComponentName = request.ComponentName,
                    Category = request.Category,
                    Manufacturer = request.Manufacturer,
                    SupplierName = request.SupplierName,
                    Specifications = request.Specifications,
                    LeadTimeDays = request.LeadTimeDays ?? 0,
                    Unit = request.Unit,
                    Notes = request.Notes,
                    MinimumStock = request.MinimumStock,
                    CreatedAt = existingComponent.CreatedAt,
                    UpdatedAt = DateTime.UtcNow
                };

                var result = await _componentRepository.UpdateAsync(id, component);
                if (result)
                    return ApiResponse<bool>.SuccessResponse(true, "Component updated successfully");

                return ApiResponse<bool>.ErrorResponse("Failed to update component");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error updating component: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> DeleteComponentAsync(int id)
        {
            try
            {
                var component = await _componentRepository.GetByIdAsync(id);
                if (component == null)
                    return ApiResponse<bool>.ErrorResponse("Component not found");

                var result = await _componentRepository.DeleteAsync(id);
                if (result)
                    return ApiResponse<bool>.SuccessResponse(true, "Component deleted successfully");

                return ApiResponse<bool>.ErrorResponse("Failed to delete component");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error deleting component: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<ComponentLowStockResponse>>> GetLowStockComponentsAsync()
        {
            try
            {
                var items = await _componentRepository.GetLowStockAsync();
                return ApiResponse<IEnumerable<ComponentLowStockResponse>>.SuccessResponse(items);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<ComponentLowStockResponse>>.ErrorResponse($"Error: {ex.Message}");
            }
        }

        private ComponentResponse MapToResponse(Component component)
        {
            return new ComponentResponse
            {
                Id = component.Id,
                PartNumber = component.PartNumber,
                ComponentName = component.ComponentName,
                Category = component.Category,
                Manufacturer = component.Manufacturer,
                SupplierName = component.SupplierName,
                Specifications = component.Specifications,
                LeadTimeDays = component.LeadTimeDays,
                Unit = component.Unit,
                Notes = component.Notes,
                MinimumStock = component.MinimumStock,
                IsActive = component.IsActive,
                CreatedAt = component.CreatedAt,
                CreatedBy = component.CreatedBy,
                UpdatedAt = component.UpdatedAt,
                UpdatedBy = component.UpdatedBy
            };
        }
    }
}

using System;
using System.Linq;
using System.Threading.Tasks;
using MultiHitechERP.API.DTOs.Request;
using MultiHitechERP.API.DTOs.Response;
using MultiHitechERP.API.Models.Masters;
using MultiHitechERP.API.Repositories.Interfaces;
using MultiHitechERP.API.Services.Interfaces;

namespace MultiHitechERP.API.Services.Implementations
{
    public class MachineModelService : IMachineModelService
    {
        private readonly IMachineModelRepository _repository;

        public MachineModelService(IMachineModelRepository repository)
        {
            _repository = repository;
        }

        public async Task<ApiResponse<MachineModelResponse[]>> GetAllAsync()
        {
            try
            {
                var models = await _repository.GetAllAsync();
                var response = new List<MachineModelResponse>();

                foreach (var model in models)
                {
                    var productCount = await _repository.GetProductCountAsync(model.Id);
                    response.Add(MapToResponse(model, productCount));
                }

                return ApiResponse<MachineModelResponse[]>.SuccessResponse(response.ToArray(), "Models retrieved successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<MachineModelResponse[]>.ErrorResponse($"Error retrieving models: {ex.Message}");
            }
        }

        public async Task<ApiResponse<MachineModelResponse>> GetByIdAsync(int id)
        {
            try
            {
                var model = await _repository.GetByIdAsync(id);
                if (model == null)
                {
                    return ApiResponse<MachineModelResponse>.ErrorResponse("Model not found");
                }

                var productCount = await _repository.GetProductCountAsync(model.Id);
                return ApiResponse<MachineModelResponse>.SuccessResponse(MapToResponse(model, productCount));
            }
            catch (Exception ex)
            {
                return ApiResponse<MachineModelResponse>.ErrorResponse($"Error retrieving model: {ex.Message}");
            }
        }

        public async Task<ApiResponse<int>> CreateAsync(CreateMachineModelRequest request)
        {
            try
            {
                // Check if model name already exists
                var exists = await _repository.ExistsByNameAsync(request.ModelName);
                if (exists)
                {
                    return ApiResponse<int>.ErrorResponse("A model with this name already exists");
                }

                var model = new MachineModel
                {
                    ModelName = request.ModelName,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = request.CreatedBy,
                    UpdatedAt = DateTime.UtcNow,
                    IsActive = true
                };

                var id = await _repository.CreateAsync(model);
                return ApiResponse<int>.SuccessResponse(id, "Model created successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<int>.ErrorResponse($"Error creating model: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> UpdateAsync(UpdateMachineModelRequest request)
        {
            try
            {
                // Check if model exists
                var existingModel = await _repository.GetByIdAsync(request.Id);
                if (existingModel == null)
                {
                    return ApiResponse<bool>.ErrorResponse("Model not found");
                }

                // Check if new name conflicts with another model
                var modelWithSameName = await _repository.GetByNameAsync(request.ModelName);
                if (modelWithSameName != null && modelWithSameName.Id != request.Id)
                {
                    return ApiResponse<bool>.ErrorResponse("Another model with this name already exists");
                }

                var model = new MachineModel
                {
                    Id = request.Id,
                    ModelName = request.ModelName,
                    CreatedAt = existingModel.CreatedAt,
                    CreatedBy = existingModel.CreatedBy,
                    UpdatedAt = DateTime.UtcNow,
                    IsActive = request.IsActive
                };

                var success = await _repository.UpdateAsync(model);
                return success
                    ? ApiResponse<bool>.SuccessResponse(true, "Model updated successfully")
                    : ApiResponse<bool>.ErrorResponse("Failed to update model");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error updating model: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> DeleteAsync(int id)
        {
            try
            {
                // Check if model exists
                var model = await _repository.GetByIdAsync(id);
                if (model == null)
                {
                    return ApiResponse<bool>.ErrorResponse("Model not found");
                }

                // Check if model is being used by products
                var productCount = await _repository.GetProductCountAsync(id);
                if (productCount > 0)
                {
                    return ApiResponse<bool>.ErrorResponse($"Cannot delete model. It is being used by {productCount} product(s)");
                }

                var success = await _repository.DeleteAsync(id);
                return success
                    ? ApiResponse<bool>.SuccessResponse(true, "Model deleted successfully")
                    : ApiResponse<bool>.ErrorResponse("Failed to delete model");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error deleting model: {ex.Message}");
            }
        }

        private static MachineModelResponse MapToResponse(MachineModel model, int productCount)
        {
            return new MachineModelResponse
            {
                Id = model.Id,
                ModelName = model.ModelName,
                CreatedAt = model.CreatedAt,
                CreatedBy = model.CreatedBy,
                UpdatedAt = model.UpdatedAt,
                IsActive = model.IsActive,
                ProductCount = productCount
            };
        }
    }
}

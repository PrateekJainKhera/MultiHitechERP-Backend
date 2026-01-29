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
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<ApiResponse<ProductResponse>> GetByIdAsync(int id)
        {
            try
            {
                var product = await _productRepository.GetByIdAsync(id);
                if (product == null)
                    return ApiResponse<ProductResponse>.ErrorResponse($"Product with ID {id} not found");

                return ApiResponse<ProductResponse>.SuccessResponse(MapToResponse(product));
            }
            catch (Exception ex)
            {
                return ApiResponse<ProductResponse>.ErrorResponse($"Error retrieving product: {ex.Message}");
            }
        }

        public async Task<ApiResponse<ProductResponse>> GetByPartCodeAsync(string partCode)
        {
            try
            {
                var product = await _productRepository.GetByPartCodeAsync(partCode);
                if (product == null)
                    return ApiResponse<ProductResponse>.ErrorResponse($"Product with part code '{partCode}' not found");

                return ApiResponse<ProductResponse>.SuccessResponse(MapToResponse(product));
            }
            catch (Exception ex)
            {
                return ApiResponse<ProductResponse>.ErrorResponse($"Error retrieving product: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<ProductResponse>>> GetAllAsync()
        {
            try
            {
                var products = await _productRepository.GetAllAsync();
                var responses = products.Select(MapToResponse).ToList();
                return ApiResponse<IEnumerable<ProductResponse>>.SuccessResponse(responses);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<ProductResponse>>.ErrorResponse($"Error retrieving products: {ex.Message}");
            }
        }

        public async Task<ApiResponse<int>> CreateProductAsync(CreateProductRequest request)
        {
            try
            {
                // Business validation: Validate required fields
                if (string.IsNullOrWhiteSpace(request.ModelName))
                    return ApiResponse<int>.ErrorResponse("Model name is required");

                // Business validation: Only Printing Roller or Magnetic Roller allowed
                var validRollerTypes = new[] { "Printing Roller", "Magnetic Roller" };
                if (!validRollerTypes.Contains(request.RollerType))
                    return ApiResponse<int>.ErrorResponse("Roller type must be either 'Printing Roller' or 'Magnetic Roller'");

                // Business validation: Validate dimensions
                if (request.Diameter <= 0)
                    return ApiResponse<int>.ErrorResponse("Diameter must be greater than 0");

                if (request.Length <= 0)
                    return ApiResponse<int>.ErrorResponse("Length must be greater than 0");

                // Auto-generate PartCode based on roller type
                string partCodePrefix = request.RollerType == "Magnetic Roller" ? "MAG" : "PRT";
                int nextSequence = await _productRepository.GetNextSequenceNumberAsync(request.RollerType);
                string generatedPartCode = $"{partCodePrefix}-{nextSequence:D4}";

                // Create product entity
                var product = new Product
                {
                    PartCode = generatedPartCode,
                    CustomerName = request.CustomerName?.Trim(),
                    ModelName = request.ModelName.Trim(),
                    RollerType = request.RollerType.Trim(),
                    Diameter = request.Diameter,
                    Length = request.Length,
                    MaterialGrade = request.MaterialGrade?.Trim(),
                    SurfaceFinish = request.SurfaceFinish?.Trim(),
                    Hardness = request.Hardness?.Trim(),
                    DrawingNo = request.DrawingNo?.Trim(),
                    RevisionNo = request.RevisionNo?.Trim(),
                    RevisionDate = request.RevisionDate?.Trim(),
                    NumberOfTeeth = request.NumberOfTeeth,
                    ProcessTemplateId = request.ProcessTemplateId,
                    CreatedBy = request.CreatedBy?.Trim() ?? "System"
                };

                var productId = await _productRepository.InsertAsync(product);
                return ApiResponse<int>.SuccessResponse(productId, $"Product '{generatedPartCode}' created successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<int>.ErrorResponse($"Error creating product: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> UpdateProductAsync(UpdateProductRequest request)
        {
            try
            {
                // Verify product exists
                var existingProduct = await _productRepository.GetByIdAsync(request.Id);
                if (existingProduct == null)
                    return ApiResponse<bool>.ErrorResponse($"Product with ID {request.Id} not found");

                // Business validation: Only Printing Roller or Magnetic Roller allowed
                var validRollerTypes = new[] { "Printing Roller", "Magnetic Roller" };
                if (!validRollerTypes.Contains(request.RollerType))
                    return ApiResponse<bool>.ErrorResponse("Roller type must be either 'Printing Roller' or 'Magnetic Roller'");

                // Business validation: Check if part code already exists (if changed)
                if (existingProduct.PartCode != request.PartCode)
                {
                    var exists = await _productRepository.ExistsAsync(request.PartCode);
                    if (exists)
                        return ApiResponse<bool>.ErrorResponse($"Product with part code '{request.PartCode}' already exists");
                }

                // Business validation: Validate dimensions
                if (request.Diameter <= 0)
                    return ApiResponse<bool>.ErrorResponse("Diameter must be greater than 0");

                if (request.Length <= 0)
                    return ApiResponse<bool>.ErrorResponse("Length must be greater than 0");

                // Update product entity
                existingProduct.PartCode = request.PartCode.Trim().ToUpper();
                existingProduct.CustomerName = request.CustomerName?.Trim();
                existingProduct.ModelName = request.ModelName.Trim();
                existingProduct.RollerType = request.RollerType.Trim();
                existingProduct.Diameter = request.Diameter;
                existingProduct.Length = request.Length;
                existingProduct.MaterialGrade = request.MaterialGrade?.Trim();
                existingProduct.SurfaceFinish = request.SurfaceFinish?.Trim();
                existingProduct.Hardness = request.Hardness?.Trim();
                existingProduct.DrawingNo = request.DrawingNo?.Trim();
                existingProduct.RevisionNo = request.RevisionNo?.Trim();
                existingProduct.RevisionDate = request.RevisionDate?.Trim();
                existingProduct.NumberOfTeeth = request.NumberOfTeeth;
                existingProduct.ProcessTemplateId = request.ProcessTemplateId;

                var success = await _productRepository.UpdateAsync(existingProduct);
                if (!success)
                    return ApiResponse<bool>.ErrorResponse("Failed to update product");

                return ApiResponse<bool>.SuccessResponse(true, "Product updated successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error updating product: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> DeleteProductAsync(int id)
        {
            try
            {
                var product = await _productRepository.GetByIdAsync(id);
                if (product == null)
                    return ApiResponse<bool>.ErrorResponse($"Product with ID {id} not found");

                var success = await _productRepository.DeleteAsync(id);
                if (!success)
                    return ApiResponse<bool>.ErrorResponse("Failed to delete product");

                return ApiResponse<bool>.SuccessResponse(true, "Product deleted successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error deleting product: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<ProductResponse>>> SearchByNameAsync(string searchTerm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                    return ApiResponse<IEnumerable<ProductResponse>>.ErrorResponse("Search term is required");

                var products = await _productRepository.SearchByNameAsync(searchTerm);
                var responses = products.Select(MapToResponse).ToList();
                return ApiResponse<IEnumerable<ProductResponse>>.SuccessResponse(responses);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<ProductResponse>>.ErrorResponse($"Error searching products: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<ProductResponse>>> GetByRollerTypeAsync(string rollerType)
        {
            try
            {
                var products = await _productRepository.GetByRollerTypeAsync(rollerType);
                var responses = products.Select(MapToResponse).ToList();
                return ApiResponse<IEnumerable<ProductResponse>>.SuccessResponse(responses);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<ProductResponse>>.ErrorResponse($"Error retrieving products by roller type: {ex.Message}");
            }
        }

        private static ProductResponse MapToResponse(Product product)
        {
            return new ProductResponse
            {
                Id = product.Id,
                PartCode = product.PartCode,
                CustomerName = product.CustomerName,
                ModelName = product.ModelName,
                RollerType = product.RollerType,
                Diameter = product.Diameter,
                Length = product.Length,
                MaterialGrade = product.MaterialGrade,
                SurfaceFinish = product.SurfaceFinish,
                Hardness = product.Hardness,
                DrawingNo = product.DrawingNo,
                RevisionNo = product.RevisionNo,
                RevisionDate = product.RevisionDate,
                NumberOfTeeth = product.NumberOfTeeth,
                ProcessTemplateId = product.ProcessTemplateId,
                CreatedAt = product.CreatedAt,
                UpdatedAt = product.UpdatedAt,
                CreatedBy = product.CreatedBy
            };
        }
    }
}

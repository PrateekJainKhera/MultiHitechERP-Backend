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

        public async Task<ApiResponse<ProductResponse>> GetByPartCodeAsync(string productCode)
        {
            try
            {
                var product = await _productRepository.GetByPartCodeAsync(productCode);
                if (product == null)
                    return ApiResponse<ProductResponse>.ErrorResponse($"Product {productCode} not found");

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

        public async Task<ApiResponse<IEnumerable<ProductResponse>>> GetActiveProductsAsync()
        {
            try
            {
                var products = await _productRepository.GetActiveProductsAsync();
                var responses = products.Select(MapToResponse).ToList();
                return ApiResponse<IEnumerable<ProductResponse>>.SuccessResponse(responses);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<ProductResponse>>.ErrorResponse($"Error retrieving active products: {ex.Message}");
            }
        }

        public async Task<ApiResponse<int>> CreateProductAsync(CreateProductRequest request)
        {
            try
            {
                var exists = await _productRepository.ExistsAsync(request.PartCode);
                if (exists)
                    return ApiResponse<int>.ErrorResponse($"Product code '{request.PartCode}' already exists");

                if (string.IsNullOrWhiteSpace(request.ModelName))
                    return ApiResponse<int>.ErrorResponse("Product name is required");

                // Validate RollerType - Only Printing Roller or Magnetic Roller allowed
                var validRollerTypes = new[] { "Printing Roller", "Magnetic Roller" };
                if (!validRollerTypes.Contains(request.RollerType))
                    return ApiResponse<int>.ErrorResponse("Roller type must be either 'Printing Roller' or 'Magnetic Roller'");

                if (!string.IsNullOrWhiteSpace(request.HSNCode) && (request.HSNCode.Length < 4 || request.HSNCode.Length > 8))
                    return ApiResponse<int>.ErrorResponse("HSN Code must be between 4 and 8 digits");

                var product = new Product
                {
                    PartCode = request.PartCode.Trim().ToUpper(),
                    CustomerId = request.CustomerId,
                    CustomerName = request.CustomerName?.Trim(),
                    ModelName = request.ModelName.Trim(),
                    RollerType = request.RollerType.Trim(),
                    Diameter = request.Diameter,
                    Length = request.Length,
                    Weight = request.Weight,
                    MaterialGrade = request.MaterialGrade?.Trim(),
                    SurfaceFinish = request.SurfaceFinish?.Trim(),
                    Hardness = request.Hardness?.Trim(),
                    DrawingNo = request.DrawingNo?.Trim(),
                    RevisionNo = request.RevisionNo?.Trim(),
                    DrawingId = request.DrawingId,
                    ProcessTemplateId = request.ProcessTemplateId,
                    ProductTemplateId = request.ProductTemplateId,
                    StandardCost = request.StandardCost,
                    SellingPrice = request.SellingPrice,
                    StandardLeadTimeDays = request.StandardLeadTimeDays,
                    MinOrderQuantity = request.MinOrderQuantity,
                    Category = request.Category?.Trim(),
                    ProductType = request.ProductType?.Trim(),
                    Description = request.Description?.Trim(),
                    HSNCode = request.HSNCode?.Trim(),
                    UOM = request.UOM?.Trim() ?? "PCS",
                    Remarks = request.Remarks?.Trim(),
                    IsActive = true,
                    CreatedBy = request.CreatedBy?.Trim() ?? "System"
                };

                var productId = await _productRepository.InsertAsync(product);
                return ApiResponse<int>.SuccessResponse(productId, $"Product '{request.PartCode}' created successfully");
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
                var existingProduct = await _productRepository.GetByIdAsync(request.Id);
                if (existingProduct == null)
                    return ApiResponse<bool>.ErrorResponse("Product not found");

                // Validate RollerType - Only Printing Roller or Magnetic Roller allowed
                var validRollerTypes = new[] { "Printing Roller", "Magnetic Roller" };
                if (!validRollerTypes.Contains(request.RollerType))
                    return ApiResponse<bool>.ErrorResponse("Roller type must be either 'Printing Roller' or 'Magnetic Roller'");

                if (existingProduct.PartCode != request.PartCode)
                {
                    var exists = await _productRepository.ExistsAsync(request.PartCode);
                    if (exists)
                        return ApiResponse<bool>.ErrorResponse($"Product code '{request.PartCode}' already exists");
                }

                existingProduct.PartCode = request.PartCode.Trim().ToUpper();
                existingProduct.CustomerId = request.CustomerId;
                existingProduct.CustomerName = request.CustomerName?.Trim();
                existingProduct.ModelName = request.ModelName.Trim();
                existingProduct.RollerType = request.RollerType.Trim();
                existingProduct.Diameter = request.Diameter;
                existingProduct.Length = request.Length;
                existingProduct.Weight = request.Weight;
                existingProduct.MaterialGrade = request.MaterialGrade?.Trim();
                existingProduct.SurfaceFinish = request.SurfaceFinish?.Trim();
                existingProduct.Hardness = request.Hardness?.Trim();
                existingProduct.DrawingNo = request.DrawingNo?.Trim();
                existingProduct.RevisionNo = request.RevisionNo?.Trim();
                existingProduct.DrawingId = request.DrawingId;
                existingProduct.ProcessTemplateId = request.ProcessTemplateId;
                existingProduct.ProductTemplateId = request.ProductTemplateId;
                existingProduct.StandardCost = request.StandardCost;
                existingProduct.SellingPrice = request.SellingPrice;
                existingProduct.StandardLeadTimeDays = request.StandardLeadTimeDays;
                existingProduct.MinOrderQuantity = request.MinOrderQuantity;
                existingProduct.Category = request.Category?.Trim();
                existingProduct.ProductType = request.ProductType?.Trim();
                existingProduct.Description = request.Description?.Trim();
                existingProduct.HSNCode = request.HSNCode?.Trim();
                existingProduct.UOM = request.UOM?.Trim();
                existingProduct.Remarks = request.Remarks?.Trim();
                existingProduct.IsActive = request.IsActive;
                existingProduct.UpdatedBy = request.UpdatedBy?.Trim() ?? "System";
                existingProduct.UpdatedAt = DateTime.UtcNow;

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
                    return ApiResponse<bool>.ErrorResponse("Product not found");

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

        public async Task<ApiResponse<bool>> ActivateProductAsync(int id)
        {
            try
            {
                var product = await _productRepository.GetByIdAsync(id);
                if (product == null)
                    return ApiResponse<bool>.ErrorResponse("Product not found");
                if (product.IsActive)
                    return ApiResponse<bool>.ErrorResponse("Product is already active");

                var success = await _productRepository.ActivateAsync(id);
                if (!success)
                    return ApiResponse<bool>.ErrorResponse("Failed to activate product");

                return ApiResponse<bool>.SuccessResponse(true, "Product activated successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error activating product: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> DeactivateProductAsync(int id)
        {
            try
            {
                var product = await _productRepository.GetByIdAsync(id);
                if (product == null)
                    return ApiResponse<bool>.ErrorResponse("Product not found");
                if (!product.IsActive)
                    return ApiResponse<bool>.ErrorResponse("Product is already inactive");

                var success = await _productRepository.DeactivateAsync(id);
                if (!success)
                    return ApiResponse<bool>.ErrorResponse("Failed to deactivate product");

                return ApiResponse<bool>.SuccessResponse(true, "Product deactivated successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error deactivating product: {ex.Message}");
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

        public async Task<ApiResponse<IEnumerable<ProductResponse>>> GetByCategoryAsync(string category)
        {
            try
            {
                var products = await _productRepository.GetByCategoryAsync(category);
                var responses = products.Select(MapToResponse).ToList();
                return ApiResponse<IEnumerable<ProductResponse>>.SuccessResponse(responses);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<ProductResponse>>.ErrorResponse($"Error retrieving products: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<ProductResponse>>> GetByProductTypeAsync(string productType)
        {
            try
            {
                var products = await _productRepository.GetByProductTypeAsync(productType);
                var responses = products.Select(MapToResponse).ToList();
                return ApiResponse<IEnumerable<ProductResponse>>.SuccessResponse(responses);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<ProductResponse>>.ErrorResponse($"Error retrieving products: {ex.Message}");
            }
        }

        private static ProductResponse MapToResponse(Product product)
        {
            return new ProductResponse
            {
                Id = product.Id,
                PartCode = product.PartCode,
                CustomerId = product.CustomerId,
                CustomerName = product.CustomerName,
                ModelName = product.ModelName,
                RollerType = product.RollerType,
                Diameter = product.Diameter,
                Length = product.Length,
                Weight = product.Weight,
                MaterialGrade = product.MaterialGrade,
                SurfaceFinish = product.SurfaceFinish,
                Hardness = product.Hardness,
                DrawingNo = product.DrawingNo,
                RevisionNo = product.RevisionNo,
                DrawingId = product.DrawingId,
                ProcessTemplateId = product.ProcessTemplateId,
                ProductTemplateId = product.ProductTemplateId,
                StandardCost = product.StandardCost,
                SellingPrice = product.SellingPrice,
                StandardLeadTimeDays = product.StandardLeadTimeDays,
                MinOrderQuantity = product.MinOrderQuantity,
                Category = product.Category,
                ProductType = product.ProductType,
                Description = product.Description,
                HSNCode = product.HSNCode,
                UOM = product.UOM,
                Remarks = product.Remarks,
                IsActive = product.IsActive,
                CreatedAt = product.CreatedAt,
                CreatedBy = product.CreatedBy,
                UpdatedAt = product.UpdatedAt,
                UpdatedBy = product.UpdatedBy
            };
        }
    }
}

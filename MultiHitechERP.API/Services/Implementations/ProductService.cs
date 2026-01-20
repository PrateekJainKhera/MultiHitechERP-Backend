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

        public async Task<ApiResponse<ProductResponse>> GetByIdAsync(Guid id)
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

        public async Task<ApiResponse<ProductResponse>> GetByProductCodeAsync(string productCode)
        {
            try
            {
                var product = await _productRepository.GetByProductCodeAsync(productCode);
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

        public async Task<ApiResponse<Guid>> CreateProductAsync(CreateProductRequest request)
        {
            try
            {
                var exists = await _productRepository.ExistsAsync(request.ProductCode);
                if (exists)
                    return ApiResponse<Guid>.ErrorResponse($"Product code '{request.ProductCode}' already exists");

                if (string.IsNullOrWhiteSpace(request.ProductName))
                    return ApiResponse<Guid>.ErrorResponse("Product name is required");

                if (!string.IsNullOrWhiteSpace(request.HSNCode) && (request.HSNCode.Length < 4 || request.HSNCode.Length > 8))
                    return ApiResponse<Guid>.ErrorResponse("HSN Code must be between 4 and 8 digits");

                var product = new Product
                {
                    ProductCode = request.ProductCode.Trim().ToUpper(),
                    ProductName = request.ProductName.Trim(),
                    Category = request.Category?.Trim(),
                    SubCategory = request.SubCategory?.Trim(),
                    ProductType = request.ProductType?.Trim(),
                    Specification = request.Specification?.Trim(),
                    Description = request.Description?.Trim(),
                    HSNCode = request.HSNCode?.Trim(),
                    Length = request.Length,
                    Width = request.Width,
                    Height = request.Height,
                    Diameter = request.Diameter,
                    Weight = request.Weight,
                    UOM = request.UOM?.Trim() ?? "PCS",
                    DrawingId = request.DrawingId,
                    DrawingNumber = request.DrawingNumber?.Trim(),
                    BOMId = request.BOMId,
                    ProcessRouteId = request.ProcessRouteId,
                    StandardCost = request.StandardCost,
                    SellingPrice = request.SellingPrice,
                    MaterialGrade = request.MaterialGrade?.Trim(),
                    MaterialSpecification = request.MaterialSpecification?.Trim(),
                    StandardBatchSize = request.StandardBatchSize ?? 1,
                    MinOrderQuantity = request.MinOrderQuantity ?? 1,
                    LeadTimeDays = request.LeadTimeDays ?? 30,
                    IsActive = true,
                    Status = "Active",
                    Remarks = request.Remarks?.Trim(),
                    CreatedBy = request.CreatedBy?.Trim() ?? "System"
                };

                var productId = await _productRepository.InsertAsync(product);
                return ApiResponse<Guid>.SuccessResponse(productId, $"Product '{request.ProductCode}' created successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<Guid>.ErrorResponse($"Error creating product: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> UpdateProductAsync(UpdateProductRequest request)
        {
            try
            {
                var existingProduct = await _productRepository.GetByIdAsync(request.Id);
                if (existingProduct == null)
                    return ApiResponse<bool>.ErrorResponse("Product not found");

                if (existingProduct.ProductCode != request.ProductCode)
                {
                    var exists = await _productRepository.ExistsAsync(request.ProductCode);
                    if (exists)
                        return ApiResponse<bool>.ErrorResponse($"Product code '{request.ProductCode}' already exists");
                }

                existingProduct.ProductCode = request.ProductCode.Trim().ToUpper();
                existingProduct.ProductName = request.ProductName.Trim();
                existingProduct.Category = request.Category?.Trim();
                existingProduct.SubCategory = request.SubCategory?.Trim();
                existingProduct.ProductType = request.ProductType?.Trim();
                existingProduct.Specification = request.Specification?.Trim();
                existingProduct.Description = request.Description?.Trim();
                existingProduct.HSNCode = request.HSNCode?.Trim();
                existingProduct.Length = request.Length;
                existingProduct.Width = request.Width;
                existingProduct.Height = request.Height;
                existingProduct.Diameter = request.Diameter;
                existingProduct.Weight = request.Weight;
                existingProduct.UOM = request.UOM?.Trim();
                existingProduct.DrawingId = request.DrawingId;
                existingProduct.DrawingNumber = request.DrawingNumber?.Trim();
                existingProduct.BOMId = request.BOMId;
                existingProduct.ProcessRouteId = request.ProcessRouteId;
                existingProduct.StandardCost = request.StandardCost;
                existingProduct.SellingPrice = request.SellingPrice;
                existingProduct.MaterialGrade = request.MaterialGrade?.Trim();
                existingProduct.MaterialSpecification = request.MaterialSpecification?.Trim();
                existingProduct.StandardBatchSize = request.StandardBatchSize;
                existingProduct.MinOrderQuantity = request.MinOrderQuantity;
                existingProduct.LeadTimeDays = request.LeadTimeDays;
                existingProduct.IsActive = request.IsActive;
                existingProduct.Status = request.Status;
                existingProduct.Remarks = request.Remarks?.Trim();
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

        public async Task<ApiResponse<bool>> DeleteProductAsync(Guid id)
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

        public async Task<ApiResponse<bool>> ActivateProductAsync(Guid id)
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

        public async Task<ApiResponse<bool>> DeactivateProductAsync(Guid id)
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
                ProductCode = product.ProductCode,
                ProductName = product.ProductName,
                Category = product.Category,
                SubCategory = product.SubCategory,
                ProductType = product.ProductType,
                Specification = product.Specification,
                Description = product.Description,
                HSNCode = product.HSNCode,
                Length = product.Length,
                Width = product.Width,
                Height = product.Height,
                Diameter = product.Diameter,
                Weight = product.Weight,
                UOM = product.UOM,
                DrawingId = product.DrawingId,
                DrawingNumber = product.DrawingNumber,
                BOMId = product.BOMId,
                ProcessRouteId = product.ProcessRouteId,
                StandardCost = product.StandardCost,
                SellingPrice = product.SellingPrice,
                MaterialGrade = product.MaterialGrade,
                MaterialSpecification = product.MaterialSpecification,
                StandardBatchSize = product.StandardBatchSize,
                MinOrderQuantity = product.MinOrderQuantity,
                LeadTimeDays = product.LeadTimeDays,
                IsActive = product.IsActive,
                Status = product.Status,
                Remarks = product.Remarks,
                CreatedAt = product.CreatedAt,
                CreatedBy = product.CreatedBy,
                UpdatedAt = product.UpdatedAt,
                UpdatedBy = product.UpdatedBy
            };
        }
    }
}

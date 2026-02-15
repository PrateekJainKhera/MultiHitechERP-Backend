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
        private readonly IMachineModelRepository _machineModelRepository;

        public ProductService(
            IProductRepository productRepository,
            IMachineModelRepository machineModelRepository)
        {
            _productRepository = productRepository;
            _machineModelRepository = machineModelRepository;
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
                // Business validation: Only Printing Roller, Magnetic Roller, or Other allowed
                var validRollerTypes = new[] { "Printing Roller", "Magnetic Roller", "Other" };
                if (!validRollerTypes.Contains(request.RollerType))
                    return ApiResponse<int>.ErrorResponse("Roller type must be either 'Printing Roller', 'Magnetic Roller', or 'Other'");

                // Business validation: Validate dimensions
                if (request.Diameter <= 0)
                    return ApiResponse<int>.ErrorResponse("Diameter must be greater than 0");

                if (request.Length <= 0)
                    return ApiResponse<int>.ErrorResponse("Length must be greater than 0");

                // Validate and get machine model
                var machineModel = await _machineModelRepository.GetByIdAsync(request.ModelId);
                if (machineModel == null)
                    return ApiResponse<int>.ErrorResponse($"Machine model with ID {request.ModelId} not found");

                // Auto-generate PartCode based on roller type
                string partCodePrefix = request.RollerType == "Magnetic Roller" ? "MAG" : "PRT";
                int nextSequence = await _productRepository.GetNextSequenceNumberAsync(request.RollerType);
                string generatedPartCode = $"{partCodePrefix}-{nextSequence:D4}";

                // Create product entity
                var product = new Product
                {
                    PartCode = generatedPartCode,
                    CustomerName = request.CustomerName?.Trim(),
                    ModelId = request.ModelId,
                    ModelName = machineModel.ModelName, // Populated from MachineModel
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
                    ProductTemplateId = request.ProductTemplateId,
                    ProcessTemplateId = request.ProcessTemplateId,
                    CreatedBy = request.CreatedBy?.Trim() ?? "System",
                    // Drawing request handling
                    DrawingReviewStatus = request.RequestDrawing ? "UnderReview" : "Pending",
                    DrawingRequestedAt = request.RequestDrawing ? DateTime.UtcNow : (DateTime?)null,
                    DrawingRequestedBy = request.RequestDrawing ? (request.CreatedBy?.Trim() ?? "System") : null
                };

                var productId = await _productRepository.InsertAsync(product);

                string successMessage = request.RequestDrawing
                    ? $"Product '{generatedPartCode}' created and drawing requested successfully"
                    : $"Product '{generatedPartCode}' created successfully";

                return ApiResponse<int>.SuccessResponse(productId, successMessage);
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

                // Business validation: Only Printing Roller, Magnetic Roller, or Other allowed
                var validRollerTypes = new[] { "Printing Roller", "Magnetic Roller", "Other" };
                if (!validRollerTypes.Contains(request.RollerType))
                    return ApiResponse<bool>.ErrorResponse("Roller type must be either 'Printing Roller', 'Magnetic Roller', or 'Other'");

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

                // Validate and get machine model
                var machineModel = await _machineModelRepository.GetByIdAsync(request.ModelId);
                if (machineModel == null)
                    return ApiResponse<bool>.ErrorResponse($"Machine model with ID {request.ModelId} not found");

                // Update product entity
                existingProduct.PartCode = request.PartCode.Trim().ToUpper();
                existingProduct.CustomerName = request.CustomerName?.Trim();
                existingProduct.ModelId = request.ModelId;
                existingProduct.ModelName = machineModel.ModelName;
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
                existingProduct.ProductTemplateId = request.ProductTemplateId;
                existingProduct.ProcessTemplateId = request.ProcessTemplateId;
                existingProduct.AssemblyDrawingId = request.AssemblyDrawingId;
                existingProduct.CustomerProvidedDrawingId = request.CustomerProvidedDrawingId;

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

        public async Task<ApiResponse<IEnumerable<ProductResponse>>> SearchByCriteriaAsync(int modelId, string rollerType, int numberOfTeeth)
        {
            try
            {
                var products = await _productRepository.SearchByCriteriaAsync(modelId, rollerType, numberOfTeeth);
                var responses = products.Select(MapToResponse).ToList();
                return ApiResponse<IEnumerable<ProductResponse>>.SuccessResponse(responses);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<ProductResponse>>.ErrorResponse($"Error searching products by criteria: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> UpdateDrawingReviewStatusAsync(int productId, string status, string? notes, string reviewedBy)
        {
            try
            {
                // Verify product exists
                var product = await _productRepository.GetByIdAsync(productId);
                if (product == null)
                    return ApiResponse<bool>.ErrorResponse($"Product with ID {productId} not found");

                // Validate status
                var validStatuses = new[] { "Pending", "UnderReview", "Approved", "Rejected", "RevisionRequired" };
                if (!validStatuses.Contains(status))
                    return ApiResponse<bool>.ErrorResponse($"Invalid status. Must be one of: {string.Join(", ", validStatuses)}");

                // Update drawing review fields
                product.DrawingReviewStatus = status;
                product.DrawingReviewNotes = notes?.Trim();
                product.DrawingReviewedBy = reviewedBy?.Trim();
                product.DrawingReviewedAt = DateTime.UtcNow;

                var success = await _productRepository.UpdateAsync(product);
                if (!success)
                    return ApiResponse<bool>.ErrorResponse("Failed to update drawing review status");

                return ApiResponse<bool>.SuccessResponse(true, $"Drawing review status updated to '{status}' successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error updating drawing review status: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> RequestDrawingAsync(int productId, string requestedBy)
        {
            try
            {
                // Verify product exists
                var product = await _productRepository.GetByIdAsync(productId);
                if (product == null)
                    return ApiResponse<bool>.ErrorResponse($"Product with ID {productId} not found");

                // Validate current status - can only request drawing if status is Pending
                if (product.DrawingReviewStatus != "Pending")
                    return ApiResponse<bool>.ErrorResponse($"Cannot request drawing. Current status is '{product.DrawingReviewStatus}'. Drawing can only be requested for products with 'Pending' status.");

                // Update status to UnderReview and set request tracking fields
                product.DrawingReviewStatus = "UnderReview";
                product.DrawingRequestedAt = DateTime.UtcNow;
                product.DrawingRequestedBy = requestedBy?.Trim();

                var success = await _productRepository.UpdateAsync(product);
                if (!success)
                    return ApiResponse<bool>.ErrorResponse("Failed to request drawing");

                return ApiResponse<bool>.SuccessResponse(true, $"Drawing request sent to drawing team successfully for product '{product.PartCode}'");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error requesting drawing: {ex.Message}");
            }
        }

        private static ProductResponse MapToResponse(Product product)
        {
            return new ProductResponse
            {
                Id = product.Id,
                PartCode = product.PartCode,
                CustomerName = product.CustomerName,
                ModelId = product.ModelId,
                ModelName = product.ModelName,
                RollerType = product.RollerType,
                Diameter = product.Diameter,
                Length = product.Length,
                MaterialGrade = product.MaterialGrade,
                SurfaceFinish = product.SurfaceFinish,
                Hardness = product.Hardness,

                // Legacy drawing fields
                DrawingNo = product.DrawingNo,
                RevisionNo = product.RevisionNo,
                RevisionDate = product.RevisionDate,

                // Product-Level Drawing Review
                AssemblyDrawingId = product.AssemblyDrawingId,
                CustomerProvidedDrawingId = product.CustomerProvidedDrawingId,
                DrawingReviewStatus = product.DrawingReviewStatus,
                DrawingReviewedBy = product.DrawingReviewedBy,
                DrawingReviewedAt = product.DrawingReviewedAt,
                DrawingReviewNotes = product.DrawingReviewNotes,
                DrawingRequestedAt = product.DrawingRequestedAt,
                DrawingRequestedBy = product.DrawingRequestedBy,

                NumberOfTeeth = product.NumberOfTeeth,
                ProductTemplateId = product.ProductTemplateId,
                ProcessTemplateId = product.ProcessTemplateId,
                CreatedAt = product.CreatedAt,
                UpdatedAt = product.UpdatedAt,
                CreatedBy = product.CreatedBy
            };
        }
    }
}

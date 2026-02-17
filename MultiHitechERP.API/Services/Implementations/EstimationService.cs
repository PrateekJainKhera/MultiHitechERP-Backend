using MultiHitechERP.API.DTOs.Request;
using MultiHitechERP.API.DTOs.Response;
using MultiHitechERP.API.Models.Sales;
using MultiHitechERP.API.Repositories.Interfaces;
using MultiHitechERP.API.Services.Interfaces;

namespace MultiHitechERP.API.Services.Implementations
{
    public class EstimationService : IEstimationService
    {
        private readonly IEstimationRepository _estimationRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IProductRepository _productRepository;
        private readonly IOrderService _orderService;

        public EstimationService(
            IEstimationRepository estimationRepository,
            ICustomerRepository customerRepository,
            IProductRepository productRepository,
            IOrderService orderService)
        {
            _estimationRepository = estimationRepository;
            _customerRepository = customerRepository;
            _productRepository = productRepository;
            _orderService = orderService;
        }

        public async Task<ApiResponse<IEnumerable<EstimationResponse>>> GetAllAsync()
        {
            try
            {
                var estimations = await _estimationRepository.GetAllAsync();
                var responses = estimations.Select(MapToResponse);
                return ApiResponse<IEnumerable<EstimationResponse>>.SuccessResponse(responses);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<EstimationResponse>>.ErrorResponse($"Error retrieving estimations: {ex.Message}");
            }
        }

        public async Task<ApiResponse<EstimationResponse>> GetByIdAsync(int id)
        {
            try
            {
                var estimation = await _estimationRepository.GetByIdAsync(id);
                if (estimation == null)
                    return ApiResponse<EstimationResponse>.ErrorResponse($"Estimation {id} not found");

                return ApiResponse<EstimationResponse>.SuccessResponse(MapToResponse(estimation));
            }
            catch (Exception ex)
            {
                return ApiResponse<EstimationResponse>.ErrorResponse($"Error retrieving estimation: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<EstimationResponse>>> GetByCustomerIdAsync(int customerId)
        {
            try
            {
                var estimations = await _estimationRepository.GetByCustomerIdAsync(customerId);
                return ApiResponse<IEnumerable<EstimationResponse>>.SuccessResponse(estimations.Select(MapToResponse));
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<EstimationResponse>>.ErrorResponse($"Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<EstimationResponse>>> GetByStatusAsync(string status)
        {
            try
            {
                var estimations = await _estimationRepository.GetByStatusAsync(status);
                return ApiResponse<IEnumerable<EstimationResponse>>.SuccessResponse(estimations.Select(MapToResponse));
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<EstimationResponse>>.ErrorResponse($"Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<EstimationResponse>> CreateAsync(CreateEstimationRequest request)
        {
            try
            {
                var customer = await _customerRepository.GetByIdAsync(request.CustomerId);
                if (customer == null)
                    return ApiResponse<EstimationResponse>.ErrorResponse("Customer not found");

                if (request.Items == null || request.Items.Count == 0)
                    return ApiResponse<EstimationResponse>.ErrorResponse("At least one item is required");

                var seq = await _estimationRepository.GetNextSequenceNumberAsync();
                var now = DateTime.UtcNow;
                var yearMonth = $"{now.Year:D4}{now.Month:D2}";
                var baseNo = $"EST-{yearMonth}-{seq:D3}";
                var estimateNo = baseNo; // R1 = no suffix

                var items = await BuildItemsAsync(request.Items);
                if (items == null)
                    return ApiResponse<EstimationResponse>.ErrorResponse("One or more products not found");

                var subTotal = items.Sum(i => i.TotalPrice);
                var (discountAmount, total) = CalculateDiscount(subTotal, request.DiscountType, request.DiscountValue);

                var estimation = new Estimation
                {
                    EstimateNo = estimateNo,
                    BaseEstimateNo = baseNo,
                    RevisionNumber = 1,
                    CustomerId = request.CustomerId,
                    CustomerName = customer.CustomerName,
                    Status = "Draft",
                    SubTotal = subTotal,
                    DiscountType = request.DiscountType,
                    DiscountValue = request.DiscountValue,
                    DiscountAmount = discountAmount,
                    TotalAmount = total,
                    ValidUntil = now.AddDays(21),
                    Notes = request.Notes,
                    TermsAndConditions = request.TermsAndConditions,
                    CreatedAt = now,
                    CreatedBy = "Admin",
                    Items = items,
                };

                var id = await _estimationRepository.InsertAsync(estimation);
                estimation.Id = id;

                return ApiResponse<EstimationResponse>.SuccessResponse(MapToResponse(estimation));
            }
            catch (Exception ex)
            {
                return ApiResponse<EstimationResponse>.ErrorResponse($"Error creating estimation: {ex.Message}");
            }
        }

        public async Task<ApiResponse<EstimationResponse>> ReviseAsync(int id, CreateEstimationRequest request)
        {
            try
            {
                var existing = await _estimationRepository.GetByIdAsync(id);
                if (existing == null)
                    return ApiResponse<EstimationResponse>.ErrorResponse("Estimation not found");

                if (existing.Status == "Converted")
                    return ApiResponse<EstimationResponse>.ErrorResponse("Cannot revise a converted estimation");

                var customer = await _customerRepository.GetByIdAsync(request.CustomerId);
                if (customer == null)
                    return ApiResponse<EstimationResponse>.ErrorResponse("Customer not found");

                if (request.Items == null || request.Items.Count == 0)
                    return ApiResponse<EstimationResponse>.ErrorResponse("At least one item is required");

                // Cancel the old one
                await _estimationRepository.UpdateStatusAsync(existing.Id, "Cancelled");

                var newRevision = existing.RevisionNumber + 1;
                var estimateNo = $"{existing.BaseEstimateNo}-R{newRevision}";

                var items = await BuildItemsAsync(request.Items);
                if (items == null)
                    return ApiResponse<EstimationResponse>.ErrorResponse("One or more products not found");

                var subTotal = items.Sum(i => i.TotalPrice);
                var (discountAmount, total) = CalculateDiscount(subTotal, request.DiscountType, request.DiscountValue);
                var now = DateTime.UtcNow;

                var revised = new Estimation
                {
                    EstimateNo = estimateNo,
                    BaseEstimateNo = existing.BaseEstimateNo,
                    RevisionNumber = newRevision,
                    CustomerId = request.CustomerId,
                    CustomerName = customer.CustomerName,
                    Status = "Draft",
                    SubTotal = subTotal,
                    DiscountType = request.DiscountType,
                    DiscountValue = request.DiscountValue,
                    DiscountAmount = discountAmount,
                    TotalAmount = total,
                    ValidUntil = now.AddDays(21),
                    Notes = request.Notes,
                    TermsAndConditions = request.TermsAndConditions,
                    CreatedAt = now,
                    CreatedBy = "Admin",
                    Items = items,
                };

                var newId = await _estimationRepository.InsertAsync(revised);
                revised.Id = newId;

                return ApiResponse<EstimationResponse>.SuccessResponse(MapToResponse(revised));
            }
            catch (Exception ex)
            {
                return ApiResponse<EstimationResponse>.ErrorResponse($"Error revising estimation: {ex.Message}");
            }
        }

        public async Task<ApiResponse<EstimationResponse>> SubmitAsync(int id)
        {
            try
            {
                var estimation = await _estimationRepository.GetByIdAsync(id);
                if (estimation == null)
                    return ApiResponse<EstimationResponse>.ErrorResponse("Estimation not found");

                if (estimation.Status != "Draft")
                    return ApiResponse<EstimationResponse>.ErrorResponse($"Cannot submit estimation in '{estimation.Status}' status");

                await _estimationRepository.UpdateStatusAsync(id, "Submitted");
                estimation.Status = "Submitted";
                return ApiResponse<EstimationResponse>.SuccessResponse(MapToResponse(estimation));
            }
            catch (Exception ex)
            {
                return ApiResponse<EstimationResponse>.ErrorResponse($"Error submitting estimation: {ex.Message}");
            }
        }

        public async Task<ApiResponse<EstimationResponse>> ApproveAsync(int id, ApproveEstimationRequest request)
        {
            try
            {
                var estimation = await _estimationRepository.GetByIdAsync(id);
                if (estimation == null)
                    return ApiResponse<EstimationResponse>.ErrorResponse("Estimation not found");

                if (estimation.Status != "Submitted")
                    return ApiResponse<EstimationResponse>.ErrorResponse($"Cannot approve estimation in '{estimation.Status}' status. Submit first.");

                await _estimationRepository.UpdateStatusAsync(id, "Approved", request.ApprovedBy);
                estimation.Status = "Approved";
                estimation.ApprovedBy = request.ApprovedBy;
                estimation.ApprovedAt = DateTime.UtcNow;
                return ApiResponse<EstimationResponse>.SuccessResponse(MapToResponse(estimation));
            }
            catch (Exception ex)
            {
                return ApiResponse<EstimationResponse>.ErrorResponse($"Error approving estimation: {ex.Message}");
            }
        }

        public async Task<ApiResponse<EstimationResponse>> RejectAsync(int id, RejectEstimationRequest request)
        {
            try
            {
                var estimation = await _estimationRepository.GetByIdAsync(id);
                if (estimation == null)
                    return ApiResponse<EstimationResponse>.ErrorResponse("Estimation not found");

                if (estimation.Status != "Submitted")
                    return ApiResponse<EstimationResponse>.ErrorResponse($"Cannot reject estimation in '{estimation.Status}' status");

                await _estimationRepository.UpdateStatusAsync(id, "Rejected", request.RejectedBy, null, request.Reason);
                estimation.Status = "Rejected";
                return ApiResponse<EstimationResponse>.SuccessResponse(MapToResponse(estimation));
            }
            catch (Exception ex)
            {
                return ApiResponse<EstimationResponse>.ErrorResponse($"Error rejecting estimation: {ex.Message}");
            }
        }

        public async Task<ApiResponse<EstimationResponse>> ConvertToOrderAsync(int id)
        {
            try
            {
                var estimation = await _estimationRepository.GetByIdAsync(id);
                if (estimation == null)
                    return ApiResponse<EstimationResponse>.ErrorResponse("Estimation not found");

                if (estimation.Status != "Approved")
                    return ApiResponse<EstimationResponse>.ErrorResponse("Only approved estimations can be converted to orders");

                var orderRequest = new CreateOrderRequest
                {
                    CustomerId = estimation.CustomerId,
                    Items = estimation.Items.Select(item => new CreateOrderItemRequest
                    {
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        DueDate = estimation.ValidUntil,
                        Priority = "Medium",
                    }).ToList(),
                };

                var orderResult = await _orderService.CreateOrderAsync(orderRequest);
                if (!orderResult.Success)
                    return ApiResponse<EstimationResponse>.ErrorResponse($"Failed to create order: {orderResult.Message}");

                var orderId = orderResult.Data;
                await _estimationRepository.UpdateStatusAsync(id, "Converted", null, orderId);
                estimation.Status = "Converted";
                estimation.ConvertedOrderId = orderId;
                estimation.ConvertedAt = DateTime.UtcNow;

                return ApiResponse<EstimationResponse>.SuccessResponse(MapToResponse(estimation));
            }
            catch (Exception ex)
            {
                return ApiResponse<EstimationResponse>.ErrorResponse($"Error converting estimation to order: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> DeleteAsync(int id)
        {
            try
            {
                var estimation = await _estimationRepository.GetByIdAsync(id);
                if (estimation == null)
                    return ApiResponse<bool>.ErrorResponse("Estimation not found");

                if (estimation.Status == "Converted")
                    return ApiResponse<bool>.ErrorResponse("Cannot delete a converted estimation");

                await _estimationRepository.DeleteAsync(id);
                return ApiResponse<bool>.SuccessResponse(true);
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error deleting estimation: {ex.Message}");
            }
        }

        private async Task<List<EstimationItem>?> BuildItemsAsync(List<CreateEstimationItemRequest> itemRequests)
        {
            var items = new List<EstimationItem>();
            foreach (var req in itemRequests)
            {
                var product = await _productRepository.GetByIdAsync(req.ProductId);
                if (product == null) return null;

                items.Add(new EstimationItem
                {
                    ProductId = req.ProductId,
                    ProductName = product.PartCode,
                    PartCode = product.PartCode,
                    Quantity = req.Quantity,
                    UnitPrice = req.UnitPrice,
                    TotalPrice = req.Quantity * req.UnitPrice,
                    Notes = req.Notes,
                });
            }
            return items;
        }

        private static (decimal discountAmount, decimal total) CalculateDiscount(decimal subTotal, string? discountType, decimal discountValue)
        {
            decimal discountAmount = 0;
            if (discountType == "Percent" && discountValue > 0)
                discountAmount = Math.Round(subTotal * discountValue / 100, 2);
            else if (discountType == "Fixed" && discountValue > 0)
                discountAmount = Math.Min(discountValue, subTotal);

            return (discountAmount, subTotal - discountAmount);
        }

        private static EstimationResponse MapToResponse(Estimation e)
        {
            return new EstimationResponse
            {
                Id = e.Id,
                EstimateNo = e.EstimateNo,
                BaseEstimateNo = e.BaseEstimateNo,
                RevisionNumber = e.RevisionNumber,
                CustomerId = e.CustomerId,
                CustomerName = e.CustomerName,
                Status = e.Status,
                SubTotal = e.SubTotal,
                DiscountType = e.DiscountType,
                DiscountValue = e.DiscountValue,
                DiscountAmount = e.DiscountAmount,
                TotalAmount = e.TotalAmount,
                ValidUntil = e.ValidUntil.ToString("yyyy-MM-dd"),
                ApprovedBy = e.ApprovedBy,
                ApprovedAt = e.ApprovedAt?.ToString("yyyy-MM-dd HH:mm"),
                RejectedBy = e.RejectedBy,
                RejectedAt = e.RejectedAt?.ToString("yyyy-MM-dd HH:mm"),
                RejectionReason = e.RejectionReason,
                ConvertedOrderId = e.ConvertedOrderId,
                ConvertedAt = e.ConvertedAt?.ToString("yyyy-MM-dd HH:mm"),
                Notes = e.Notes,
                TermsAndConditions = e.TermsAndConditions,
                CreatedAt = e.CreatedAt.ToString("yyyy-MM-dd HH:mm"),
                CreatedBy = e.CreatedBy,
                Items = e.Items.Select(i => new EstimationItemResponse
                {
                    Id = i.Id,
                    EstimationId = i.EstimationId,
                    ProductId = i.ProductId,
                    ProductName = i.ProductName,
                    PartCode = i.PartCode,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice,
                    TotalPrice = i.TotalPrice,
                    Notes = i.Notes,
                }).ToList(),
            };
        }
    }
}

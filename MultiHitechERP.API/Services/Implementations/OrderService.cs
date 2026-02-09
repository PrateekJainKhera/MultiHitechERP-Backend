using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MultiHitechERP.API.DTOs.Request;
using MultiHitechERP.API.DTOs.Response;
using MultiHitechERP.API.Models.Orders;
using MultiHitechERP.API.Repositories.Interfaces;
using MultiHitechERP.API.Services.Interfaces;

namespace MultiHitechERP.API.Services.Implementations
{
    /// <summary>
    /// Service implementation for Order business logic
    /// </summary>
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IProductRepository _productRepository;

        public OrderService(
            IOrderRepository orderRepository,
            ICustomerRepository customerRepository,
            IProductRepository productRepository)
        {
            _orderRepository = orderRepository;
            _customerRepository = customerRepository;
            _productRepository = productRepository;
        }

        public async Task<ApiResponse<OrderResponse>> GetByIdAsync(int id)
        {
            try
            {
                var order = await _orderRepository.GetByIdAsync(id);
                if (order == null)
                {
                    return ApiResponse<OrderResponse>.ErrorResponse($"Order with ID {id} not found");
                }

                var response = await MapToResponseAsync(order);
                return ApiResponse<OrderResponse>.SuccessResponse(response);
            }
            catch (Exception ex)
            {
                return ApiResponse<OrderResponse>.ErrorResponse($"Error retrieving order: {ex.Message}");
            }
        }

        public async Task<ApiResponse<OrderResponse>> GetByOrderNoAsync(string orderNo)
        {
            try
            {
                var order = await _orderRepository.GetByOrderNoAsync(orderNo);
                if (order == null)
                {
                    return ApiResponse<OrderResponse>.ErrorResponse($"Order {orderNo} not found");
                }

                var response = await MapToResponseAsync(order);
                return ApiResponse<OrderResponse>.SuccessResponse(response);
            }
            catch (Exception ex)
            {
                return ApiResponse<OrderResponse>.ErrorResponse($"Error retrieving order: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<OrderResponse>>> GetAllAsync()
        {
            try
            {
                var orders = await _orderRepository.GetAllAsync();
                var responses = new List<OrderResponse>();

                foreach (var order in orders)
                {
                    responses.Add(await MapToResponseAsync(order));
                }

                return ApiResponse<IEnumerable<OrderResponse>>.SuccessResponse(responses);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<OrderResponse>>.ErrorResponse($"Error retrieving orders: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<OrderResponse>>> GetByCustomerIdAsync(int customerId)
        {
            try
            {
                var orders = await _orderRepository.GetByCustomerIdAsync(customerId);
                var responses = new List<OrderResponse>();

                foreach (var order in orders)
                {
                    responses.Add(await MapToResponseAsync(order));
                }

                return ApiResponse<IEnumerable<OrderResponse>>.SuccessResponse(responses);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<OrderResponse>>.ErrorResponse($"Error retrieving orders: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<OrderResponse>>> GetByStatusAsync(string status)
        {
            try
            {
                var orders = await _orderRepository.GetByStatusAsync(status);
                var responses = new List<OrderResponse>();

                foreach (var order in orders)
                {
                    responses.Add(await MapToResponseAsync(order));
                }

                return ApiResponse<IEnumerable<OrderResponse>>.SuccessResponse(responses);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<OrderResponse>>.ErrorResponse($"Error retrieving orders: {ex.Message}");
            }
        }

        public async Task<ApiResponse<int>> CreateOrderAsync(CreateOrderRequest request)
        {
            try
            {
                // Business Rule 1: Validate Customer exists and is active
                var customer = await _customerRepository.GetByIdAsync(request.CustomerId);
                if (customer == null)
                {
                    return ApiResponse<int>.ErrorResponse("Customer not found");
                }
                if (!customer.IsActive)
                {
                    return ApiResponse<int>.ErrorResponse("Customer is inactive");
                }

                // Business Rule 2: Validate Product exists
                var product = await _productRepository.GetByIdAsync(request.ProductId);
                if (product == null)
                {
                    return ApiResponse<int>.ErrorResponse("Product not found");
                }

                // Business Rule 3: Validate Due Date is in future
                if (request.DueDate <= DateTime.UtcNow)
                {
                    return ApiResponse<int>.ErrorResponse("Due date must be in the future");
                }

                // Business Rule 4: Validate Quantity
                if (request.Quantity <= 0)
                {
                    return ApiResponse<int>.ErrorResponse("Quantity must be greater than 0");
                }

                // Business Rule 5: Validate Order Source
                var validOrderSources = new[] { "Direct", "Agent", "Dealer", "Distributor" };
                if (!validOrderSources.Contains(request.OrderSource))
                {
                    return ApiResponse<int>.ErrorResponse($"Invalid order source. Must be one of: {string.Join(", ", validOrderSources)}");
                }

                // Business Rule 6: Validate Agent Customer if provided (optional for future use)
                if (request.AgentCustomerId.HasValue)
                {
                    var agentCustomer = await _customerRepository.GetByIdAsync(request.AgentCustomerId.Value);
                    if (agentCustomer == null)
                    {
                        return ApiResponse<int>.ErrorResponse("Agent customer not found");
                    }
                    if (agentCustomer.CustomerType != "Agent")
                    {
                        return ApiResponse<int>.ErrorResponse("Selected customer is not an agent");
                    }
                }

                // Business Rule 7: Validate Scheduling Strategy
                var validStrategies = new[] { "Due Date", "Priority Flag", "Customer Importance", "Resource Availability" };
                if (!validStrategies.Contains(request.SchedulingStrategy))
                {
                    return ApiResponse<int>.ErrorResponse($"Invalid scheduling strategy. Must be one of: {string.Join(", ", validStrategies)}");
                }

                // Business Rule 8: Validate Drawing Source if provided
                if (!string.IsNullOrEmpty(request.DrawingSource))
                {
                    var validDrawingSources = new[] { "customer", "company" };
                    if (!validDrawingSources.Contains(request.DrawingSource))
                    {
                        return ApiResponse<int>.ErrorResponse($"Invalid drawing source. Must be one of: {string.Join(", ", validDrawingSources)}");
                    }
                }

                // Generate Order Number
                var orderNo = await GenerateOrderNoInternalAsync();

                // Create Order
                var order = new Order
                {
                    OrderNo = orderNo,
                    OrderDate = request.OrderDate ?? DateTime.UtcNow, // Default to current date if not provided
                    DueDate = request.DueDate,
                    CustomerId = request.CustomerId,
                    ProductId = request.ProductId,
                    Quantity = request.Quantity,
                    OriginalQuantity = request.Quantity,
                    Status = "Pending",
                    Priority = request.Priority,
                    PlanningStatus = "Not Planned",
                    DrawingReviewStatus = "Pending",

                    // Order Source & Agent
                    OrderSource = request.OrderSource,
                    AgentCustomerId = request.AgentCustomerId,
                    AgentCommission = request.AgentCommission,
                    SchedulingStrategy = request.SchedulingStrategy,

                    // Drawing Linkage
                    PrimaryDrawingId = request.PrimaryDrawingId,
                    DrawingSource = request.DrawingSource,
                    DrawingReviewNotes = request.DrawingNotes,

                    // Template Linkage
                    LinkedProductTemplateId = request.LinkedProductTemplateId,

                    // Customer Requirements
                    CustomerMachine = request.CustomerMachine,
                    MaterialGradeRemark = request.MaterialGradeRemark,

                    // Financial (Optional - for future use)
                    OrderValue = request.OrderValue,
                    AdvancePayment = request.AdvancePayment,
                    BalancePayment = request.OrderValue.HasValue ? request.OrderValue - (request.AdvancePayment ?? 0) : null,

                    // Audit
                    CreatedBy = request.CreatedBy,
                    Version = 1
                };

                var orderId = await _orderRepository.InsertAsync(order);

                return ApiResponse<int>.SuccessResponse(orderId, $"Order {orderNo} created successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<int>.ErrorResponse($"Error creating order: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> UpdateOrderAsync(UpdateOrderRequest request)
        {
            try
            {
                // Get existing order
                var existingOrder = await _orderRepository.GetByIdAsync(request.Id);
                if (existingOrder == null)
                {
                    return ApiResponse<bool>.ErrorResponse("Order not found");
                }

                // Business Rule: Check version for optimistic locking
                if (existingOrder.Version != request.Version)
                {
                    return ApiResponse<bool>.ErrorResponse(
                        "Order has been modified by another user. Please refresh and try again.");
                }

                // Validate Customer and Product
                var customer = await _customerRepository.GetByIdAsync(request.CustomerId);
                if (customer == null || !customer.IsActive)
                {
                    return ApiResponse<bool>.ErrorResponse("Invalid or inactive customer");
                }

                var product = await _productRepository.GetByIdAsync(request.ProductId);
                if (product == null)
                {
                    return ApiResponse<bool>.ErrorResponse("Invalid product");
                }

                // Update order
                existingOrder.OrderDate = request.OrderDate;
                existingOrder.DueDate = request.DueDate;
                existingOrder.AdjustedDueDate = request.AdjustedDueDate;
                existingOrder.CustomerId = request.CustomerId;
                existingOrder.ProductId = request.ProductId;
                existingOrder.Quantity = request.Quantity;
                existingOrder.Status = request.Status;
                existingOrder.Priority = request.Priority;
                if (!string.IsNullOrEmpty(request.PlanningStatus))  // ✅ Update planning status if provided
                {
                    existingOrder.PlanningStatus = request.PlanningStatus;
                }
                existingOrder.OrderValue = request.OrderValue;
                existingOrder.AdvancePayment = request.AdvancePayment;
                existingOrder.BalancePayment = request.BalancePayment;
                existingOrder.DelayReason = request.DelayReason;
                existingOrder.UpdatedBy = request.UpdatedBy;
                existingOrder.UpdatedAt = DateTime.UtcNow;

                var success = await _orderRepository.UpdateWithVersionCheckAsync(existingOrder);

                if (!success)
                {
                    return ApiResponse<bool>.ErrorResponse("Failed to update order. Please try again.");
                }

                return ApiResponse<bool>.SuccessResponse(true, "Order updated successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error updating order: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> DeleteOrderAsync(int id)
        {
            try
            {
                var order = await _orderRepository.GetByIdAsync(id);
                if (order == null)
                {
                    return ApiResponse<bool>.ErrorResponse("Order not found");
                }

                // Business Rule: Cannot delete orders that are in progress or completed
                if (order.Status == "In Progress" || order.Status == "Completed")
                {
                    return ApiResponse<bool>.ErrorResponse(
                        $"Cannot delete order with status '{order.Status}'");
                }

                var success = await _orderRepository.DeleteAsync(id);

                if (!success)
                {
                    return ApiResponse<bool>.ErrorResponse("Failed to delete order");
                }

                return ApiResponse<bool>.SuccessResponse(true, "Order deleted successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error deleting order: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> UpdateDrawingReviewStatusAsync(UpdateDrawingReviewRequest request)
        {
            try
            {
                var order = await _orderRepository.GetByIdAsync(request.OrderId);
                if (order == null)
                {
                    return ApiResponse<bool>.ErrorResponse("Order not found");
                }

                // Validate status (must match frontend enum: Pending, In Review, Approved, Needs Revision)
                var validStatuses = new[] { "Pending", "In Review", "Approved", "Needs Revision" };
                if (!validStatuses.Contains(request.Status))
                {
                    return ApiResponse<bool>.ErrorResponse($"Invalid drawing review status: {request.Status}");
                }

                var success = await _orderRepository.UpdateDrawingReviewStatusAsync(
                    request.OrderId,
                    request.Status,
                    request.ReviewedBy,
                    request.Notes
                );

                if (!success)
                {
                    return ApiResponse<bool>.ErrorResponse("Failed to update drawing review status");
                }

                return ApiResponse<bool>.SuccessResponse(true, "Drawing review status updated successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error updating drawing review: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> ApproveDrawingReviewAsync(int orderId, string reviewedBy, string? notes, int linkedProductTemplateId)
        {
            try
            {
                var order = await _orderRepository.GetByIdAsync(orderId);
                if (order == null)
                {
                    return ApiResponse<bool>.ErrorResponse("Order not found");
                }

                // Business Rule: Cannot approve if already approved
                if (order.DrawingReviewStatus == "Approved")
                {
                    return ApiResponse<bool>.ErrorResponse("Drawing review is already approved");
                }

                // Link the product template
                order.LinkedProductTemplateId = linkedProductTemplateId;
                order.DrawingReviewStatus = "Approved";
                order.DrawingReviewedBy = reviewedBy;
                order.DrawingReviewedAt = DateTime.UtcNow;
                order.DrawingReviewNotes = notes;
                order.UpdatedAt = DateTime.UtcNow;

                var success = await _orderRepository.UpdateAsync(order);

                if (!success)
                {
                    return ApiResponse<bool>.ErrorResponse("Failed to approve drawing review");
                }

                return ApiResponse<bool>.SuccessResponse(true, "Drawing review approved and product template linked successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error approving drawing review: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> RejectDrawingReviewAsync(int orderId, string reviewedBy, string reason)
        {
            var request = new UpdateDrawingReviewRequest
            {
                OrderId = orderId,
                Status = "Needs Revision",
                ReviewedBy = reviewedBy,
                Notes = reason
            };

            return await UpdateDrawingReviewStatusAsync(request);
        }

        public async Task<ApiResponse<bool>> UpdateOrderStatusAsync(int orderId, string status)
        {
            try
            {
                var success = await _orderRepository.UpdateStatusAsync(orderId, status);
                if (!success)
                {
                    return ApiResponse<bool>.ErrorResponse("Failed to update order status");
                }

                return ApiResponse<bool>.SuccessResponse(true, "Order status updated successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error updating status: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> UpdatePlanningStatusAsync(int orderId, string status)
        {
            try
            {
                var success = await _orderRepository.UpdatePlanningStatusAsync(orderId, status);
                if (!success)
                {
                    return ApiResponse<bool>.ErrorResponse("Failed to update planning status");
                }

                return ApiResponse<bool>.SuccessResponse(true, "Planning status updated successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error updating planning status: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<OrderResponse>>> GetPendingDrawingReviewAsync()
        {
            try
            {
                var orders = await _orderRepository.GetPendingDrawingReviewAsync();
                var responses = new List<OrderResponse>();

                foreach (var order in orders)
                {
                    responses.Add(await MapToResponseAsync(order));
                }

                return ApiResponse<IEnumerable<OrderResponse>>.SuccessResponse(responses);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<OrderResponse>>.ErrorResponse($"Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<OrderResponse>>> GetReadyForPlanningAsync()
        {
            try
            {
                var orders = await _orderRepository.GetReadyForPlanningAsync();
                var responses = new List<OrderResponse>();

                foreach (var order in orders)
                {
                    responses.Add(await MapToResponseAsync(order));
                }

                return ApiResponse<IEnumerable<OrderResponse>>.SuccessResponse(responses);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<OrderResponse>>.ErrorResponse($"Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<OrderResponse>>> GetInProgressOrdersAsync()
        {
            try
            {
                var orders = await _orderRepository.GetInProgressOrdersAsync();
                var responses = new List<OrderResponse>();

                foreach (var order in orders)
                {
                    responses.Add(await MapToResponseAsync(order));
                }

                return ApiResponse<IEnumerable<OrderResponse>>.SuccessResponse(responses);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<OrderResponse>>.ErrorResponse($"Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<OrderResponse>>> GetDelayedOrdersAsync()
        {
            try
            {
                var orders = await _orderRepository.GetDelayedOrdersAsync();
                var responses = new List<OrderResponse>();

                foreach (var order in orders)
                {
                    responses.Add(await MapToResponseAsync(order));
                }

                return ApiResponse<IEnumerable<OrderResponse>>.SuccessResponse(responses);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<OrderResponse>>.ErrorResponse($"Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> CanGenerateJobCardsAsync(int orderId)
        {
            try
            {
                var order = await _orderRepository.GetByIdAsync(orderId);
                if (order == null)
                {
                    return ApiResponse<bool>.ErrorResponse("Order not found");
                }

                // GATE: Drawing Review must be approved
                if (order.DrawingReviewStatus != "Approved")
                {
                    return ApiResponse<bool>.ErrorResponse(
                        "Cannot generate job cards: Drawing review must be approved first");
                }

                return ApiResponse<bool>.SuccessResponse(true, "Order is ready for job card generation");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<string>> GenerateOrderNoAsync()
        {
            try
            {
                var orderNo = await GenerateOrderNoInternalAsync();
                return ApiResponse<string>.SuccessResponse(orderNo);
            }
            catch (Exception ex)
            {
                return ApiResponse<string>.ErrorResponse($"Error generating order number: {ex.Message}");
            }
        }

        // Helper Methods

        private async Task<string> GenerateOrderNoInternalAsync()
        {
            // Format: ORD-YYYYMM-NNNN
            var prefix = "ORD";
            var yearMonth = DateTime.UtcNow.ToString("yyyyMM");

            // Get all orders for current month to find next number
            var allOrders = await _orderRepository.GetAllAsync();
            var currentMonthOrders = allOrders
                .Where(o => o.OrderNo.StartsWith($"{prefix}-{yearMonth}"))
                .ToList();

            var nextNumber = currentMonthOrders.Count + 1;
            return $"{prefix}-{yearMonth}-{nextNumber:D4}";
        }

        private async Task<OrderResponse> MapToResponseAsync(Order order)
        {
            // Get customer and product details for enriched response
            var customer = await _customerRepository.GetByIdAsync(order.CustomerId);
            var product = await _productRepository.GetByIdAsync(order.ProductId);

            var daysUntilDue = (order.DueDate - DateTime.UtcNow).Days;
            var isDelayed = order.Status != "Completed" && order.Status != "Cancelled" && daysUntilDue < 0;

            var completionPercentage = order.OriginalQuantity > 0
                ? (decimal)order.QtyCompleted / order.OriginalQuantity * 100
                : 0;

            return new OrderResponse
            {
                Id = order.Id,
                OrderNo = order.OrderNo,
                OrderDate = order.OrderDate,
                DueDate = order.DueDate,
                AdjustedDueDate = order.AdjustedDueDate,

                CustomerId = order.CustomerId,
                CustomerName = customer?.CustomerName,
                CustomerCode = customer?.CustomerCode,

                ProductId = order.ProductId,
                ProductName = product?.ModelName,
                ProductCode = product?.PartCode,

                Quantity = order.Quantity,
                OriginalQuantity = order.OriginalQuantity,
                QtyCompleted = order.QtyCompleted,
                QtyRejected = order.QtyRejected,
                QtyInProgress = order.QtyInProgress,
                QtyScrap = order.QtyScrap,

                Status = order.Status,
                Priority = order.Priority,
                PlanningStatus = order.PlanningStatus,

                OrderSource = order.OrderSource,
                AgentCustomerId = order.AgentCustomerId,
                AgentCommission = order.AgentCommission,
                SchedulingStrategy = order.SchedulingStrategy,

                DrawingReviewStatus = order.DrawingReviewStatus,
                DrawingReviewedBy = order.DrawingReviewedBy,
                DrawingReviewedAt = order.DrawingReviewedAt,
                DrawingReviewNotes = order.DrawingReviewNotes,

                PrimaryDrawingId = order.PrimaryDrawingId,
                DrawingSource = order.DrawingSource,
                LinkedProductTemplateId = order.LinkedProductTemplateId,

                CustomerMachine = order.CustomerMachine,
                MaterialGradeRemark = order.MaterialGradeRemark,

                CurrentProcess = order.CurrentProcess,
                CurrentMachine = order.CurrentMachine,
                CurrentOperator = order.CurrentOperator,
                ProductionStartDate = order.ProductionStartDate,
                ProductionEndDate = order.ProductionEndDate,

                DelayReason = order.DelayReason,
                RescheduleCount = order.RescheduleCount,
                IsDelayed = isDelayed,
                DaysUntilDue = daysUntilDue,

                MaterialGradeApproved = order.MaterialGradeApproved,
                MaterialGradeApprovalDate = order.MaterialGradeApprovalDate,
                MaterialGradeApprovedBy = order.MaterialGradeApprovedBy,

                OrderValue = order.OrderValue,
                AdvancePayment = order.AdvancePayment,
                BalancePayment = order.BalancePayment,

                CompletionPercentage = completionPercentage,

                CreatedAt = order.CreatedAt,
                CreatedBy = order.CreatedBy,
                UpdatedAt = order.UpdatedAt,
                UpdatedBy = order.UpdatedBy,
                Version = order.Version
            };
        }
    }
}

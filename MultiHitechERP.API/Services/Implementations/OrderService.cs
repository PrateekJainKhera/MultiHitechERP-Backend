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
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IProductRepository _productRepository;

        public OrderService(
            IOrderRepository orderRepository,
            IOrderItemRepository orderItemRepository,
            ICustomerRepository customerRepository,
            IProductRepository productRepository)
        {
            _orderRepository = orderRepository;
            _orderItemRepository = orderItemRepository;
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
                // ===== COMMON VALIDATIONS =====

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

                // Business Rule 2: Validate Order Source
                var validOrderSources = new[] { "Direct", "Agent", "Dealer", "Distributor" };
                if (!validOrderSources.Contains(request.OrderSource))
                {
                    return ApiResponse<int>.ErrorResponse($"Invalid order source. Must be one of: {string.Join(", ", validOrderSources)}");
                }

                // Business Rule 3: Validate Agent Customer if provided
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

                // Business Rule 4: Validate Scheduling Strategy
                var validStrategies = new[] { "Due Date", "Priority Flag", "Customer Importance", "Resource Availability" };
                if (!validStrategies.Contains(request.SchedulingStrategy))
                {
                    return ApiResponse<int>.ErrorResponse($"Invalid scheduling strategy. Must be one of: {string.Join(", ", validStrategies)}");
                }

                // Generate Order Number
                var orderNo = await GenerateOrderNoInternalAsync();

                // ===== MULTI-PRODUCT ORDER (NEW FLOW) =====
                if (request.Items != null && request.Items.Any())
                {
                    // Validate Items
                    if (request.Items.Count == 0)
                    {
                        return ApiResponse<int>.ErrorResponse("At least one item is required");
                    }

                    // Validate all items and collect products
                    var productIds = new HashSet<int>();
                    var products = new List<Models.Masters.Product>();

                    foreach (var item in request.Items)
                    {
                        // Check for duplicate products
                        if (productIds.Contains(item.ProductId))
                        {
                            return ApiResponse<int>.ErrorResponse($"Duplicate product detected. Same product cannot be added multiple times in one order.");
                        }
                        productIds.Add(item.ProductId);

                        // Validate product exists
                        var product = await _productRepository.GetByIdAsync(item.ProductId);
                        if (product == null)
                        {
                            return ApiResponse<int>.ErrorResponse($"Product with ID {item.ProductId} not found");
                        }
                        products.Add(product);

                        // Validate quantity
                        if (item.Quantity <= 0)
                        {
                            return ApiResponse<int>.ErrorResponse($"Quantity must be greater than 0 for product {product.PartCode}");
                        }

                        // Validate due date
                        if (item.DueDate <= DateTime.UtcNow)
                        {
                            return ApiResponse<int>.ErrorResponse($"Due date must be in the future for product {product.PartCode}");
                        }

                        // Validate priority
                        var validPriorities = new[] { "Low", "Medium", "High", "Urgent" };
                        if (!validPriorities.Contains(item.Priority))
                        {
                            return ApiResponse<int>.ErrorResponse($"Invalid priority for product {product.PartCode}. Must be one of: {string.Join(", ", validPriorities)}");
                        }
                    }

                    // Check if all products have approved drawings
                    bool allProductDrawingsApproved = products.All(p => p.DrawingReviewStatus == "Approved");
                    string orderDrawingReviewStatus = allProductDrawingsApproved ? "Approved" : "Pending";

                    // Create Order (Header)
                    var order = new Order
                    {
                        OrderNo = orderNo,
                        OrderDate = request.OrderDate ?? DateTime.UtcNow,
                        CustomerId = request.CustomerId,
                        CustomerName = customer.CustomerName,
                        Status = "Pending",
                        PlanningStatus = "Not Planned",
                        DrawingReviewStatus = orderDrawingReviewStatus,
                        OrderSource = request.OrderSource,
                        AgentCustomerId = request.AgentCustomerId,
                        AgentCommission = request.AgentCommission,
                        SchedulingStrategy = request.SchedulingStrategy,
                        CustomerMachine = request.CustomerMachine,
                        OrderValue = request.OrderValue,
                        AdvancePayment = request.AdvancePayment,
                        BalancePayment = request.OrderValue.HasValue ? request.OrderValue - (request.AdvancePayment ?? 0) : null,
                        CreatedBy = request.CreatedBy,
                        Version = 1,

                        // Legacy fields kept for compatibility - will be aggregate of first item or defaults
                        ProductId = request.Items.First().ProductId,
                        DueDate = request.Items.First().DueDate,
                        Priority = request.Items.First().Priority,
                        Quantity = request.Items.Sum(i => i.Quantity),
                        OriginalQuantity = request.Items.Sum(i => i.Quantity)
                    };

                    var orderId = await _orderRepository.InsertAsync(order);

                    // Create OrderItems with sequences (A, B, C...)
                    var itemSequence = 'A';
                    var orderItems = new List<OrderItem>();

                    foreach (var itemRequest in request.Items)
                    {
                        var product = await _productRepository.GetByIdAsync(itemRequest.ProductId);

                        var orderItem = new OrderItem
                        {
                            OrderId = orderId,
                            ItemSequence = itemSequence.ToString(),
                            ProductId = itemRequest.ProductId,
                            ProductName = product?.PartCode,
                            Quantity = itemRequest.Quantity,
                            QtyCompleted = 0,
                            QtyRejected = 0,
                            QtyInProgress = 0,
                            QtyScrap = 0,
                            DueDate = itemRequest.DueDate,
                            Priority = itemRequest.Priority,
                            Status = "Pending",
                            PlanningStatus = "Not Planned",
                            PrimaryDrawingId = itemRequest.PrimaryDrawingId,
                            DrawingSource = itemRequest.DrawingSource,
                            LinkedProductTemplateId = itemRequest.LinkedProductTemplateId,
                            MaterialGradeRemark = itemRequest.MaterialGradeRemark,
                            CreatedAt = DateTime.UtcNow,
                            CreatedBy = request.CreatedBy ?? "System"
                        };

                        orderItems.Add(orderItem);
                        itemSequence++;
                    }

                    // Insert all items
                    await _orderItemRepository.CreateBatchAsync(orderItems);

                    return ApiResponse<int>.SuccessResponse(orderId, $"Multi-product order {orderNo} created successfully with {request.Items.Count} items");
                }
                // ===== LEGACY SINGLE-PRODUCT ORDER (BACKWARD COMPATIBLE) =====
                else
                {
                    // Validate legacy fields
                    if (!request.ProductId.HasValue || request.ProductId.Value <= 0)
                    {
                        return ApiResponse<int>.ErrorResponse("Product ID is required for single-product orders");
                    }
                    if (!request.Quantity.HasValue || request.Quantity.Value <= 0)
                    {
                        return ApiResponse<int>.ErrorResponse("Quantity must be greater than 0");
                    }
                    if (!request.DueDate.HasValue || request.DueDate.Value <= DateTime.UtcNow)
                    {
                        return ApiResponse<int>.ErrorResponse("Due date is required and must be in the future");
                    }

                    // Validate product exists
                    var product = await _productRepository.GetByIdAsync(request.ProductId.Value);
                    if (product == null)
                    {
                        return ApiResponse<int>.ErrorResponse("Product not found");
                    }

                    // Check if product drawing is approved
                    string orderDrawingReviewStatus = product.DrawingReviewStatus == "Approved" ? "Approved" : "Pending";

                    // Create Order (Legacy format)
                    var order = new Order
                    {
                        OrderNo = orderNo,
                        OrderDate = request.OrderDate ?? DateTime.UtcNow,
                        DueDate = request.DueDate.Value,
                        CustomerId = request.CustomerId,
                        CustomerName = customer.CustomerName,
                        ProductId = request.ProductId.Value,
                        ProductName = product.PartCode,
                        Quantity = request.Quantity.Value,
                        OriginalQuantity = request.Quantity.Value,
                        Status = "Pending",
                        Priority = request.Priority ?? "Medium",
                        PlanningStatus = "Not Planned",
                        DrawingReviewStatus = orderDrawingReviewStatus,
                        OrderSource = request.OrderSource,
                        AgentCustomerId = request.AgentCustomerId,
                        AgentCommission = request.AgentCommission,
                        SchedulingStrategy = request.SchedulingStrategy,
                        PrimaryDrawingId = request.PrimaryDrawingId,
                        DrawingSource = request.DrawingSource,
                        DrawingReviewNotes = request.DrawingNotes,
                        LinkedProductTemplateId = request.LinkedProductTemplateId,
                        CustomerMachine = request.CustomerMachine,
                        MaterialGradeRemark = request.MaterialGradeRemark,
                        OrderValue = request.OrderValue,
                        AdvancePayment = request.AdvancePayment,
                        BalancePayment = request.OrderValue.HasValue ? request.OrderValue - (request.AdvancePayment ?? 0) : null,
                        CreatedBy = request.CreatedBy,
                        Version = 1
                    };

                    var orderId = await _orderRepository.InsertAsync(order);

                    // Create single OrderItem with sequence 'A' for backward compatibility
                    var orderItem = new OrderItem
                    {
                        OrderId = orderId,
                        ItemSequence = "A",
                        ProductId = request.ProductId.Value,
                        ProductName = product.PartCode,
                        Quantity = request.Quantity.Value,
                        QtyCompleted = 0,
                        QtyRejected = 0,
                        QtyInProgress = 0,
                        QtyScrap = 0,
                        DueDate = request.DueDate.Value,
                        Priority = request.Priority ?? "Medium",
                        Status = "Pending",
                        PlanningStatus = "Not Planned",
                        PrimaryDrawingId = request.PrimaryDrawingId,
                        DrawingSource = request.DrawingSource,
                        LinkedProductTemplateId = request.LinkedProductTemplateId,
                        MaterialGradeRemark = request.MaterialGradeRemark,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = request.CreatedBy ?? "System"
                    };

                    await _orderItemRepository.CreateAsync(orderItem);

                    return ApiResponse<int>.SuccessResponse(orderId, $"Order {orderNo} created successfully");
                }
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

            // Load OrderItems for multi-product support
            var orderItems = await _orderItemRepository.GetByOrderIdAsync(order.Id);
            List<OrderItemResponse>? itemResponses = null;

            if (orderItems != null && orderItems.Any())
            {
                itemResponses = new List<OrderItemResponse>();
                foreach (var item in orderItems)
                {
                    var itemProduct = await _productRepository.GetByIdAsync(item.ProductId);
                    itemResponses.Add(new OrderItemResponse
                    {
                        Id = item.Id,
                        OrderId = item.OrderId,
                        ItemSequence = item.ItemSequence,
                        ProductId = item.ProductId,
                        ProductName = itemProduct?.ModelName,
                        PartCode = itemProduct?.PartCode,
                        Quantity = item.Quantity,
                        OriginalQuantity = item.Quantity, // Use Quantity as OriginalQuantity
                        QtyCompleted = item.QtyCompleted,
                        QtyRejected = item.QtyRejected,
                        QtyInProgress = item.QtyInProgress,
                        QtyScrap = item.QtyScrap,
                        DueDate = item.DueDate,
                        AdjustedDueDate = null, // OrderItem doesn't track adjusted due date
                        Priority = item.Priority,
                        Status = item.Status,
                        PrimaryDrawingId = item.PrimaryDrawingId,
                        LinkedProductTemplateId = item.LinkedProductTemplateId,
                        MaterialGradeApproved = item.MaterialGradeApproved,
                        MaterialGradeApprovalDate = item.MaterialGradeApprovalDate,
                        MaterialGradeApprovedBy = item.MaterialGradeApprovedBy,
                        MaterialGradeRemark = item.MaterialGradeRemark,
                        CreatedAt = item.CreatedAt,
                        CreatedBy = item.CreatedBy,
                        UpdatedAt = item.UpdatedAt,
                        UpdatedBy = item.UpdatedBy
                    });
                }
            }

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
                Version = order.Version,

                // Multi-Product Support
                Items = itemResponses
            };
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MultiHitechERP.API.DTOs.Response;
using MultiHitechERP.API.Models.Dispatch;
using MultiHitechERP.API.DTOs.Request;
using MultiHitechERP.API.Repositories.Interfaces;
using MultiHitechERP.API.Services.Interfaces;

namespace MultiHitechERP.API.Services.Implementations
{
    /// <summary>
    /// DispatchService implementation with comprehensive dispatch logic
    /// Handles delivery challan creation, dispatch, and delivery tracking
    /// </summary>
    public class DispatchService : IDispatchService
    {
        private readonly IDeliveryChallanRepository _challanRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderItemRepository _orderItemRepository;

        public DispatchService(
            IDeliveryChallanRepository challanRepository,
            IOrderRepository orderRepository,
            IOrderItemRepository orderItemRepository)
        {
            _challanRepository = challanRepository;
            _orderRepository = orderRepository;
            _orderItemRepository = orderItemRepository;
        }

        public async Task<ApiResponse<DeliveryChallan>> GetByIdAsync(int id)
        {
            var challan = await _challanRepository.GetByIdAsync(id);
            if (challan == null)
                return ApiResponse<DeliveryChallan>.ErrorResponse("Delivery challan not found");

            return ApiResponse<DeliveryChallan>.SuccessResponse(challan);
        }

        public async Task<ApiResponse<DeliveryChallan>> GetByChallanNoAsync(string challanNo)
        {
            if (string.IsNullOrWhiteSpace(challanNo))
                return ApiResponse<DeliveryChallan>.ErrorResponse("Challan number is required");

            var challan = await _challanRepository.GetByChallanNoAsync(challanNo);
            if (challan == null)
                return ApiResponse<DeliveryChallan>.ErrorResponse("Delivery challan not found");

            return ApiResponse<DeliveryChallan>.SuccessResponse(challan);
        }

        public async Task<ApiResponse<IEnumerable<DeliveryChallan>>> GetAllAsync()
        {
            var challans = await _challanRepository.GetAllAsync();
            return ApiResponse<IEnumerable<DeliveryChallan>>.SuccessResponse(challans);
        }

        public async Task<ApiResponse<int>> CreateChallanAsync(DeliveryChallan challan)
        {
            var id = await _challanRepository.InsertAsync(challan);
            return ApiResponse<int>.SuccessResponse(id, "Delivery challan created successfully");
        }

        public async Task<ApiResponse<bool>> UpdateChallanAsync(DeliveryChallan challan)
        {
            var existing = await _challanRepository.GetByIdAsync(challan.Id);
            if (existing == null)
                return ApiResponse<bool>.ErrorResponse("Delivery challan not found");

            var success = await _challanRepository.UpdateAsync(challan);
            if (!success)
                return ApiResponse<bool>.ErrorResponse("Failed to update delivery challan");

            return ApiResponse<bool>.SuccessResponse(true, "Delivery challan updated successfully");
        }

        public async Task<ApiResponse<bool>> DeleteChallanAsync(int id)
        {
            var existing = await _challanRepository.GetByIdAsync(id);
            if (existing == null)
                return ApiResponse<bool>.ErrorResponse("Delivery challan not found");

            var success = await _challanRepository.DeleteAsync(id);
            if (!success)
                return ApiResponse<bool>.ErrorResponse("Failed to delete delivery challan");

            return ApiResponse<bool>.SuccessResponse(true, "Delivery challan deleted successfully");
        }

        public async Task<ApiResponse<IEnumerable<DeliveryChallan>>> GetByOrderIdAsync(int orderId)
        {
            var challans = await _challanRepository.GetByOrderIdAsync(orderId);
            return ApiResponse<IEnumerable<DeliveryChallan>>.SuccessResponse(challans);
        }

        public async Task<ApiResponse<IEnumerable<DeliveryChallan>>> GetByCustomerIdAsync(int customerId)
        {
            var challans = await _challanRepository.GetByCustomerIdAsync(customerId);
            return ApiResponse<IEnumerable<DeliveryChallan>>.SuccessResponse(challans);
        }

        public async Task<ApiResponse<IEnumerable<DeliveryChallan>>> GetByStatusAsync(string status)
        {
            if (string.IsNullOrWhiteSpace(status))
                return ApiResponse<IEnumerable<DeliveryChallan>>.ErrorResponse("Status is required");

            var challans = await _challanRepository.GetByStatusAsync(status);
            return ApiResponse<IEnumerable<DeliveryChallan>>.SuccessResponse(challans);
        }

        public async Task<ApiResponse<IEnumerable<DeliveryChallan>>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
                return ApiResponse<IEnumerable<DeliveryChallan>>.ErrorResponse("Start date must be before end date");

            var challans = await _challanRepository.GetByDateRangeAsync(startDate, endDate);
            return ApiResponse<IEnumerable<DeliveryChallan>>.SuccessResponse(challans);
        }

        public async Task<ApiResponse<IEnumerable<DeliveryChallan>>> GetByVehicleNumberAsync(string vehicleNumber)
        {
            if (string.IsNullOrWhiteSpace(vehicleNumber))
                return ApiResponse<IEnumerable<DeliveryChallan>>.ErrorResponse("Vehicle number is required");

            var challans = await _challanRepository.GetByVehicleNumberAsync(vehicleNumber);
            return ApiResponse<IEnumerable<DeliveryChallan>>.SuccessResponse(challans);
        }

        public async Task<ApiResponse<int>> CreateDispatchChallanAsync(
            int orderId,
            int quantityDispatched,
            string deliveryAddress,
            string? transportMode,
            string? vehicleNumber,
            string? driverName,
            string? driverContact)
        {
            // Validate order exists
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null)
                return ApiResponse<int>.ErrorResponse("Order not found");

            if (order.Status != "Completed" && order.Status != "QC Approved")
                return ApiResponse<int>.ErrorResponse($"Cannot create dispatch challan - order status is '{order.Status}'");

            // Validate quantity
            if (quantityDispatched <= 0)
                return ApiResponse<int>.ErrorResponse("Quantity dispatched must be greater than zero");

            if (quantityDispatched > order.Quantity)
                return ApiResponse<int>.ErrorResponse($"Quantity dispatched ({quantityDispatched}) cannot exceed order quantity ({order.Quantity})");

            // Generate challan number
            var challanNo = $"DC-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";

            // Create delivery challan
            var challan = new DeliveryChallan
            {
                ChallanNo = challanNo,
                ChallanDate = DateTime.UtcNow,
                OrderId = orderId,
                OrderNo = order.OrderNo,
                CustomerId = order.CustomerId,
                ProductId = order.ProductId,
                QuantityDispatched = quantityDispatched,
                DeliveryAddress = deliveryAddress,
                TransportMode = transportMode,
                VehicleNumber = vehicleNumber,
                DriverName = driverName,
                DriverContact = driverContact,
                Status = "Pending"
            };

            var id = await _challanRepository.InsertAsync(challan);

            return ApiResponse<int>.SuccessResponse(id, $"Delivery challan {challanNo} created successfully");
        }

        public async Task<ApiResponse<bool>> DispatchChallanAsync(int id)
        {
            var challan = await _challanRepository.GetByIdAsync(id);
            if (challan == null)
                return ApiResponse<bool>.ErrorResponse("Delivery challan not found");

            if (challan.Status != "Pending")
                return ApiResponse<bool>.ErrorResponse($"Cannot dispatch - challan status is '{challan.Status}'");

            // Validate delivery details
            if (string.IsNullOrWhiteSpace(challan.DeliveryAddress))
                return ApiResponse<bool>.ErrorResponse("Delivery address is required");

            var success = await _challanRepository.DispatchChallanAsync(id, DateTime.UtcNow);
            if (!success)
                return ApiResponse<bool>.ErrorResponse("Failed to dispatch challan");

            return ApiResponse<bool>.SuccessResponse(true, "Challan dispatched successfully");
        }

        public async Task<ApiResponse<bool>> DeliverChallanAsync(int id, string receivedBy)
        {
            var challan = await _challanRepository.GetByIdAsync(id);
            if (challan == null)
                return ApiResponse<bool>.ErrorResponse("Delivery challan not found");

            if (challan.Status != "Dispatched")
                return ApiResponse<bool>.ErrorResponse($"Cannot mark as delivered - challan status is '{challan.Status}'");

            if (string.IsNullOrWhiteSpace(receivedBy))
                return ApiResponse<bool>.ErrorResponse("Receiver name is required");

            var success = await _challanRepository.DeliverChallanAsync(id, DateTime.UtcNow, receivedBy);
            if (!success)
                return ApiResponse<bool>.ErrorResponse("Failed to mark challan as delivered");

            return ApiResponse<bool>.SuccessResponse(true, "Challan marked as delivered successfully");
        }

        public async Task<ApiResponse<bool>> UpdateStatusAsync(int id, string status)
        {
            var challan = await _challanRepository.GetByIdAsync(id);
            if (challan == null)
                return ApiResponse<bool>.ErrorResponse("Delivery challan not found");

            if (string.IsNullOrWhiteSpace(status))
                return ApiResponse<bool>.ErrorResponse("Status is required");

            var success = await _challanRepository.UpdateStatusAsync(id, status);
            if (!success)
                return ApiResponse<bool>.ErrorResponse("Failed to update challan status");

            return ApiResponse<bool>.SuccessResponse(true, "Challan status updated successfully");
        }

        public async Task<ApiResponse<IEnumerable<DeliveryChallan>>> GetPendingChallansAsync()
        {
            var challans = await _challanRepository.GetPendingChallansAsync();
            return ApiResponse<IEnumerable<DeliveryChallan>>.SuccessResponse(challans);
        }

        public async Task<ApiResponse<IEnumerable<DeliveryChallan>>> GetDispatchedChallansAsync()
        {
            var challans = await _challanRepository.GetDispatchedChallansAsync();
            return ApiResponse<IEnumerable<DeliveryChallan>>.SuccessResponse(challans);
        }

        public async Task<ApiResponse<List<ReadyToDispatchItem>>> GetReadyToDispatchAsync()
        {
            var items = await _orderItemRepository.GetReadyToDispatchAsync();
            return ApiResponse<List<ReadyToDispatchItem>>.SuccessResponse(items);
        }

        // Dispatch several ready items for ONE customer on a single consolidated challan / bill.
        public async Task<ApiResponse<int>> ConsolidatedDispatchAsync(ConsolidatedDispatchRequest request, string? invoiceDocument)
        {
            if (request == null || request.Items == null || request.Items.Count == 0)
                return ApiResponse<int>.ErrorResponse("Select at least one item to dispatch");

            if (request.CustomerId <= 0)
                return ApiResponse<int>.ErrorResponse("A customer must be selected");

            // Load the ready-to-dispatch list once → validate & enrich each line from it.
            var ready = (await _orderItemRepository.GetReadyToDispatchAsync())
                .ToDictionary(r => r.OrderItemId, r => r);

            var lines = new List<(ConsolidatedDispatchLine req, ReadyToDispatchItem ready)>();
            foreach (var line in request.Items)
            {
                if (line.QtyToDispatch <= 0)
                    return ApiResponse<int>.ErrorResponse("Quantity to dispatch must be greater than 0");
                if (!ready.TryGetValue(line.OrderItemId, out var r))
                    return ApiResponse<int>.ErrorResponse($"Order item {line.OrderItemId} is not ready to dispatch");
                if (r.CustomerId != request.CustomerId)
                    return ApiResponse<int>.ErrorResponse($"{r.OrderNo}-{r.ItemSequence} belongs to a different customer — one bill must be for one customer");
                if (line.QtyToDispatch > r.QtyPendingDispatch)
                    return ApiResponse<int>.ErrorResponse($"{r.OrderNo}-{r.ItemSequence}: cannot dispatch {line.QtyToDispatch}, only {r.QtyPendingDispatch} pending");
                lines.Add((line, r));
            }

            var first = lines[0].ready;
            var distinctOrders = lines.Select(l => l.ready.OrderId).Distinct().Count();
            var totalQty = lines.Sum(l => l.req.QtyToDispatch);
            var challanNo = $"DC-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";

            var challan = new DeliveryChallan
            {
                ChallanNo = challanNo,
                ChallanDate = request.DispatchDate,
                OrderId = first.OrderId,                        // representative (header requires a value)
                OrderNo = distinctOrders == 1 ? first.OrderNo : $"{distinctOrders} orders",
                CustomerId = request.CustomerId,
                CustomerName = first.CustomerName,
                ProductId = 0,                                  // 0 = multiple products (see line items)
                ProductName = $"{lines.Count} item(s)",
                QuantityDispatched = totalQty,
                DeliveryAddress = request.DeliveryAddress,
                TransportMode = request.TransportMode,
                VehicleNumber = request.VehicleNumber,
                DriverName = request.DriverName,
                DriverContact = request.DriverContact,
                InvoiceNo = request.InvoiceNo,
                InvoiceDate = request.InvoiceDate,
                InvoiceDocument = invoiceDocument,
                Status = "Dispatched",
                DispatchedAt = DateTime.UtcNow,
                Remarks = request.Remarks,
                IsConsolidated = true,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = string.IsNullOrWhiteSpace(request.CreatedBy) ? "Admin" : request.CreatedBy,
            };

            var challanId = await _challanRepository.InsertAsync(challan);

            foreach (var (req, r) in lines)
            {
                await _challanRepository.InsertChallanItemAsync(new DeliveryChallanItem
                {
                    ChallanId = challanId,
                    OrderId = r.OrderId,
                    OrderItemId = r.OrderItemId,
                    OrderNo = r.OrderNo,
                    ItemSequence = r.ItemSequence,
                    ProductId = r.ProductId,
                    ProductCode = r.PartCode,
                    ProductName = r.ProductName,
                    Quantity = req.QtyToDispatch,
                    UOM = "pcs",
                    MachineModel = r.MachineModel,
                    RollerType = r.RollerType,
                    NumberOfTeeth = r.NumberOfTeeth,
                });
                await _orderItemRepository.UpdateQtyDispatchedAsync(r.OrderItemId, req.QtyToDispatch);
            }

            return ApiResponse<int>.SuccessResponse(challanId,
                $"Dispatched {lines.Count} item(s) across {distinctOrders} order(s) on challan {challanNo}");
        }

        public async Task<ApiResponse<IEnumerable<DeliveryChallanItem>>> GetChallanItemsAsync(int challanId)
        {
            var items = await _challanRepository.GetChallanItemsAsync(challanId);
            return ApiResponse<IEnumerable<DeliveryChallanItem>>.SuccessResponse(items);
        }

    }
}
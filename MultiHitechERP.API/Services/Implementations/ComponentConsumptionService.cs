using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MultiHitechERP.API.DTOs.Request;
using MultiHitechERP.API.DTOs.Response;
using MultiHitechERP.API.Models.Stores;
using MultiHitechERP.API.Repositories.Interfaces;
using MultiHitechERP.API.Services.Interfaces;

namespace MultiHitechERP.API.Services.Implementations
{
    public class ComponentConsumptionService : IComponentConsumptionService
    {
        private readonly IOrderComponentRepository _ocRepo;
        private readonly IShopFloorComponentStockRepository _floorRepo;

        public ComponentConsumptionService(
            IOrderComponentRepository ocRepo,
            IShopFloorComponentStockRepository floorRepo)
        {
            _ocRepo = ocRepo;
            _floorRepo = floorRepo;
        }

        // Reserve floor stock for an order-item's planned components (idempotent: replaces prior reservation).
        public async Task<ApiResponse<bool>> ReserveForOrderItemAsync(ReserveComponentsRequest request)
        {
            try
            {
                // Release the previous (unconsumed) reservation for this order-item, then re-reserve.
                var existing = (await _ocRepo.GetReservedForOrderItemAsync(request.OrderId, request.OrderItemId)).ToList();
                foreach (var e in existing)
                    await _floorRepo.AdjustReservedAsync(e.ComponentId, -e.ReservedQty);
                await _ocRepo.DeleteReservedForOrderItemAsync(request.OrderId, request.OrderItemId);

                foreach (var c in request.Components.Where(c => c.ComponentId > 0 && c.Quantity > 0))
                {
                    await _ocRepo.InsertAsync(new OrderComponent
                    {
                        OrderId = request.OrderId,
                        OrderItemId = request.OrderItemId,
                        OrderNo = request.OrderNo,
                        ComponentId = c.ComponentId,
                        ComponentName = c.ComponentName,
                        PartNumber = c.PartNumber,
                        UOM = c.UOM,
                        ReservedQty = c.Quantity,
                        ConsumedQty = 0,
                        Status = "Reserved",
                        UpdatedBy = request.ReservedBy,
                    });
                    await _floorRepo.AdjustReservedAsync(c.ComponentId, c.Quantity);
                }

                return ApiResponse<bool>.SuccessResponse(true, "Components reserved on the shop floor for this order.");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Failed to reserve components: {ex.Message}");
            }
        }

        // Consume one-or-many (order × component) pairs from the shop floor. Blocks per item if the floor is short.
        public async Task<ApiResponse<IEnumerable<ConsumeResultResponse>>> ConsumeAsync(ConsumeComponentsRequest request)
        {
            var results = new List<ConsumeResultResponse>();
            try
            {
                foreach (var item in request.Items.Where(i => i.ComponentId > 0 && i.Quantity > 0))
                {
                    var res = new ConsumeResultResponse
                    {
                        OrderId = item.OrderId,
                        OrderNo = item.OrderNo,
                        ComponentId = item.ComponentId,
                        ComponentName = item.ComponentName,
                        Quantity = item.Quantity,
                    };

                    // Physically remove from the floor (blocks if insufficient).
                    var ok = await _floorRepo.ConsumeAsync(item.ComponentId, item.Quantity);
                    if (!ok)
                    {
                        res.Success = false;
                        res.Message = "Insufficient shop-floor stock — issue more from Main Stock first.";
                        results.Add(res);
                        continue;
                    }

                    // Record against the order (upsert per order+component).
                    var existing = await _ocRepo.GetByOrderComponentAsync(item.OrderId, item.ComponentId);
                    if (existing != null)
                    {
                        var release = Math.Min(item.Quantity, existing.ReservedQty);
                        var newReserved = existing.ReservedQty - release;
                        var status = newReserved <= 0 ? "Consumed" : "Partial";
                        await _ocRepo.AddConsumedAsync(existing.Id, item.Quantity, release, status, request.ConsumedBy);
                    }
                    else
                    {
                        await _ocRepo.InsertAsync(new OrderComponent
                        {
                            OrderId = item.OrderId,
                            OrderItemId = item.OrderItemId,
                            OrderNo = item.OrderNo,
                            ComponentId = item.ComponentId,
                            ComponentName = item.ComponentName,
                            PartNumber = item.PartNumber,
                            UOM = item.UOM,
                            ReservedQty = 0,
                            ConsumedQty = item.Quantity,
                            Status = "Consumed",
                            UpdatedBy = request.ConsumedBy,
                        });
                    }

                    res.Success = true;
                    res.Message = "Consumed";
                    results.Add(res);
                }

                return ApiResponse<IEnumerable<ConsumeResultResponse>>.SuccessResponse(results);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<ConsumeResultResponse>>.ErrorResponse($"Failed to consume components: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<OrderComponentResponse>>> GetByOrderAsync(int orderId)
        {
            try
            {
                var rows = await _ocRepo.GetByOrderAsync(orderId);
                return ApiResponse<IEnumerable<OrderComponentResponse>>.SuccessResponse(rows.Select(r => new OrderComponentResponse
                {
                    OrderId = r.OrderId,
                    OrderItemId = r.OrderItemId,
                    OrderNo = r.OrderNo,
                    ComponentId = r.ComponentId,
                    ComponentName = r.ComponentName,
                    PartNumber = r.PartNumber,
                    UOM = r.UOM,
                    ReservedQty = r.ReservedQty,
                    ConsumedQty = r.ConsumedQty,
                    Status = r.Status,
                }));
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<OrderComponentResponse>>.ErrorResponse($"Error: {ex.Message}");
            }
        }
    }
}

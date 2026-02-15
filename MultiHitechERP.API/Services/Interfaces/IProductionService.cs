using System.Collections.Generic;
using System.Threading.Tasks;
using MultiHitechERP.API.DTOs.Request;
using MultiHitechERP.API.DTOs.Response;

namespace MultiHitechERP.API.Services.Interfaces
{
    public interface IProductionService
    {
        /// <summary>List all orders that have at least one scheduled job card (LEGACY - groups by order)</summary>
        Task<ApiResponse<IEnumerable<ProductionOrderSummary>>> GetOrdersAsync();

        /// <summary>List all order items that have at least one scheduled job card (NEW - groups by order item)</summary>
        Task<ApiResponse<IEnumerable<ProductionOrderSummary>>> GetOrderItemsAsync();

        /// <summary>Full production detail for one order, grouped by child part (LEGACY)</summary>
        Task<ApiResponse<ProductionOrderDetail>> GetOrderDetailAsync(int orderId);

        /// <summary>Full production detail for one order item, grouped by child part (NEW)</summary>
        Task<ApiResponse<ProductionOrderDetail>> GetOrderItemDetailAsync(int orderItemId);

        /// <summary>Operator action on a job card: start | pause | resume | complete</summary>
        Task<ApiResponse<bool>> HandleActionAsync(int jobCardId, ProductionActionRequest request);
    }
}

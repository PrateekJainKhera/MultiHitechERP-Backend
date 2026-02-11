using System.Collections.Generic;
using System.Threading.Tasks;
using MultiHitechERP.API.DTOs.Request;
using MultiHitechERP.API.DTOs.Response;

namespace MultiHitechERP.API.Services.Interfaces
{
    public interface IProductionService
    {
        /// <summary>List all orders that have at least one scheduled job card</summary>
        Task<ApiResponse<IEnumerable<ProductionOrderSummary>>> GetOrdersAsync();

        /// <summary>Full production detail for one order, grouped by child part</summary>
        Task<ApiResponse<ProductionOrderDetail>> GetOrderDetailAsync(int orderId);

        /// <summary>Operator action on a job card: start | pause | resume | complete</summary>
        Task<ApiResponse<bool>> HandleActionAsync(int jobCardId, ProductionActionRequest request);
    }
}

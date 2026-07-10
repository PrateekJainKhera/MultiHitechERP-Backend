using System.Collections.Generic;
using System.Threading.Tasks;
using MultiHitechERP.API.DTOs.Request;
using MultiHitechERP.API.DTOs.Response;

namespace MultiHitechERP.API.Services.Interfaces
{
    public interface IComponentConsumptionService
    {
        Task<ApiResponse<bool>> ReserveForOrderItemAsync(ReserveComponentsRequest request);
        Task<ApiResponse<IEnumerable<ConsumeResultResponse>>> ConsumeAsync(ConsumeComponentsRequest request);
        Task<ApiResponse<IEnumerable<OrderComponentResponse>>> GetByOrderAsync(int orderId);
    }
}

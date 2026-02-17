using System.Collections.Generic;
using System.Threading.Tasks;
using MultiHitechERP.API.DTOs.Request;
using MultiHitechERP.API.DTOs.Response;

namespace MultiHitechERP.API.Services.Interfaces
{
    public interface IComponentIssueService
    {
        Task<ApiResponse<ComponentIssueResponse>> CreateAsync(CreateComponentIssueRequest request);
        Task<ApiResponse<IEnumerable<ComponentIssueResponse>>> GetAllAsync();
        Task<ApiResponse<IEnumerable<ComponentIssueResponse>>> GetByComponentIdAsync(int componentId);
        Task<ApiResponse<IEnumerable<ComponentWithStockResponse>>> GetComponentsWithStockAsync();
    }
}

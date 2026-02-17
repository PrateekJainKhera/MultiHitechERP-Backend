using MultiHitechERP.API.DTOs.Request;
using MultiHitechERP.API.DTOs.Response;

namespace MultiHitechERP.API.Services.Interfaces
{
    public interface IEstimationService
    {
        Task<ApiResponse<IEnumerable<EstimationResponse>>> GetAllAsync();
        Task<ApiResponse<EstimationResponse>> GetByIdAsync(int id);
        Task<ApiResponse<IEnumerable<EstimationResponse>>> GetByCustomerIdAsync(int customerId);
        Task<ApiResponse<IEnumerable<EstimationResponse>>> GetByStatusAsync(string status);
        Task<ApiResponse<EstimationResponse>> CreateAsync(CreateEstimationRequest request);
        Task<ApiResponse<EstimationResponse>> ReviseAsync(int id, CreateEstimationRequest request);
        Task<ApiResponse<EstimationResponse>> SubmitAsync(int id);
        Task<ApiResponse<EstimationResponse>> ApproveAsync(int id, ApproveEstimationRequest request);
        Task<ApiResponse<EstimationResponse>> RejectAsync(int id, RejectEstimationRequest request);
        Task<ApiResponse<EstimationResponse>> ConvertToOrderAsync(int id);
        Task<ApiResponse<bool>> DeleteAsync(int id);
    }
}

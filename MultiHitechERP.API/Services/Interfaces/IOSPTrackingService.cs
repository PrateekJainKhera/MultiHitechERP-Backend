using MultiHitechERP.API.DTOs.Request;
using MultiHitechERP.API.DTOs.Response;
using MultiHitechERP.API.Models.Production;

namespace MultiHitechERP.API.Services.Interfaces
{
    public interface IOSPTrackingService
    {
        Task<ApiResponse<IEnumerable<OSPTracking>>> GetAllAsync();
        Task<ApiResponse<IEnumerable<OSPJobCardOption>>> GetAvailableJobCardsAsync();
        Task<ApiResponse<int>> CreateAsync(CreateOSPTrackingRequest request);
        Task<ApiResponse<IEnumerable<int>>> BatchCreateAsync(BatchCreateOSPRequest request);
        Task<ApiResponse<bool>> MarkReceivedAsync(int id, ReceiveOSPRequest request);
    }
}

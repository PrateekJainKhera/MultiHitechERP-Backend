using MultiHitechERP.API.DTOs.Response;
using MultiHitechERP.API.Models;

namespace MultiHitechERP.API.Services.Interfaces
{
    public interface IPurchaseOrderService
    {
        Task<ApiResponse<IEnumerable<PurchaseOrderResponse>>> GetAllAsync();
        Task<ApiResponse<IEnumerable<PurchaseOrderResponse>>> GetByVendorAsync(int vendorId);
        Task<ApiResponse<IEnumerable<PurchaseOrderResponse>>> GetByPurchaseRequestAsync(int prId);
        Task<ApiResponse<PurchaseOrderResponse>> GetByIdAsync(int id);
        Task<ApiResponse<bool>> SendAsync(int id);
        Task<ApiResponse<bool>> CancelAsync(int id);
    }
}

using MultiHitechERP.API.DTOs.Request;
using MultiHitechERP.API.DTOs.Response;
using MultiHitechERP.API.Models;

namespace MultiHitechERP.API.Services.Interfaces
{
    public interface IPurchaseRequestService
    {
        Task<ApiResponse<IEnumerable<PurchaseRequestResponse>>> GetAllAsync();
        Task<ApiResponse<IEnumerable<PurchaseRequestResponse>>> GetByStatusAsync(string status);
        Task<ApiResponse<IEnumerable<PurchaseRequestResponse>>> GetByItemTypeAsync(string itemType);
        Task<ApiResponse<PurchaseRequestResponse>> GetByIdAsync(int id);
        Task<ApiResponse<int>> CreateAsync(CreatePurchaseRequestRequest request);
        Task<ApiResponse<bool>> SubmitAsync(int id);
        Task<ApiResponse<bool>> StartReviewAsync(int id);
        Task<ApiResponse<IEnumerable<PurchaseOrderResponse>>> ApproveAsync(int id, ApprovePurchaseRequestRequest request);
        Task<ApiResponse<bool>> RejectAsync(int id, string rejectionReason, string rejectedBy);
        Task<ApiResponse<bool>> AddItemAsync(int prId, CreatePurchaseRequestItemRequest item);
        Task<ApiResponse<bool>> RemoveItemAsync(int prId, int itemId);
    }
}

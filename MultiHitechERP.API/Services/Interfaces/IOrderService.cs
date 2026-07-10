using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MultiHitechERP.API.DTOs.Request;
using MultiHitechERP.API.DTOs.Response;

namespace MultiHitechERP.API.Services.Interfaces
{
    /// <summary>
    /// Service interface for Order business logic
    /// </summary>
    public interface IOrderService
    {
        // CRUD Operations
        Task<ApiResponse<OrderResponse>> GetByIdAsync(int id);
        Task<ApiResponse<OrderResponse>> GetByOrderNoAsync(string orderNo);
        Task<ApiResponse<IEnumerable<OrderResponse>>> GetAllAsync();
        Task<ApiResponse<PagedOrdersResponse>> GetPagedAsync(int page, int pageSize, string? search, string? status, OrderListFilter? filter = null);
        Task<ApiResponse<IEnumerable<OrderLiteResponse>>> GetLiteListAsync();
        Task<ApiResponse<bool>> ChangeCustomerAsync(int orderId, ChangeCustomerRequest request);
        Task<ApiResponse<OrderSummaryResponse>> GetSummaryAsync();
        Task<ApiResponse<PagedPlanningItemsResponse>> GetPlanningItemsAsync(string type, int page, int pageSize, string? search);
        Task<ApiResponse<PlanningSummaryResponse>> GetPlanningSummaryAsync();
        Task<ApiResponse<IEnumerable<OrderResponse>>> GetByCustomerIdAsync(int customerId);
        Task<ApiResponse<IEnumerable<OrderResponse>>> GetByStatusAsync(string status);

        Task<ApiResponse<int>> CreateOrderAsync(CreateOrderRequest request);
        Task<ApiResponse<bool>> UpdateOrderAsync(UpdateOrderRequest request);
        Task<ApiResponse<bool>> UpdateQuantityAsync(int orderId, int newQuantity, int? orderItemId, string updatedBy);
        Task<ApiResponse<bool>> DeleteOrderAsync(int id);

        // Business Operations
        Task<ApiResponse<bool>> UpdateDrawingReviewStatusAsync(UpdateDrawingReviewRequest request);
        Task<ApiResponse<bool>> ApproveDrawingReviewAsync(int orderId, string reviewedBy, string? notes, int linkedProductTemplateId);
        Task<ApiResponse<bool>> RejectDrawingReviewAsync(int orderId, string reviewedBy, string reason);

        Task<ApiResponse<bool>> UpdateOrderStatusAsync(int orderId, string status);
        Task<ApiResponse<bool>> UpdatePlanningStatusAsync(int orderId, string status);

        // Queries
        Task<ApiResponse<IEnumerable<OrderResponse>>> GetPendingDrawingReviewAsync();
        Task<ApiResponse<IEnumerable<OrderResponse>>> GetReadyForPlanningAsync();
        Task<ApiResponse<IEnumerable<OrderResponse>>> GetInProgressOrdersAsync();
        Task<ApiResponse<IEnumerable<OrderResponse>>> GetDelayedOrdersAsync();

        // Validation
        Task<ApiResponse<bool>> CanGenerateJobCardsAsync(int orderId);
        Task<ApiResponse<string>> GenerateOrderNoAsync();
    }
}

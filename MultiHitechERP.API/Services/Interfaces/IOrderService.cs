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
        Task<ApiResponse<OrderResponse>> GetByIdAsync(Guid id);
        Task<ApiResponse<OrderResponse>> GetByOrderNoAsync(string orderNo);
        Task<ApiResponse<IEnumerable<OrderResponse>>> GetAllAsync();
        Task<ApiResponse<IEnumerable<OrderResponse>>> GetByCustomerIdAsync(Guid customerId);
        Task<ApiResponse<IEnumerable<OrderResponse>>> GetByStatusAsync(string status);

        Task<ApiResponse<Guid>> CreateOrderAsync(CreateOrderRequest request);
        Task<ApiResponse<bool>> UpdateOrderAsync(UpdateOrderRequest request);
        Task<ApiResponse<bool>> DeleteOrderAsync(Guid id);

        // Business Operations
        Task<ApiResponse<bool>> UpdateDrawingReviewStatusAsync(UpdateDrawingReviewRequest request);
        Task<ApiResponse<bool>> ApproveDrawingReviewAsync(Guid orderId, string reviewedBy, string? notes);
        Task<ApiResponse<bool>> RejectDrawingReviewAsync(Guid orderId, string reviewedBy, string reason);

        Task<ApiResponse<bool>> UpdateOrderStatusAsync(Guid orderId, string status);
        Task<ApiResponse<bool>> UpdatePlanningStatusAsync(Guid orderId, string status);

        // Queries
        Task<ApiResponse<IEnumerable<OrderResponse>>> GetPendingDrawingReviewAsync();
        Task<ApiResponse<IEnumerable<OrderResponse>>> GetReadyForPlanningAsync();
        Task<ApiResponse<IEnumerable<OrderResponse>>> GetInProgressOrdersAsync();
        Task<ApiResponse<IEnumerable<OrderResponse>>> GetDelayedOrdersAsync();

        // Validation
        Task<ApiResponse<bool>> CanGenerateJobCardsAsync(Guid orderId);
        Task<ApiResponse<string>> GenerateOrderNoAsync();
    }
}

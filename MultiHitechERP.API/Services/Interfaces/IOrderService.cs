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
        Task<ApiResponse<IEnumerable<OrderResponse>>> GetByCustomerIdAsync(int customerId);
        Task<ApiResponse<IEnumerable<OrderResponse>>> GetByStatusAsync(string status);

        Task<ApiResponse<int>> CreateOrderAsync(CreateOrderRequest request);
        Task<ApiResponse<bool>> UpdateOrderAsync(UpdateOrderRequest request);
        Task<ApiResponse<bool>> DeleteOrderAsync(int id);

        // Business Operations
        Task<ApiResponse<bool>> UpdateDrawingReviewStatusAsync(UpdateDrawingReviewRequest request);
        Task<ApiResponse<bool>> ApproveDrawingReviewAsync(int orderId, string reviewedBy, string? notes);
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

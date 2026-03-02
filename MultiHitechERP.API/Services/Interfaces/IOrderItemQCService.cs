using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using MultiHitechERP.API.DTOs.Response;

namespace MultiHitechERP.API.Services.Interfaces
{
    public interface IOrderItemQCService
    {
        /// <summary>Order items with completed assembly but no Passed QC.</summary>
        Task<ApiResponse<IEnumerable<QCPendingItemResponse>>> GetPendingItemsAsync();

        /// <summary>Latest QC record for a specific order item (null if none submitted yet).</summary>
        Task<OrderItemQCResponse?> GetLatestByOrderItemAsync(int orderItemId);

        /// <summary>
        /// Submit or re-submit QC for an order item.
        /// Creates a new QC record each time (keeps history).
        /// </summary>
        Task<ApiResponse<OrderItemQCResponse>> SubmitQCAsync(
            int orderItemId,
            int orderId,
            string qcStatus,
            string qcBy,
            string? notes,
            Stream? certificateStream,
            string? certificateContentType,
            string? originalFileName);
    }
}

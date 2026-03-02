using System.Collections.Generic;
using System.Threading.Tasks;
using MultiHitechERP.API.DTOs.Response;
using MultiHitechERP.API.Models.Production;

namespace MultiHitechERP.API.Repositories.Interfaces
{
    public interface IOrderItemQCRepository
    {
        /// <summary>Order items where assembly is Completed but no Passed QC record exists.</summary>
        Task<IEnumerable<QCPendingItemResponse>> GetPendingItemsAsync();

        /// <summary>Latest QC record for a specific order item (null if none).</summary>
        Task<OrderItemQC?> GetLatestByOrderItemAsync(int orderItemId);

        /// <summary>True if the order item has a QC record with Status='Passed'.</summary>
        Task<bool> HasPassedQCAsync(int orderItemId);

        /// <summary>Insert a new QC record. Returns the new Id.</summary>
        Task<int> InsertAsync(OrderItemQC record);

        /// <summary>Update QCStatus, CertificatePath, QCCompletedAt, QCCompletedBy, Notes on an existing record.</summary>
        Task<bool> UpdateAsync(OrderItemQC record);
    }
}

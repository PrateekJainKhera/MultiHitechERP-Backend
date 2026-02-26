using MultiHitechERP.API.DTOs.Request;
using MultiHitechERP.API.Models.Production;

namespace MultiHitechERP.API.Repositories.Interfaces
{
    public interface IOSPTrackingRepository
    {
        Task<IEnumerable<OSPTracking>> GetAllAsync();
        Task<OSPTracking?> GetByIdAsync(int id);
        Task<IEnumerable<OSPJobCardOption>> GetAvailableJobCardsAsync();
        Task<int> InsertAsync(OSPTracking entry);
        Task<IEnumerable<int>> BatchInsertAsync(IEnumerable<OSPTracking> entries);

        /// <summary>
        /// Partial/full receive. Accumulates ReceivedQty + RejectedQty.
        /// When total >= Quantity, status flips to Received and job card is auto-completed.
        /// </summary>
        Task MarkReceivedAsync(int id, int receivedQty, int rejectedQty, DateTime actualReturnDate, string? notes, string? updatedBy);

        /// <summary>
        /// Returns a map of JobCardId → "Sent" for any job cards
        /// that currently have an active (Status=Sent) OSP entry.
        /// </summary>
        Task<Dictionary<int, string>> GetActiveOspStatusByJobCardIdsAsync(IEnumerable<int> jobCardIds);
    }
}

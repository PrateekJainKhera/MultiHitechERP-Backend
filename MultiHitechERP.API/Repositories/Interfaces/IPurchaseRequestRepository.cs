using MultiHitechERP.API.Models.Procurement;

namespace MultiHitechERP.API.Repositories.Interfaces
{
    public interface IPurchaseRequestRepository
    {
        Task<IEnumerable<PurchaseRequest>> GetAllAsync();
        Task<IEnumerable<PurchaseRequest>> GetByStatusAsync(string status);
        Task<IEnumerable<PurchaseRequest>> GetByItemTypeAsync(string itemType);
        Task<PurchaseRequest?> GetByIdAsync(int id);
        Task<int> InsertAsync(PurchaseRequest pr);
        Task<bool> UpdateStatusAsync(int id, string status, string? approvedBy = null, string? rejectionReason = null);
        Task<bool> UpdateItemAsync(PurchaseRequestItem item);
        Task<int> InsertItemAsync(PurchaseRequestItem item);
        Task<bool> DeleteItemAsync(int itemId);
        Task<int> GetNextSequenceNumberAsync();
    }
}

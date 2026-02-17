using MultiHitechERP.API.Models.Procurement;

namespace MultiHitechERP.API.Repositories.Interfaces
{
    public interface IPurchaseOrderRepository
    {
        Task<IEnumerable<PurchaseOrder>> GetAllAsync();
        Task<IEnumerable<PurchaseOrder>> GetByVendorAsync(int vendorId);
        Task<IEnumerable<PurchaseOrder>> GetByPurchaseRequestAsync(int prId);
        Task<PurchaseOrder?> GetByIdAsync(int id);
        Task<int> InsertAsync(PurchaseOrder po);
        Task<bool> UpdateStatusAsync(int id, string status);
        Task<int> GetNextSequenceNumberAsync();
    }
}

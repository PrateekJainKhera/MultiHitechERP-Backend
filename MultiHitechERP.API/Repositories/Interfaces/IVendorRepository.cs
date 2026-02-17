using MultiHitechERP.API.Models.Masters;

namespace MultiHitechERP.API.Repositories.Interfaces
{
    public interface IVendorRepository
    {
        Task<IEnumerable<Vendor>> GetAllAsync();
        Task<IEnumerable<Vendor>> GetActiveAsync();
        Task<Vendor?> GetByIdAsync(int id);
        Task<Vendor?> GetByVendorCodeAsync(string vendorCode);
        Task<IEnumerable<Vendor>> SearchByNameAsync(string searchTerm);
        Task<int> InsertAsync(Vendor vendor);
        Task<bool> UpdateAsync(Vendor vendor);
        Task<bool> DeleteAsync(int id);
        Task<int> GetNextSequenceNumberAsync();
    }
}

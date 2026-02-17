using MultiHitechERP.API.Models.Sales;

namespace MultiHitechERP.API.Repositories.Interfaces
{
    public interface IEstimationRepository
    {
        Task<IEnumerable<Estimation>> GetAllAsync();
        Task<Estimation?> GetByIdAsync(int id);
        Task<IEnumerable<Estimation>> GetByCustomerIdAsync(int customerId);
        Task<IEnumerable<Estimation>> GetByStatusAsync(string status);
        Task<int> InsertAsync(Estimation estimation);
        Task UpdateStatusAsync(int id, string status, string? updatedBy = null, int? convertedOrderId = null, string? rejectionReason = null);
        Task DeleteAsync(int id);
        Task<int> GetNextSequenceNumberAsync();
    }
}

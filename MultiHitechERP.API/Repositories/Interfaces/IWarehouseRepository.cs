using System.Collections.Generic;
using System.Threading.Tasks;
using MultiHitechERP.API.DTOs.Response;
using MultiHitechERP.API.Models.Masters;

namespace MultiHitechERP.API.Repositories.Interfaces
{
    public interface IWarehouseRepository
    {
        Task<IEnumerable<Warehouse>> GetAllAsync();
        Task<Warehouse?> GetByIdAsync(int id);
        Task<int> CreateAsync(Warehouse warehouse);
        Task<bool> UpdateAsync(Warehouse warehouse);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<LowStockAlertResponse>> GetLowStockStatusAsync();
    }
}

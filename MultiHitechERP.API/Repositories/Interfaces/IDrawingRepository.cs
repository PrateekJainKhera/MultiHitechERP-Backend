using System.Collections.Generic;
using System.Threading.Tasks;
using MultiHitechERP.API.Models.Masters;

namespace MultiHitechERP.API.Repositories.Interfaces
{
    public interface IDrawingRepository
    {
        Task<Drawing?> GetByIdAsync(int id);
        Task<IEnumerable<Drawing>> GetAllAsync();
        Task<int> InsertAsync(Drawing drawing);
        Task<bool> UpdateAsync(Drawing drawing);
        Task<bool> DeleteAsync(int id);
        Task<string> GetNextDrawingNumberAsync();
        Task<bool> ExistsAsync(string drawingNumber);
    }
}

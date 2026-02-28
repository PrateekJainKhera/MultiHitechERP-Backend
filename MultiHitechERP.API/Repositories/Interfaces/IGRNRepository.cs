using System.Collections.Generic;
using System.Threading.Tasks;
using MultiHitechERP.API.Models.Stores;

namespace MultiHitechERP.API.Repositories.Interfaces
{
    public interface IGRNRepository
    {
        Task<int> CreateAsync(GRN grn);
        Task<GRN?> GetByIdAsync(int id);
        Task<GRN?> GetByGRNNoAsync(string grnNo);
        Task<IEnumerable<GRN>> GetAllAsync();
        Task<IEnumerable<GRN>> GetBySupplierId(int supplierId);
        Task<IEnumerable<GRN>> GetByStatusAsync(string status);
        Task<bool> UpdateAsync(GRN grn);
        Task<bool> DeleteAsync(int id);

        // GRN Lines
        Task<int> CreateLineAsync(GRNLine line);
        Task<IEnumerable<GRNLine>> GetLinesByGRNIdAsync(int grnId);
        Task<bool> UpdateLineAsync(GRNLine line);
        Task<bool> DeleteLineAsync(int lineId);
    }
}

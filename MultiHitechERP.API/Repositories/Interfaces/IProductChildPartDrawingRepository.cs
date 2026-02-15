using System.Collections.Generic;
using System.Threading.Tasks;
using MultiHitechERP.API.Models.Masters;

namespace MultiHitechERP.API.Repositories.Interfaces
{
    public interface IProductChildPartDrawingRepository
    {
        Task<int> CreateAsync(ProductChildPartDrawing record);
        Task<ProductChildPartDrawing?> GetByIdAsync(int id);
        Task<IEnumerable<ProductChildPartDrawing>> GetByProductIdAsync(int productId);
        Task<ProductChildPartDrawing?> GetByProductAndChildPartAsync(int productId, int childPartTemplateId);
        Task<bool> UpdateAsync(ProductChildPartDrawing record);
        Task<bool> DeleteAsync(int id);
        Task<bool> DeleteByProductIdAsync(int productId);
    }
}

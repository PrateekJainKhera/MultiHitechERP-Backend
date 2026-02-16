using System.Collections.Generic;
using System.Threading.Tasks;
using MultiHitechERP.API.Models.Masters;

namespace MultiHitechERP.API.Repositories.Interfaces
{
    public interface IProductDefaultMaterialRepository
    {
        Task<IEnumerable<ProductDefaultMaterial>> GetByProductIdAsync(int productId);
        Task SaveDefaultsAsync(int productId, IEnumerable<ProductDefaultMaterial> defaults, string? updatedBy);
    }
}

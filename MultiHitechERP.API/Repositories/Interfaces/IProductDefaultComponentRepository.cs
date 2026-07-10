using System.Collections.Generic;
using System.Threading.Tasks;
using MultiHitechERP.API.Models.Masters;

namespace MultiHitechERP.API.Repositories.Interfaces
{
    public interface IProductDefaultComponentRepository
    {
        Task<IEnumerable<ProductDefaultComponent>> GetByProductIdAsync(int productId);
        Task SaveDefaultsAsync(int productId, IEnumerable<ProductDefaultComponent> defaults, string? updatedBy);
    }
}

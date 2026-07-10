using System.Collections.Generic;
using System.Threading.Tasks;
using MultiHitechERP.API.Models.Stores;

namespace MultiHitechERP.API.Repositories.Interfaces
{
    public interface IShopFloorComponentStockRepository
    {
        Task<IEnumerable<ShopFloorComponentStock>> GetAllAsync();
        Task<ShopFloorComponentStock?> GetByComponentIdAsync(int componentId);

        /// <summary>Add quantity to the floor for a component (upsert; creates the row if absent).</summary>
        Task AddToFloorAsync(int componentId, string? componentName, string? partNumber, string? uom, decimal qty, string? by);

        /// <summary>Adjust the reserved quantity by delta (positive to reserve, negative to release). Never goes below 0.</summary>
        Task AdjustReservedAsync(int componentId, decimal delta);

        /// <summary>Consume (physically remove) qty from the floor. Returns false if the floor doesn't have enough.</summary>
        Task<bool> ConsumeAsync(int componentId, decimal qty);
    }
}

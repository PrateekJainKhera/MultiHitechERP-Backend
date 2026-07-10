using System.Collections.Generic;
using System.Threading.Tasks;
using MultiHitechERP.API.Models.Stores;

namespace MultiHitechERP.API.Repositories.Interfaces
{
    public interface IOrderComponentRepository
    {
        Task<IEnumerable<OrderComponent>> GetByOrderAsync(int orderId);
        Task<IEnumerable<OrderComponent>> GetReservedForOrderItemAsync(int orderId, int? orderItemId);
        Task DeleteReservedForOrderItemAsync(int orderId, int? orderItemId);
        Task<int> InsertAsync(OrderComponent oc);
        Task<OrderComponent?> GetByOrderComponentAsync(int orderId, int componentId);
        Task AddConsumedAsync(int id, decimal addConsumed, decimal releaseReserved, string status, string? by);
    }
}

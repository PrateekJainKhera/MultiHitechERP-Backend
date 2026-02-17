using MultiHitechERP.API.Models.Orders;

namespace MultiHitechERP.API.Repositories.Interfaces
{
    public interface IOrderCustomerDrawingRepository
    {
        Task<IEnumerable<OrderCustomerDrawing>> GetByOrderIdAsync(int orderId);
        Task<OrderCustomerDrawing?> GetByIdAsync(int id);
        Task<int> InsertAsync(OrderCustomerDrawing drawing);
        Task DeleteAsync(int id);
    }
}

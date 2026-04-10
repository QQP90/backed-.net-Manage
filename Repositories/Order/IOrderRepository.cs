using WebApplication1.Models.Entities;

namespace WebApplication1.Repositories.Order
{
    public interface IOrderRepository
    {
        Task<List<global::WebApplication1.Models.Entities.Order>> GetAllAsync();
        Task<global::WebApplication1.Models.Entities.Order?> GetByIdAsync(int orderId);
        Task<global::WebApplication1.Models.Entities.Order> CreateAsync(global::WebApplication1.Models.Entities.Order order);
        Task<bool> UpdateAsync(global::WebApplication1.Models.Entities.Order order);
        Task<bool> DeleteAsync(int orderId);
    }
}

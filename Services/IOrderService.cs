using WebApplication1.Models.DTOs.Order;

namespace WebApplication1.Services
{
    public interface IOrderService
    {
        Task<List<OrderDto>> GetAllAsync();
        Task<OrderDto?> GetByIdAsync(int orderId);
        Task<OrderDto> CreateAsync(CreateOrderDto dto);
        Task<OrderDto?> UpdateAsync(int orderId, UpdateOrderDto dto);
        Task<bool> DeleteAsync(int orderId);
    }
}

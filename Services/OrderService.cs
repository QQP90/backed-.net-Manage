using WebApplication1.Models.DTOs.Order;
using WebApplication1.Models.Entities;
using WebApplication1.Repositories.Order;

namespace WebApplication1.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _repo;

        // 第 1 步（Service 层）：注入 Repository，Service 负责“业务规则/默认值/字段处理”
        // 如果把这些逻辑放在 Controller：会导致 Controller 代码膨胀、难测试、难复用
        public OrderService(IOrderRepository repo)
        {
            _repo = repo;
        }

        // 第 2 步（列表）：从 Repository 拿实体，再映射成 DTO 返回给前端
        // 如果直接返回 Entity：以后改表字段/加敏感字段（例如成本价）会不小心暴露给前端
        public async Task<List<OrderDto>> GetAllAsync()
        {
            var items = await _repo.GetAllAsync();
            return items.Select(Map).ToList();
        }

        // 第 3 步（详情）：同样做 Entity -> DTO 映射
        public async Task<OrderDto?> GetByIdAsync(int orderId)
        {
            var item = await _repo.GetByIdAsync(orderId);
            return item == null ? null : Map(item);
        }

        // 第 4 步（新增）：在这里实现默认值策略
        // 关键：orderDate 如果不传，默认用“当前时间”，避免出现 1900-01-01 这种默认值
        // 说明：这里用 UTC 时间，和数据库 default (SYSUTCDATETIME) 保持一致
        public async Task<OrderDto> CreateAsync(CreateOrderDto dto)
        {
            var now = DateTime.UtcNow;
            var normalizedOrderDate = dto.OrderDate;
            if (!normalizedOrderDate.HasValue || normalizedOrderDate.Value.Year < 2000)
            {
                normalizedOrderDate = now;
            }

            var entity = new Order
            {
                Name = dto.Name,
                totalPrice = dto.TotalPrice,
                orderDate = normalizedOrderDate.Value,
                createdAt = now,
                updatedAt = now
            };
            var created = await _repo.CreateAsync(entity);
            return Map(created);
        }

        // 第 5 步（更新）：只更新客户端传入的字段（null 表示“不改”）
        // 如果直接赋值：客户端没传的字段会被覆盖为默认值，导致数据丢失
        public async Task<OrderDto?> UpdateAsync(int orderId, UpdateOrderDto dto)
        {
            var entity = await _repo.GetByIdAsync(orderId);
            if (entity == null) return null;

            if (!string.IsNullOrWhiteSpace(dto.Name)) entity.Name = dto.Name;
            if (dto.totalPrice.HasValue) entity.totalPrice = dto.totalPrice.Value;
            if (dto.orderDate.HasValue)
            {
                entity.orderDate = dto.orderDate.Value.Year < 2000 ? DateTime.UtcNow : dto.orderDate.Value;
            }

            var ok = await _repo.UpdateAsync(entity);
            return ok ? Map(entity) : null;
        }

        // 第 6 步（删除）：交给 Repository 统一处理数据库操作
        public Task<bool> DeleteAsync(int orderId) => _repo.DeleteAsync(orderId);

        // 第 7 步（映射）：集中做 DTO 映射，避免在各处复制粘贴
        private static OrderDto Map(Order x) =>
            new OrderDto { OrderId = x.OrderId, Name = x.Name, totalPrice = x.totalPrice, orderDate = x.orderDate };
    }
}

using Microsoft.EntityFrameworkCore;
using WebApplication1.Infrastructure;
using WebApplication1.Models.Entities;

namespace WebApplication1.Repositories.Order
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _dbContext;

        // 第 1 步（Repository 层）：注入 EF Core DbContext
        // Repository 只负责“怎么查/怎么写数据库”，不放业务规则（默认值/校验等放 Service）
        public OrderRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<global::WebApplication1.Models.Entities.Order>> GetAllAsync()
        {
            // AsNoTracking：只读查询不需要跟踪，性能更好、内存更省
            // 如果不加：默认会跟踪实体，数据量大时会明显变慢、占内存
            return await _dbContext.Orders.AsNoTracking().ToListAsync();
        }

        public async Task<global::WebApplication1.Models.Entities.Order?> GetByIdAsync(int orderId)
        {
            return await _dbContext.Orders.FirstOrDefaultAsync(x => x.OrderId == orderId);
        }

        public async Task<global::WebApplication1.Models.Entities.Order> CreateAsync(global::WebApplication1.Models.Entities.Order order)
        {
            // createdAt/updatedAt 在 Repository 兜底一层，避免外部忘记赋值
            // 如果两边都不赋值：数据库里会出现默认时间或 0001-01-01，前端显示异常
            order.createdAt = DateTime.UtcNow;
            order.updatedAt = DateTime.UtcNow;

            _dbContext.Orders.Add(order);
            // SaveChangesAsync：真正把 INSERT 发给 SQL Server
            // 如果不调用：只是在内存里 Add，数据库不会变化
            await _dbContext.SaveChangesAsync();
            return order;
        }

        public async Task<bool> UpdateAsync(global::WebApplication1.Models.Entities.Order order)
        {
            order.updatedAt = DateTime.UtcNow;

            _dbContext.Orders.Update(order);
            return await _dbContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(int orderId)
        {
            var entity = await _dbContext.Orders.FirstOrDefaultAsync(x => x.OrderId == orderId);
            if (entity == null) return false;

            _dbContext.Orders.Remove(entity);
            return await _dbContext.SaveChangesAsync() > 0;
        }
    }
}

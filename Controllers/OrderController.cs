using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models.DTOs.Order;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _service;

        // 第 1 步（Controller 层）：通过依赖注入拿到业务层 IOrderService
        // 如果 IOrderService 没有在 Program.cs 注册：这里会直接启动失败或请求时报 500
        public OrderController(IOrderService service)
        {
            _service = service;
        }

        // 第 2 步（查询列表）：Controller 只负责 HTTP 入参/出参，不直接写 SQL
        // 流程：HTTP -> Controller -> Service -> Repository -> DbContext -> SQL Server
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _service.GetAllAsync());
        }

        // 第 3 步（查询单条）：先校验参数，再调用 Service
        // 如果不校验：orderId=0/-1 这种无意义请求会打到数据库，浪费资源，甚至引发异常
        [HttpGet("{orderId:int}")]
        public async Task<IActionResult> GetById(int orderId)
        {
            if (orderId <= 0)
            {
                return BadRequest(new { message = "ID 必须大于 0" });
            }

            var item = await _service.GetByIdAsync(orderId);
            return item == null ? NotFound() : Ok(item);
        }

        // 第 4 步（新增）：[ApiController] + DataAnnotations 自动校验请求体
        // 如果不检查 ModelState：可能把空 Name/非法价格写入数据库，导致脏数据或数据库异常
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateOrderDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var created = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { orderId = created.OrderId }, created);
        }

        // 第 5 步（更新）：一般用 DTO（可选字段）做“局部更新”
        // 如果直接用 Entity 做入参：很容易被覆盖字段（例如 createdAt 被客户端篡改）
        [HttpPut("{orderId:int}")]
        public async Task<IActionResult> Update(int orderId, [FromBody] UpdateOrderDto dto)
        {
            if (orderId <= 0)
            {
                return BadRequest(new { message = "ID 必须大于 0" });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updated = await _service.UpdateAsync(orderId, dto);
            return updated == null ? NotFound() : Ok(updated);
        }

        // 第 6 步（删除）：先校验参数，再调用 Service
        // 删除策略目前是“物理删除”；如果你希望“软删除”，需要给表加 DeletedAt 并改 Repository
        [HttpDelete("{orderId:int}")]
        public async Task<IActionResult> Delete(int orderId)
        {
            if (orderId <= 0)
            {
                return BadRequest(new { message = "ID 必须大于 0" });
            }

            var ok = await _service.DeleteAsync(orderId);
            return ok ? Ok() : NotFound();
        }
    }
}

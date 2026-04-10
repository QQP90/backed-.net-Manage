namespace WebApplication1.Models.Entities
{
    public class Order
    {
        // 主键：数据库自增（IDENTITY）
        public int OrderId { get; set; }
        // 订单名称/描述
        public string Name { get; set; } = string.Empty;
        // 总金额
        public decimal totalPrice { get; set; }
        // 下单时间（前端不传时，在 Service/DB 两端都会兜底为当前时间）
        public DateTime orderDate { get; set; }
        // 创建时间（服务端生成）
        public DateTime createdAt { get; set; } = DateTime.Now;
        // 更新时间（服务端更新时刷新）
        public DateTime updatedAt { get; set; } = DateTime.Now;
    }
}

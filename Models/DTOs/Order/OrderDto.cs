namespace WebApplication1.Models.DTOs.Order
{
    public class OrderDto
    {
        // 返回给前端的订单ID
        public int OrderId { get; set; }
        // 返回给前端的名称
        public string Name { get; set; } = string.Empty;
        // 返回给前端的金额
        public decimal totalPrice { get; set; }
        // 返回给前端的下单时间
        public DateTime orderDate { get; set; }
    }
}

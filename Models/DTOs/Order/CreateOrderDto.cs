using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models.DTOs.Order
{
    public class CreateOrderDto
    {
        // 前端创建订单时传入的名称（必填）
        [Required]
        [StringLength(100, MinimumLength = 1)]
        public string Name { get; set; } = string.Empty;

        // 前端传入金额（必填）
        [Range(typeof(decimal), "0", "79228162514264337593543950335")]
        public decimal TotalPrice { get; set; }

        // 前端可选传入下单时间
        // 不传：Service 会用当前时间；数据库也有默认值兜底
        public DateTime? OrderDate { get; set; }
    }
}

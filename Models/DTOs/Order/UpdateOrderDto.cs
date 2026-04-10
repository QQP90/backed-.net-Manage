using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models.DTOs.Order
{
    public class UpdateOrderDto
    {
        // 可选字段：传了才更新，不传就不改
        [StringLength(100, MinimumLength = 1)]
        public string? Name { get; set; }

        // 可选字段：传了才更新，不传就不改
        [Range(typeof(decimal), "0", "79228162514264337593543950335")]
        public decimal? totalPrice { get; set; }

        // 可选字段：传了才更新，不传就不改
        public DateTime? orderDate { get; set; }
    }
}

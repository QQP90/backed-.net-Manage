using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models.DTOs.User
{
    public class CreateUserDto
    {
        // 第 1 步（入参 DTO）：定义“前端创建用户时允许传什么字段”
        // 如果不用 DTO 而直接用 Entity 接收：前端可能篡改不该传的字段（如 IsEmailVerified/CreatedAt），造成安全和数据问题

        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; } = string.Empty;

        [StringLength(30)]
        public string? PhoneNumber { get; set; }

        [StringLength(50)]
        public string? FirstName { get; set; }

        [StringLength(50)]
        public string? LastName { get; set; }

        [StringLength(20)]
        public string? Gender { get; set; }

        public DateTime? DateOfBirth { get; set; }

        [StringLength(300)]
        public string? Avatar { get; set; }

        [StringLength(500)]
        public string? Bio { get; set; }

        [StringLength(200)]
        public string? Address { get; set; }

        [StringLength(100)]
        public string? City { get; set; }

        [StringLength(100)]
        public string? State { get; set; }

        [StringLength(20)]
        public string? ZipCode { get; set; }

        [StringLength(100)]
        public string? Country { get; set; }

        public bool? IsActive { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models.DTOs
{
    /// <summary>
    /// 创建学生请求 DTO
    /// </summary>
    public class CreateStudentDto
    {
        /// <summary>
        /// 姓名（必填，2-50 字符）
        /// </summary>
        [Required(ErrorMessage = "姓名不能为空")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "姓名长度必须在 2-50 个字符之间")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 年龄（必填，1-150）
        /// </summary>
        [Required(ErrorMessage = "年龄不能为空")]
        [Range(1, 150, ErrorMessage = "年龄必须在 1-150 之间")]
        public int Age { get; set; }

        /// <summary>
        /// 邮箱（可选）
        /// </summary>
        [RegularExpression(@"^1[3-9]\d{9}$", ErrorMessage = "手机号格式不正确")]
        public string? Phone { get; set; }
    }
}

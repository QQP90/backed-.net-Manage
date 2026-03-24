using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models.DTOs
{
    /// <summary>
    /// 更新学生请求 DTO
    /// </summary>
    public class UpdateStudentDto
    {
        /// <summary>
        /// 姓名（可选，2-50 字符）
        /// </summary>
        [StringLength(50, MinimumLength = 2, ErrorMessage = "姓名长度必须在 2-50 个字符之间")]
        public string? Name { get; set; }

        /// <summary>
        /// 年龄（可选，1-150）
        /// </summary>
        [Range(1, 150, ErrorMessage = "年龄必须在 1-150 之间")]
        public int? Age { get; set; }

        /// <summary>
        /// 邮箱（可选）
        /// </summary>
        [EmailAddress(ErrorMessage = "邮箱格式不正确")]
        public string? Email { get; set; }
    }
}

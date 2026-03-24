namespace WebApplication1.Models.DTOs
{
    /// <summary>
    /// 学生数据传输对象 - 用于 API 响应
    /// </summary>
    public class StudentDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
        public string? Phone { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}

namespace WebApplication1.Models.Entities
{
    /// <summary>
    /// 学生实体类 - 对应数据库 Student 表
    /// </summary>
    public class Student
    {
        /// <summary>
        /// 学生 ID（主键）
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 年龄
        /// </summary>
        public int Age { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        public string? Phone { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime? UpdatedAt { get; set; }
    }
}

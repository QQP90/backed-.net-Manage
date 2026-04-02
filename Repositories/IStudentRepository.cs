using WebApplication1.Models.Entities;

namespace WebApplication1.Repositories
{
    /// <summary>
    /// 学生数据访问接口
    /// </summary>
    public interface IStudentRepository
    {
        /// <summary>
        /// 获取所有学生
        /// </summary>
        Task<IEnumerable<Student>> GetAllAsync();

        /// <summary>
        /// 根据 ID 获取学生
        /// </summary>
        Task<Student?> GetByIdAsync(int id);

        /// <summary>
        /// 创建学生
        /// </summary>
        Task<int> CreateAsync(Student student);

        /// <summary>
        /// 更新学生
        /// </summary>
        Task<bool> UpdateAsync(Student student);

        /// <summary>
        /// 删除学生
        /// </summary>
        Task<bool> DeleteAsync(int id);
    }
}

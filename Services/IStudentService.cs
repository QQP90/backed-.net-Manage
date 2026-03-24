using WebApplication1.Models;
using WebApplication1.Models.DTOs;

namespace WebApplication1.Services
{
    /// <summary>
    /// 学生业务逻辑接口
    /// </summary>
    public interface IStudentService
    {
        /// <summary>
        /// 获取所有学生
        /// </summary>
        Task<IEnumerable<StudentDto>> GetAllAsync();

        /// <summary>
        /// 根据 ID 获取学生
        /// </summary>
        Task<StudentDto?> GetByIdAsync(int id);

        /// <summary>
        /// 创建学生
        /// </summary>
        Task<StudentDto> CreateAsync(CreateStudentDto dto);

        /// <summary>
        /// 更新学生
        /// </summary>
        Task<StudentDto?> UpdateAsync(int id, UpdateStudentDto dto);

        /// <summary>
        /// 删除学生
        /// </summary>
        Task<bool> DeleteAsync(int id);
    }
}

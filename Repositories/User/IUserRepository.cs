using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication1.Models.Entities;
using WebApplication1.Models.Queries;

namespace WebApplication1.Repositories
{
    /// <summary>
    /// 用户仓储接口
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>
        /// 根据ID获取用户
        /// </summary>
        Task<User> GetUserByIdAsync(int userId);

        /// <summary>
        /// 根据用户名获取用户
        /// </summary>
        Task<User> GetUserByUsernameAsync(string username);

        /// <summary>
        /// 根据邮箱获取用户
        /// </summary>
        Task<User> GetUserByEmailAsync(string email);

        /// <summary>
        /// 查询用户列表（分页）
        /// </summary>
        Task<(List<User> Items, int TotalCount)> GetUsersAsync(UserQueryModel query);

        /// <summary>
        /// 创建用户
        /// </summary>
        Task<User> CreateUserAsync(User user);

        /// <summary>
        /// 更新用户
        /// </summary>
        Task<bool> UpdateUserAsync(User user);

        /// <summary>
        /// 删除用户（软删除）
        /// </summary>
        Task<bool> DeleteUserAsync(int userId);
    }
}

using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication1.Models;
using WebApplication1.Models.DTOs.User;
using WebApplication1.Models.Queries;

namespace WebApplication1.Services
{
    /// <summary>
    /// 用户服务接口
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// 根据ID获取用户
        /// </summary>
        Task<SingleResponse<UserDto>> GetUserByIdAsync(int userId);

        /// <summary>
        /// 查询用户列表
        /// </summary>
        Task<PagedResponse<UserDto>> GetUsersAsync(UserQueryModel query);

        /// <summary>
        /// 创建用户
        /// </summary>
        Task<SingleResponse<UserDto>> CreateUserAsync(CreateUserDto createUserDto);

        ///// <summary>
        ///// 更新用户
        ///// </summary>
        Task<SingleResponse<UserDto>> UpdateUserAsync(int userId, UpdateUserDto updateUserDto);

        /// <summary>
        /// 删除用户
        /// </summary>
        Task<SingleResponse<bool>> DeleteUserAsync(int userId);
    }
}

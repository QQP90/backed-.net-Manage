using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WebApplication1.Models.DTOs;
using WebApplication1.Models.DTOs.User;
using WebApplication1.Models.Queries;
using WebApplication1.Models.Queries;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    /// <summary>
    /// 用户管理接口
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// 获取用户列表（支持分页、搜索、排序）
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetUsers([FromQuery] UserQueryModel query)
        {
            var response = await _userService.GetUsersAsync(query);
            return Ok(response);
        }

        /// <summary>
        /// 根据ID获取用户详情
        /// </summary>
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserById(int userId)
        {
            var response = await _userService.GetUserByIdAsync(userId);

            if (!response.Success)
                return NotFound(response);

            return Ok(response);
        }

        /// <summary>
        /// 创建用户
        /// </summary>


        /// <summary>
        /// 更新用户 ✅ 新增接口
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="updateUserDto">更新用户数据</param>
        /// <returns>更新后的用户</returns>
        [HttpPut("{userId}")]
        public async Task<IActionResult> UpdateUser(int userId, [FromBody] UpdateUserDto updateUserDto)
        {
            // 1. 验证模型状态
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // 2. 验证userId有效性
            if (userId <= 0)
                return BadRequest(new { message = "用户ID必须大于0" });

            // 3. 调用Service层更新
            var response = await _userService.UpdateUserAsync(userId, updateUserDto);

            // 4. 根据响应返回相应的HTTP状态码
            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }


        /// <summary>
        /// 删除用户
        /// </summary>
        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            var response = await _userService.DeleteUserAsync(userId);

            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }
    }
}
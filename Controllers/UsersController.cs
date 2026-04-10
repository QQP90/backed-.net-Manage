using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WebApplication1.Models.DTOs.User;
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
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto createUserDto)
        {
            // 第 1 步：Controller 校验请求体（[ApiController] 会自动做一部分校验，这里显式判断更清晰）
            // 如果不校验：无效数据会进入 Service/Repository，最终变成 500 或脏数据
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // 第 2 步：调用业务层创建用户
            // 业务规则（例如用户名/邮箱是否重复、密码哈希）必须在 Service 做，Controller 不写业务逻辑
            var response = await _userService.CreateUserAsync(createUserDto);

            // 第 3 步：根据业务结果返回合适的 HTTP 状态码
            // 如果不区分：前端只能得到 200/500，无法知道是“参数错”还是“重复”还是“系统故障”
            if (!response.Success)
            {
                if (response.Message != null &&
                    (response.Message.Contains("已存在") || response.Message.Contains("重复")))
                {
                    return Conflict(response);
                }

                return BadRequest(response);
            }

            // 第 4 步：CreatedAtAction 返回 201，并带上 Location（能直接跳转到新用户详情）
            // 如果只 return Ok：语义上不是“创建”，并且前端很难拿到资源定位
            return CreatedAtAction(nameof(GetUserById), new { userId = response.Data.UserId }, response);
        }

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

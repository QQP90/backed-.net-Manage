using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Models;
using WebApplication1.Models.DTOs.User;
using WebApplication1.Models.Entities;
using WebApplication1.Models.Queries;
using WebApplication1.Repositories;
using WebApplication1.Models.DTOs;
using WebApplication1.Models.Entities;
using WebApplication1.Models.Queries;
using WebApplication1.Repositories;

namespace WebApplication1.Services
{
    /// <summary>
    /// 用户服务实现
    /// </summary>
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UserService(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<SingleResponse<UserDto>> GetUserByIdAsync(int userId)
        {
            try
            {
                var user = await _userRepository.GetUserByIdAsync(userId);

                if (user == null)
                {
                    return new SingleResponse<UserDto>
                    {
                        Success = false,
                        Message = "用户不存在",
                        Data = null
                    };
                }

                var userDto = _mapper.Map<UserDto>(user);

                return new SingleResponse<UserDto>
                {
                    Success = true,
                    Message = "获取成功",
                    Data = userDto
                };
            }
            catch (Exception ex)
            {
                return new SingleResponse<UserDto>
                {
                    Success = false,
                    Message = $"获取用户失败: {ex.Message}",
                    Data = null
                };
            }
        }

        public async Task<PagedResponse<UserDto>> GetUsersAsync(UserQueryModel query)
        {
            try
            {
                var (users, totalCount) = await _userRepository.GetUsersAsync(query);

                var userDtos = _mapper.Map<List<UserDto>>(users);

                var totalPages = (totalCount + query.PageSize - 1) / query.PageSize;

                return new PagedResponse<UserDto>
                {
                    Success = true,
                    Message = "查询成功",
                    Data = userDtos,
                    TotalCount = totalCount,
                    PageNumber = query.PageNumber,
                    PageSize = query.PageSize,
                    TotalPages = totalPages
                };
            }
            catch (Exception ex)
            {
                return new PagedResponse<UserDto>
                {
                    Success = false,
                    Message = $"查询用户失表: {ex.Message}",
                    Data = new List<UserDto>()
                };
            }
        }

        /// <summary>
        /// 更新用户
        /// </summary>
        public async Task<SingleResponse<UserDto>> UpdateUserAsync(int userId, UpdateUserDto updateUserDto)
        {
            try
            {
                // 1. 参数验证
                if (updateUserDto == null)
                {
                    return new SingleResponse<UserDto>
                    {
                        Success = false,
                        Message = "更新数据不能为空",
                        Data = null
                    };
                }

                // 2. 检查用户是否存在
                var existingUser = await _userRepository.GetUserByIdAsync(userId);
                if (existingUser == null)
                {
                    return new SingleResponse<UserDto>
                    {
                        Success = false,
                        Message = "用户不存在",
                        Data = null
                    };
                }

                // 3. 使用 AutoMapper 映射数据（只映射非null字段）
                _mapper.Map(updateUserDto, existingUser);

                // 4. 调用Repository更新
                var result = await _userRepository.UpdateUserAsync(existingUser);

                if (!result)
                {
                    return new SingleResponse<UserDto>
                    {
                        Success = false,
                        Message = "更新用户失败",
                        Data = null
                    };
                }

                // 5. 重新获取更新后的用户（确保返回最新数据）
                var updatedUser = await _userRepository.GetUserByIdAsync(userId);
                var userDto = _mapper.Map<UserDto>(updatedUser);

                return new SingleResponse<UserDto>
                {
                    Success = true,
                    Message = "用户更新成功",
                    Data = userDto
                };
            }
            catch (Exception ex)
            {
                return new SingleResponse<UserDto>
                {
                    Success = false,
                    Message = $"更新用户失败: {ex.Message}",
                    Data = null
                };
            }
        }

        public async Task<SingleResponse<bool>> DeleteUserAsync(int userId)
        {
            try
            {
                var user = await _userRepository.GetUserByIdAsync(userId);
                if (user == null)
                {
                    return new SingleResponse<bool>
                    {
                        Success = false,
                        Message = "用户不存在",
                        Data = false
                    };
                }

                var result = await _userRepository.DeleteUserAsync(userId);

                return new SingleResponse<bool>
                {
                    Success = result,
                    Message = result ? "删除成功" : "删除失败",
                    Data = result
                };
            }
            catch (Exception ex)
            {
                return new SingleResponse<bool>
                {
                    Success = false,
                    Message = $"删除用户失败: {ex.Message}",
                    Data = false
                };
            }
        }

        /// <summary>
        /// 密码哈希（简单示例，生产环境应使用BCrypt等）
        /// </summary>
        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }
    }
}
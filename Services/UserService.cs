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
        /// 创建用户
        /// </summary>
        public async Task<SingleResponse<UserDto>> CreateUserAsync(CreateUserDto createUserDto)
        {
            try
            {
                // 第 1 步：参数校验（Controller 也会做 ModelState 校验，这里再兜底）
                // 如果不校验：可能出现空用户名/空密码，后续写库会失败或产生脏数据
                if (createUserDto == null)
                {
                    return new SingleResponse<UserDto>
                    {
                        Success = false,
                        Message = "创建数据不能为空",
                        Data = null
                    };
                }

                var username = createUserDto.Username?.Trim() ?? string.Empty;
                var email = createUserDto.Email?.Trim() ?? string.Empty;

                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(email) ||
                    string.IsNullOrWhiteSpace(createUserDto.Password))
                {
                    return new SingleResponse<UserDto>
                    {
                        Success = false,
                        Message = "Username/Email/Password 不能为空",
                        Data = null
                    };
                }

                // 第 2 步：唯一性检查（避免用户名/邮箱重复）
                // 如果不做：会在 SaveChanges 时抛数据库异常（更难看懂，也不友好）
                var existingByUsername = await _userRepository.GetUserByUsernameAsync(username);
                if (existingByUsername != null)
                {
                    return new SingleResponse<UserDto>
                    {
                        Success = false,
                        Message = "用户名已存在",
                        Data = null
                    };
                }

                var existingByEmail = await _userRepository.GetUserByEmailAsync(email);
                if (existingByEmail != null)
                {
                    return new SingleResponse<UserDto>
                    {
                        Success = false,
                        Message = "邮箱已存在",
                        Data = null
                    };
                }

                // 第 3 步：密码哈希（绝不能存明文密码）
                // 如果不做：数据库泄露时会直接暴露所有用户密码
                var passwordHash = HashPassword(createUserDto.Password);

                // BCrypt 自带 salt，PasswordSalt 这里保留字段但不再额外存储独立 salt（保持兼容）
                var user = new User
                {
                    Username = username,
                    Email = email,
                    PhoneNumber = createUserDto.PhoneNumber?.Trim() ?? string.Empty,
                    FirstName = createUserDto.FirstName?.Trim() ?? string.Empty,
                    LastName = createUserDto.LastName?.Trim() ?? string.Empty,
                    PasswordHash = passwordHash,
                    PasswordSalt = string.Empty,
                    Gender = createUserDto.Gender?.Trim() ?? string.Empty,
                    DateOfBirth = createUserDto.DateOfBirth,
                    Avatar = createUserDto.Avatar?.Trim() ?? string.Empty,
                    Bio = createUserDto.Bio?.Trim() ?? string.Empty,
                    Address = createUserDto.Address?.Trim() ?? string.Empty,
                    City = createUserDto.City?.Trim() ?? string.Empty,
                    State = createUserDto.State?.Trim() ?? string.Empty,
                    ZipCode = createUserDto.ZipCode?.Trim() ?? string.Empty,
                    Country = createUserDto.Country?.Trim() ?? string.Empty,
                    IsActive = createUserDto.IsActive ?? true,
                    IsEmailVerified = false,
                    IsPhoneVerified = false,
                    LastLoginTime = null,
                    DeletedAt = null,
                    Remarks = string.Empty
                };

                // 第 4 步：写入数据库（真正 INSERT 的地方在 Repository.SaveChangesAsync）
                var created = await _userRepository.CreateUserAsync(user);

                // 第 5 步：返回给前端 DTO（避免把 PasswordHash 之类敏感字段返回出去）
                var userDto = _mapper.Map<UserDto>(created);

                return new SingleResponse<UserDto>
                {
                    Success = true,
                    Message = "创建成功",
                    Data = userDto
                };
            }
            catch (Exception ex)
            {
                return new SingleResponse<UserDto>
                {
                    Success = false,
                    Message = $"创建用户失败: {ex.Message}",
                    Data = null
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

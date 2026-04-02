using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Infrastructure;
using WebApplication1.Models.Entities;
using WebApplication1.Models.Queries;

namespace WebApplication1.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _dbContext;

        public UserRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<User> GetUserByIdAsync(int userId)
        {
            return await _dbContext.Users
                .Where(u => u.DeletedAt == null)
                .FirstOrDefaultAsync(u => u.UserId == userId);
        }

        public async Task<User> GetUserByUsernameAsync(string username)
        {
            return await _dbContext.Users
                .Where(u => u.DeletedAt == null)
                .FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _dbContext.Users
                .Where(u => u.DeletedAt == null)
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<(List<User> Items, int TotalCount)> GetUsersAsync(UserQueryModel query)
        {
            var queryable = _dbContext.Users
                .Where(u => u.DeletedAt == null);

            // 搜索条件
            if (!string.IsNullOrWhiteSpace(query.SearchKeyword))
            {
                queryable = queryable.Where(u =>
                    u.Username.Contains(query.SearchKeyword) ||
                    u.Email.Contains(query.SearchKeyword) ||
                    u.PhoneNumber.Contains(query.SearchKeyword));
            }

            if (!string.IsNullOrWhiteSpace(query.Username))
            {
                queryable = queryable.Where(u => u.Username.Contains(query.Username));
            }

            if (!string.IsNullOrWhiteSpace(query.Email))
            {
                queryable = queryable.Where(u => u.Email.Contains(query.Email));
            }

            if (query.IsActive.HasValue)
            {
                queryable = queryable.Where(u => u.IsActive == query.IsActive.Value);
            }

            // 获取总数
            var totalCount = await queryable.CountAsync();

            // 排序
            queryable = query.SortOrder.ToLower() == "asc"
                ? queryable.OrderBy(GetSortProperty(query.SortBy))
                : queryable.OrderByDescending(GetSortProperty(query.SortBy));

            // 分页
            var items = await queryable
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<User> CreateUserAsync(User user)
        {
            user.CreatedAt = DateTime.Now;
            user.UpdatedAt = DateTime.Now;

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            return user;
        }

        public async Task<bool> UpdateUserAsync(User user)
        {
            user.UpdatedAt = DateTime.Now;
            _dbContext.Users.Update(user);

            var result = await _dbContext.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> DeleteUserAsync(int userId)
        {
            var user = await GetUserByIdAsync(userId);
            if (user == null)
                return false;

            user.DeletedAt = DateTime.Now;
            _dbContext.Users.Update(user);

            var result = await _dbContext.SaveChangesAsync();
            return result > 0;
        }

        /// <summary>
        /// 获取排序属性
        /// </summary>
        private System.Linq.Expressions.Expression<Func<User, dynamic>> GetSortProperty(string sortBy)
        {
            return sortBy?.ToLower() switch
            {
                "username" => u => u.Username,
                "email" => u => u.Email,
                "createdat" => u => u.CreatedAt,
                "updatedat" => u => u.UpdatedAt,
                _ => u => u.CreatedAt
            };
        }
    }
}

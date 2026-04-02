namespace WebApplication1.Models.DTOs.User
{
    /// <summary>
    /// 用户查询DTO - 用于返回给前端的数据
    /// </summary>
    public class UserDto
    {
        public int UserId { get; set; }

        public string Username { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Gender { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public string Avatar { get; set; }

        public string Bio { get; set; }

        public string City { get; set; }

        public string Country { get; set; }

        public bool IsActive { get; set; }

        public bool IsEmailVerified { get; set; }

        public DateTime? LastLoginTime { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}

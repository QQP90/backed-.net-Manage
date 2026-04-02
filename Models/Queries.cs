namespace WebApplication1.Models.Queries
{
    public class UserQueryModel
    {
        /// <summary>
        /// 搜索关键词（用户名、邮箱、电话号码）
        /// </summary>
        public string SearchKeyword { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 是否激活
        /// </summary>
        public bool? IsActive { get; set; }

        /// <summary>
        /// 页码
        /// </summary>
        public int PageNumber { get; set; } = 1;

        /// <summary>
        /// 每页数量
        /// </summary>
        public int PageSize { get; set; } = 10;

        /// <summary>
        /// 排序字段
        /// </summary>
        public string SortBy { get; set; } = "CreatedAt";

        /// <summary>
        /// 排序方向（asc/desc）
        /// </summary>
        public string SortOrder { get; set; } = "desc";
    }
}

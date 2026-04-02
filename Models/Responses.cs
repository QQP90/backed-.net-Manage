using System.Collections.Generic;

namespace WebApplication1.Models
{
    /// <summary>
    /// 分页响应
    /// </summary>
    public class PagedResponse<T>
    {
        public bool Success { get; set; }

        public string Message { get; set; }

        public int TotalCount { get; set; }

        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public int TotalPages { get; set; }

        public List<T> Data { get; set; }
    }

    /// <summary>
    /// 单条响应
    /// </summary>
    public class SingleResponse<T>
    {
        public bool Success { get; set; }

        public string Message { get; set; }

        public T Data { get; set; }
    }
}

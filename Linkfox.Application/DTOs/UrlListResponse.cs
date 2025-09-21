using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkFox.Application.DTOs
{
    /// <summary>
    /// Respose DTO For Url List with Pagination, Searching and Sorting.
    /// </summary>
    public class UrlListResponse
    {
        public long Id { get; set; }
        public string ShortCode { get; set; } = string.Empty;
        public string ShortUrl { get; set; } = string.Empty;
        public string LongUrl { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? ExpireAt { get; set; }
        public long ClickCount { get; set; }
    }

    public class PagedList<T>
    {
        public IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    }
}

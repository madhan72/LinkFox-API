using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkFox.Application.DTOs
{
    /// <summary>
    /// Request DTO For Url List with Pagination, Searching and Sorting.
    /// </summary>
    public class UrlListRequest
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? Search { get; set; }
        public string SortBy { get; set; }
        public string SortOrder { get; set; }
    }
}

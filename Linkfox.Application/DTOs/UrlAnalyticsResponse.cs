using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkFox.Application.DTOs
{
    public class UrlAnalyticsResponse
    {
        public string ShortCode { get; set; } = string.Empty;
        public string ShortUrl { get; set; } = string.Empty;
        public string LongUrl { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public long ClickCount { get; set; }
        public DateTime? LastAccessed { get; set; }
        public List<ClickDetail> Clicks { get; set; } = new();
    }

    public class ClickDetail
    {
        public DateTime ClickedAt { get; set; }
        public string IpAddress { get; set; } = string.Empty;
        public string UserAgent { get; set; } = string.Empty;
    }
}

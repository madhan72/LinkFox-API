using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linkfox.domain
{
    /// <summary>
    /// Click entity maps to dbo.Clicks table and stores per-redirect info.
    /// </summary>
    public class Click
    {
        public long Id { get; set; }

        // FK to Url.Id
        public long UrlId { get; set; }
        public Url Url { get; set; } = null!;

        // When the click happened. Default provided by SQL: SYSUTCDATETIME()
        public DateTime ClickedAt { get; set; }

        // IP Address (supports IPv6)
        public string IpAddress { get; set; } = null!;

        public string? UserAgent { get; set; }
        public string? Referrer { get; set; }
        public string? AcceptLanguage { get; set; }
        public string? Country { get; set; }
        public string? City { get; set; }
        public string? DeviceCategory { get; set; }
    }
}

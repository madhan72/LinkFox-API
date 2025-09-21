using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linkfox.domain
{
    /// <summary>
    /// Url entity maps to dbo.Urls table.
    /// </summary>
    public class Url
    {
        // Primary key
        public long Id { get; set; }

        // Short code.
        public string ShortCode { get; set; } = null!;

        // Target URL
        public string LongUrl { get; set; } = null!;

        // Created timestamp  
        public DateTime CreatedAt { get; set; }

        // Click Count
        public long ClickCount { get; set; }

        // Navigation: clicks
        public List<Click> Clicks { get; set; } = new();
    }
}

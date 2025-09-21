using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkFox.Application.Utils
{
    /// <summary>
    /// Get Device category by UserAgent from request
    /// </summary>
    /// <param name="userAgent"></param>
    /// <returns></returns>
    public class DeviceDetector
    {
        public static string GetDeviceCategory(string? userAgent)
        {
            if (string.IsNullOrWhiteSpace(userAgent))
                return "Unknown";

            var ua = userAgent.ToLower();

            if (ua.Contains("mobile") || ua.Contains("android") || ua.Contains("iphone"))
                return "Mobile";

            if (ua.Contains("ipad") || ua.Contains("tablet"))
                return "Tablet";

            if (ua.Contains("windows") || ua.Contains("macintosh") || ua.Contains("linux"))
                return "Desktop";

            if (ua.Contains("bot") || ua.Contains("spider") || ua.Contains("crawl"))
                return "Bot";

            return "Other";
        }
    }
}

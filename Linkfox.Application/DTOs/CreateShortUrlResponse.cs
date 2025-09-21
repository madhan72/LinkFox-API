using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkFox.Application.DTOs
{
    /// <summary>
    /// Response DTO returned after creation.
    /// </summary>
    public record CreateShortUrlResponse(long Id, string ShortCode, string ShortUrl, string LongUrl);
}

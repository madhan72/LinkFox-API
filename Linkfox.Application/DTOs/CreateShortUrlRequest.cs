using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkFox.Application.DTOs
{
    /// <summary>
    /// Request DTO for creating a shortened URL.
    /// Optionally accepts an alias (ShortCode). If alias is null, put a short code.
    /// </summary>
    public record CreateShortUrlRequest(string LongUrl, string? Alias=null);
}

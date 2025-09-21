using LinkFox.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkFox.Application.Interface
{
    /// <summary>
    /// Service interface for URL shortening and retrieval operations.
    /// </summary>
    public interface IUrlService
    {
        Task<Result<CreateShortUrlResponse>> CreateShortUrlAsync(CreateShortUrlRequest request, string origin);
        Task<string?> GetLongUrlAndRecordClickAsync(string shortCode, string ipAddress, string? userAgent, string? referrer, string? acceptLanguage);
        Task<Result<PagedList<UrlListResponse>>> GetUrlsAsync(UrlListRequest request, string origin);
    }
}

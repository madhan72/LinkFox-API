using Linkfox.domain;
using LinkFox.Application.DTOs;
using LinkFox.Application.Interface;
using LinkFox.Application.Utils;
using Microsoft.Extensions.Logging;
using System.Data.SqlTypes;

namespace LinkFox.Application.Services
{
    /// <summary>
    /// Creating short Urls and Resolving shortcodes to long Urls.
    /// Coordinate with repo operation and short-code generation.
    /// </summary>
    public class UrlService : IUrlService
    {
        private readonly IUrlRepository _repo;
        private readonly ILogger<UrlService> _logger;
        private readonly string _defaultHost;

        public UrlService(IUrlRepository repo,ILogger<UrlService> logger)
        {
            _repo = repo;
            _logger = logger;
            _defaultHost = ""; 
        }

        /// <summary>
        /// Creates a short URL. If Alias provided, checks uniqueness.
        /// If not, we persist to get numeric Id and produce base62 ShortCode from Id.
        /// </summary>
        /// 
        public async Task<Result<CreateShortUrlResponse>> CreateShortUrlAsync(CreateShortUrlRequest request, string origin)
        {
            if (string.IsNullOrWhiteSpace(request.LongUrl))
                throw new BadRequestException("LongUrl is required");

            _logger.LogInformation("Creating short URL for {LongUrl}", request.LongUrl);

            // If alias is provided, check uniqueness first
            if (!string.IsNullOrWhiteSpace(request.Alias))
            {
                var existing = await _repo.GetByShortCodeAsync(request.Alias);
                if (existing != null)
                {
                    throw new ConflictException($"Alias '{request.Alias}' is already in use.");
                }
            }

            // Create entity without ShortCode (we'll set it after insert to use identity Id)
            var url = new Url
            {
                LongUrl = request.LongUrl,
                ShortCode = request.Alias ?? string.Empty, // temporary placeholder
                CreatedAt = DateTime.UtcNow
            };

            // Persist to get Id (IDENTITY)
            var saved = await _repo.AddAsync(url);
            await _repo.SaveChangesAsync(); // ensure Id is assigned

            //If alias is not provided, generate ShortCode from numeric Id
            if (string.IsNullOrWhiteSpace(request.Alias))
            {
                var code = Base62.Encode(saved.Id);
                saved.ShortCode = code;
                await _repo.UpdateAsync(saved);
                await _repo.SaveChangesAsync();
            }

            // Build full short URL
            var shortUrl = origin?.TrimEnd('/') + "/r/" + saved.ShortCode;

            return Result<CreateShortUrlResponse>.Ok(new CreateShortUrlResponse(saved.Id, saved.ShortCode, shortUrl, saved.LongUrl));
        }

        public async Task<string?> GetLongUrlAndRecordClickAsync(string shortCode, string ipAddress, string? userAgent, string? referrer, string? acceptLanguage)
        {
            var url = await _repo.GetByShortCodeAsync(shortCode);
            if (url == null) return null;

            // Create click record
            var click = new Click
            {
                UrlId = url.Id,
                IpAddress = ipAddress ?? "unknown",
                UserAgent = userAgent,
                Referrer = referrer,
                AcceptLanguage = acceptLanguage,
                ClickedAt = DateTime.UtcNow // application-level timestamp; DB also has default
            };

            await _repo.AddClickAsync(click);

            // Increment aggregated count and persist
            url.ClickCount += 1;
            await _repo.UpdateAsync(url);

            // Save both click and url update
            await _repo.SaveChangesAsync();

            _logger.LogInformation("Recorded click for ShortCode {ShortCode} from IP {IP}", shortCode, ipAddress);

            return url.LongUrl;
        }

        public async Task<Result<PagedList<UrlListResponse>>> GetUrlsAsync(UrlListRequest request, string origin)
        {
            _logger.LogInformation("Fetching paged URL list: Page {Page}, Size {PageSize}, Search {Search}",
                request.PageNumber, request.PageSize, request.Search);

            var (items, total) = await _repo.GetPagedUrlList(request.PageNumber, request.PageSize,
                request.Search, request.SortBy, request.SortOrder);

            var result = items.Select(u => new UrlListResponse
            {
                Id = u.Id,
                ShortCode = u.ShortCode,
                ShortUrl = origin.TrimEnd('/') + "/r/" + u.ShortCode,
                LongUrl = u.LongUrl,
                CreatedAt = u.CreatedAt,
                ClickCount = u.ClickCount,
            });

            var pagedList = new PagedList<UrlListResponse>
            { 
                Items = result, 
                Page = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = total
            };

            return Result<PagedList<UrlListResponse>>.Ok(pagedList);
        }
    }
}

using LinkFox.Application.DTOs;
using LinkFox.Application.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LinkFox.Api.Controllers
{
    //[Route("api/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class UrlsController : ControllerBase
    {
        private readonly IUrlService _service;
        private readonly ILogger<UrlsController> _logger;

        public UrlsController(IUrlService service, ILogger<UrlsController> logger)
        {
            _service = service;
            _logger = logger;
        }

        ///<summary>
        /// Create a short Url
        ///</summary>
        [HttpPost("CreateShortUrl")]
        public async Task<IActionResult> Create([FromBody] CreateShortUrlRequest request)
        {
            try
            {
                // Build origin to create full short Url
                var origin = $"{Request.Scheme}: // {Request.Host.Value}";

                var result = await _service.CreateShortUrlAsync(request, origin);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, " Error creating short url");
                throw;
            }
        }

        ///<summary>
        ///Redirect EndPoint
        ///Finds the long url using short code and record click info
        /// </summary>
        [HttpGet("/r/{shortCode}")]
        public async Task<IActionResult> RedirectToLongUrl([FromRoute] string shortCode)
        {
            try
            {
                var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
                var userAgent = Request.Headers["User-Agent"].ToString();
                var referer = Request.Headers["Referer"].ToString() ;

                var longUrl = await _service.GetLongUrlAndRecordClickAsync(shortCode, ip, userAgent, referer,"");

                if (longUrl == null) return NotFound();
                
                // Redirect(302) to LongUrl
                return Redirect(longUrl);
                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while redirecting for {ShortCode}", shortCode);
                return Problem(detail: ex.Message, statusCode:500);
            }
        }

        /// <summary>
        /// List of Urls / with pagination support
        /// </summary>
        [HttpGet("list")]
        public async Task<IActionResult> List([FromQuery] UrlListRequest request)
        {
            var origin = $"{Request.Scheme}://{Request.Host}";
            var result = await _service.GetUrlsAsync(request, origin);
            return Ok(result);
        }

    }
}

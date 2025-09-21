using LinkFox.Application.DTOs;
using LinkFox.Application.Interface;
using LinkFox.Application.Services;
using Linkfox.domain;
using Microsoft.Extensions.Logging;
using Moq;

namespace LinkFox.Application.Tests.Services
{
    public class UrlServiceTests
    {
        private readonly Mock<IUrlRepository> _repoMock;
        private readonly Mock<ILogger<UrlService>> _loggerMock;
        private readonly UrlService _service;

        public UrlServiceTests()
        {
            _repoMock = new Mock<IUrlRepository>();
            _loggerMock = new Mock<ILogger<UrlService>>();
            _service = new UrlService(_repoMock.Object, _loggerMock.Object);
        }

        /// <summary>
        /// Tests that CreateShortUrlAsync throws BadRequestException when LongUrl is missing.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task CreateShortUrlAsync_ThrowsBadRequest_WhenLongUrlIsMissing()
        {
            var request = new CreateShortUrlRequest(null, null);
            await Assert.ThrowsAsync<BadRequestException>(() =>
                _service.CreateShortUrlAsync(request, "https://host"));
        }

        /// <summary>
        /// Tests that CreateShortUrlAsync throws ConflictException when Alias already exists.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task CreateShortUrlAsync_ThrowsConflict_WhenAliasExists()
        {
            var request = new CreateShortUrlRequest("https://test.com", "abc");
            _repoMock.Setup(r => r.GetByShortCodeAsync("abc"))
                .ReturnsAsync(new Url { Id = 1, ShortCode = "abc", LongUrl = "https://test.com" });

            await Assert.ThrowsAsync<ConflictException>(() =>
                _service.CreateShortUrlAsync(request, "https://host"));
        }

        /// <summary>
        /// Tests that CreateShortUrlAsync successfully creates a short URL with provided alias.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task CreateShortUrlAsync_CreatesShortUrl_WithAlias()
        {
            var request = new CreateShortUrlRequest("https://test.com", "myalias");
            _repoMock.Setup(r => r.GetByShortCodeAsync("myalias")).ReturnsAsync((Url)null!);
            _repoMock.Setup(r => r.AddAsync(It.IsAny<Url>()))
                .ReturnsAsync((Url u) => { u.Id = 123; return u; });
            _repoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            var result = await _service.CreateShortUrlAsync(request, "https://host");

            Assert.True(result.Success);
            Assert.Equal("myalias", result.Data!.ShortCode);
            Assert.Equal("https://host/r/myalias", result.Data.ShortUrl);
            Assert.Equal("https://test.com", result.Data.LongUrl);
        }

        /// <summary>
        /// Tests that CreateShortUrlAsync successfully creates a short URL without alias, generating ShortCode from Id.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task CreateShortUrlAsync_CreatesShortUrl_WithoutAlias()
        {
            var request = new CreateShortUrlRequest("https://test.com", null);
            _repoMock.Setup(r => r.AddAsync(It.IsAny<Url>()))
                .ReturnsAsync((Url u) => { u.Id = 42; return u; });
            _repoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
            _repoMock.Setup(r => r.UpdateAsync(It.IsAny<Url>())).Returns(Task.CompletedTask);

            var result = await _service.CreateShortUrlAsync(request, "https://host");

            Assert.True(result.Success);
            Assert.Equal("G", result.Data!.ShortCode);
            Assert.Equal("https://host/r/G", result.Data.ShortUrl);
            Assert.Equal("https://test.com", result.Data.LongUrl);
        }

        /// <summary>
        /// Tests that GetLongUrlAndRecordClickAsync returns null when ShortCode not found.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetLongUrlAndRecordClickAsync_ReturnsNull_WhenShortCodeNotFound()
        {
            _repoMock.Setup(r => r.GetByShortCodeAsync("notfound")).ReturnsAsync((Url)null!);

            var result = await _service.GetLongUrlAndRecordClickAsync("notfound", "1.2.3.4", null, null, null);

            Assert.Null(result);
        }

        /// <summary>
        /// Tests that GetLongUrlAndRecordClickAsync records a click and returns the LongUrl.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetLongUrlAndRecordClickAsync_RecordsClick_AndReturnsLongUrl()
        {
            var url = new Url { Id = 1, ShortCode = "abc", LongUrl = "https://test.com", ClickCount = 0 };
            _repoMock.Setup(r => r.GetByShortCodeAsync("abc")).ReturnsAsync(url);
            _repoMock.Setup(r => r.AddClickAsync(It.IsAny<Click>())).Returns(Task.CompletedTask);
            _repoMock.Setup(r => r.UpdateAsync(It.IsAny<Url>())).Returns(Task.CompletedTask);
            _repoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            // Mock DeviceDetector
            DeviceDetector.SetDeviceCategoryResolver(_ => "Desktop");

            var result = await _service.GetLongUrlAndRecordClickAsync("abc", "1.2.3.4", "UA", "ref", "en");

            Assert.Equal("https://test.com", result);
            Assert.Equal(1, url.ClickCount);
        }


        /// <summary>
        /// Tests that GetUrlsAsync returns a paged list of URLs.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetUrlsAsync_ReturnsPagedList()
        {
            var urls = new List<Url>
            {
                new Url { Id = 1, ShortCode = "a", LongUrl = "https://a.com", CreatedAt = DateTime.UtcNow, ClickCount = 2 },
                new Url { Id = 2, ShortCode = "b", LongUrl = "https://b.com", CreatedAt = DateTime.UtcNow, ClickCount = 3 }
            };
            _repoMock.Setup(r => r.GetPagedUrlList(1, 10, null, null, null))
                .ReturnsAsync((urls, 2));

            var request = new UrlListRequest { PageNumber = 1, PageSize = 10 };
            var result = await _service.GetUrlsAsync(request, "https://host");

            Assert.True(result.Success);
            Assert.Equal(2, result.Data!.Items.Count());
            Assert.All(result.Data.Items, u => Assert.StartsWith("https://host/r/", u.ShortUrl));
        }

        [Fact]
        public async Task GetAnalyticsByShortCodeAsync_ReturnsAnalyticsWithClicks()
        {
            // Arrange
            var shortCode = "abc123";
            var origin = "https://host";
            var url = new Url
            {
                Id = 1,
                ShortCode = shortCode,
                LongUrl = "https://test.com",
                CreatedAt = new DateTime(2024, 1, 1),
                ClickCount = 2,
                Clicks = new List<Click>
                {
                    new Click
                    {
                        ClickedAt = new DateTime(2024, 6, 1, 10, 0, 0),
                        IpAddress = "1.2.3.4",
                        UserAgent = "UA1"
                    },
                    new Click
                    {
                        ClickedAt = new DateTime(2024, 6, 2, 11, 0, 0),
                        IpAddress = "5.6.7.8",
                        UserAgent = "UA2"
                    }
                }
            };
            _repoMock.Setup(r => r.GetByShortCodeAsync(shortCode)).ReturnsAsync(url);

            // Act
            var result = await _service.GetAnalyticsByShortCodeAsync(shortCode, origin);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal(shortCode, result.Data!.ShortCode);
            Assert.Equal($"{origin}/r/{shortCode}", result.Data.ShortUrl);
            Assert.Equal(url.LongUrl, result.Data.LongUrl);
            Assert.Equal(url.CreatedAt, result.Data.CreatedAt);
            Assert.Equal(url.ClickCount, result.Data.ClickCount);
            Assert.Equal(new DateTime(2024, 6, 2, 11, 0, 0), result.Data.LastAccessed);
            Assert.Equal(2, result.Data.Clicks.Count);

            Assert.Equal(new DateTime(2024, 6, 2, 11, 0, 0), result.Data.Clicks[0].ClickedAt);
            Assert.Equal("5.6.7.8", result.Data.Clicks[0].IpAddress);
            Assert.Equal("UA2", result.Data.Clicks[0].UserAgent);

            Assert.Equal(new DateTime(2024, 6, 1, 10, 0, 0), result.Data.Clicks[1].ClickedAt);
            Assert.Equal("1.2.3.4", result.Data.Clicks[1].IpAddress);
            Assert.Equal("UA1", result.Data.Clicks[1].UserAgent);
        }

        /// <summary>
        /// Tests that GetAnalyticsByShortCodeAsync returns NotFound when the URL does not exist.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetAnalyticsByShortCodeAsync_ReturnsNotFound_WhenUrlDoesNotExist()
        {
            // Arrange
            var shortCode = "notfound";
            var origin = "https://host";
            _repoMock.Setup(r => r.GetByShortCodeAsync(shortCode)).ReturnsAsync((Url)null!);

            // Act
            var result = await _service.GetAnalyticsByShortCodeAsync(shortCode, origin);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("NotFound", result.ErrorCode);
            Assert.Null(result.Data);
        }
    }

    // Helper for DeviceDetector mocking
    public static class DeviceDetector
    {
        private static Func<string?, string?>? _resolver;
        public static void SetDeviceCategoryResolver(Func<string?, string?> resolver) => _resolver = resolver;
        public static string? GetDeviceCategory(string? userAgent) => _resolver?.Invoke(userAgent) ?? "Unknown";
    }

}

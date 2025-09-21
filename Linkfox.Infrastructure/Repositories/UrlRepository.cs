using Linkfox.domain;
using LinkFox.Application.Interface;
using LinkFox.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkFox.Infrastructure.Repositories
{
    /// <summary>
    /// Implementation of IUrlRepository
    /// Keeps Database access as isolated
    /// </summary>
    public class UrlRepository : IUrlRepository
    {

        private readonly AppDbContext _db;
        private readonly ILogger<UrlRepository> _logger;

        public UrlRepository(AppDbContext db, ILogger<UrlRepository> logger)
        {
            _db = db;
            _logger = logger;
        }

        /// <summary>
        /// Get Url by ShortCode
        /// </summary>
        /// <param name="shortCode"></param>
        /// <returns></returns>
        public async Task<Url?> GetByShortCodeAsync(string shortCode)
        {
            return await _db.Urls.Include(u => u.Clicks)
                                 .FirstOrDefaultAsync(u => u.ShortCode == shortCode);
        }


        /// <summary>
        /// Add new Url entity
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public async Task<Url> AddAsync(Url url)
        {
            var entry = await _db.Urls.AddAsync(url);
            return entry.Entity;
        }


        /// <summary>
        /// Update existing Url entity
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public async Task UpdateAsync(Url url)
        {
            _db.Urls.Update(url);
            await Task.CompletedTask;
        }


        /// <summary>
        /// Add new Click entity
        /// </summary>
        /// <param name="click"></param>
        /// <returns></returns>
        public async Task AddClickAsync(Click click)
        {
            await _db.Clicks.AddAsync(click);
        }


        /// <summary>
        /// Persist changes to database
        /// </summary>
        /// <returns></returns>
        public async Task SaveChangesAsync()
        {
            // In a real app you may want to add retry logic or wrap in transaction.
            await _db.SaveChangesAsync();
        }


        /// <summary>
        /// Get Url by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Url?> GetByIdAsync(long id)
        {
            return await _db.Urls.Include(u => u.Clicks).FirstOrDefaultAsync(u => u.Id == id);
        }


        /// <summary>
        /// Get paged Url list with search and sorting
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchString"></param>
        /// <param name="sortBy"></param>
        /// <param name="sortOrder"></param>
        /// <returns></returns>
        public async Task<(IEnumerable<Url> Items, int totalCount)> GetPagedUrlList(int page, int pageSize, string? searchString, string sortBy, string sortOrder)
        {
            var query = _db.Urls.AsQueryable();


            //Search by Shortcode or LongUrl
            if (!string.IsNullOrWhiteSpace(searchString))
            {
                query = query.Where(u => u.ShortCode.Contains(searchString) || u.LongUrl.Contains(searchString));
            }

            //Sorting
            query = (sortBy.ToLower(), sortOrder.ToLower()) switch
            {
                ("clickcount", "asc") => query.OrderBy(u => u.ClickCount),
                ("clickcount", "desc") => query.OrderByDescending(u => u.ClickCount),
                ("shortcode", "asc") => query.OrderBy(u => u.ShortCode),
                ("shortcode", "desc") => query.OrderByDescending(u => u.ShortCode),
                ("createdat", "asc") => query.OrderBy(u => u.ClickCount),
                _ => query.OrderByDescending( u=> u.CreatedAt) //default sorting
            };

            var total = await query.CountAsync();

            var items = await query.Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return(items, total);

        }
    }
}

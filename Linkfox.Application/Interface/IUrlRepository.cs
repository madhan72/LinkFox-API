using Linkfox.domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkFox.Application.Interface
{
    /// <summary>
    /// Repository abstraction for Url and Click class
    /// </summary>
    public interface IUrlRepository
    {
        Task<Url?> GetByShortCodeAsync(string shortCode);
        Task<Url?> GetByIdAsync(long id);
        Task<Url> AddAsync(Url url);
        Task UpdateAsync(Url url);
        Task AddClickAsync(Click click);
        Task SaveChangesAsync();
        Task<(IEnumerable<Url> Items, int totalCount)> GetPagedUrlList(int page, int pageSize, string? searchString,
            string sortBy, string sortOrder);
    }
}

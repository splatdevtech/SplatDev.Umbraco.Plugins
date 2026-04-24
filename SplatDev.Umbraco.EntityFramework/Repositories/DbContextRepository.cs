using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using SplatDev.Umbraco.Pagination.Extensions;
using SplatDev.Umbraco.Pagination.Models;

namespace SplatDev.Umbraco.EntityFramework.Repositories
{
    public class DbContextRepository<TEntity>(
        DbContext context,
        ILogger<DbContextRepository<TEntity>> logger) : IRepository<TEntity> where TEntity : class
    {
        private readonly DbContext _dbContext = context;
        private readonly ILogger<DbContextRepository<TEntity>> _logger = logger;

        public async Task<TEntity> CreateAsync(TEntity entity)
        {
            try
            {
                await _dbContext.Set<TEntity>().AddAsync(entity);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Could not save Entity");
            }
            return entity;
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity is null) return;

            _dbContext.Set<TEntity>().Remove(entity);
            await _dbContext.SaveChangesAsync();
        }

        public void Dispose() => _dbContext.Dispose();

        public virtual async Task<IList<TEntity>> FetchAsync(string query)
        {
            var items = await _dbContext.Set<TEntity>().FromSqlRaw(query).AsNoTracking().ToListAsync();
            return items;
        }

        public async Task<IList<TEntity>?> GetAllAsync()
        {
            var items = await _dbContext.Set<TEntity>().AsNoTracking().ToListAsync();
            return items;
        }

        public async Task<TEntity?> GetByIdAsync(int id)
        {
            var item = await _dbContext.Set<TEntity>().FindAsync(id);
            return item;
        }

        public async Task<PagedResults<TEntity>> GetPagedResultsAsync(int pageNumber, int pageSize)
        {
            var all = await GetAllAsync();
            int total = all?.Count ?? 0;
            IList<TEntity>? allPaged = all?.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            var pagedResults = new PagedResults<TEntity>
            {
                Results = allPaged,
                Pagination = new Pagination.Models.Pagination
                {
                    Page = pageNumber,
                    PageSize = pageSize,
                    TotalResults = total,
                    TotalPages = all is not null && all.Any() ? PaginationExtensions.GetTotalPages(all.Count, pageSize) : 0
                }
            };
            return pagedResults;
        }

        public async Task<TEntity> UpdateAsync(TEntity entity)
        {
            _dbContext.Set<TEntity>().Update(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task<int?> CountRecordsAsync()
        {
            await Task.FromResult(0);
            int count = _dbContext.Set<TEntity>().Count();
            return count;
        }
    }
}

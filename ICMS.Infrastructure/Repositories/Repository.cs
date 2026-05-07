using ICMS.Application.Interfaces.Repositories;
using ICMS.Domain.Entites.Common;
using ICMS.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace ICMS.Infrastructure.Repositories
{
    public class Repository<TEntity, TKey> : IRepository<TEntity, TKey> where TEntity : BaseEntity<TKey>
    {
        protected readonly AppDbContext _context;
        protected readonly DbSet<TEntity> _dbSet;

        public Repository(AppDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<TEntity>();
        }

        public IQueryable<TEntity> GetQueryable(bool track = false, CancellationToken cancellationToken = default, params Expression<Func<TEntity, object>>[] includes)
        {
            var query = track ? _dbSet : _dbSet.AsNoTracking();

            if (includes != null)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }

            return query;
        }

        public async Task<IReadOnlyList<TEntity>> GetAllAsync(bool track = false, CancellationToken cancellationToken = default, params Expression<Func<TEntity, object>>[] includes)
        {
            var query = GetQueryable(track, cancellationToken, includes);
            return await query.ToListAsync(cancellationToken);
        }

        public async Task<TEntity?> GetByIdAsync(TKey id, CancellationToken cancellationToken = default, bool track = true, params Expression<Func<TEntity, object>>[] includes)
        {
            var query = GetQueryable(track, cancellationToken, includes);
            return await query.FirstOrDefaultAsync(e => e.Id!.Equals(id), cancellationToken);
        }

        public async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            await _dbSet.AddAsync(entity, cancellationToken);
            return entity;
        }

        public async Task<IEnumerable<TEntity>> AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        {
            await _dbSet.AddRangeAsync(entities, cancellationToken);
            return entities;
        }

        public async Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            _dbSet.Update(entity);
            await Task.CompletedTask;
        }

        public void UpdateRange(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        {
            _dbSet.UpdateRange(entities);
        }

        public async Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            _dbSet.Remove(entity);
            await Task.CompletedTask;
        }

        public async Task<bool> ExistAsync(TKey id, CancellationToken cancellationToken = default)
        {
            return await _dbSet.AnyAsync(e => e.Id!.Equals(id), cancellationToken);
        }

        public async Task<bool> ExistAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await _dbSet.AnyAsync(predicate, cancellationToken);
        }

        public async Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await _dbSet.FirstOrDefaultAsync(predicate, cancellationToken);
        }

        public async Task<IReadOnlyList<TEntity>> GetPagedAsync(int pageNumber, int pageSize, bool track = false, CancellationToken cancellationToken = default, params Expression<Func<TEntity, object>>[] includes)
        {
            var query = GetQueryable(track, cancellationToken, includes);
            return await query
                .OrderBy(e => e.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

        }
    }
}

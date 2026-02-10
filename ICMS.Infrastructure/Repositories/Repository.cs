using ICMS.Application.Interfaces.Repositories;
using ICMS.Domain.Entites;
using ICMS.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
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
            var query = track ? _dbSet : _dbSet.AsNoTracking();


            if (includes != null)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }


            return await query.ToListAsync();
        }

        public async Task<TEntity?> GetByIdAsync(TKey id, CancellationToken cancellationToken = default)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            await _dbSet.AddAsync(entity);

            return entity;
        }

        public async Task<IEnumerable<TEntity>> AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        {
            await _dbSet.AddRangeAsync(entities);

            return entities;
        }


        public async Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            var existingEntity = await _dbSet.FindAsync(entity.Id);

            _dbSet.Update(entity);
        }

        public void UpdateRange(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        {
            _dbSet.UpdateRange(entities);
        }

        public async Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            _dbSet.Remove(entity);
        }

        public async Task<bool> ExistAsync(TKey id, CancellationToken cancellationToken = default)
        {
            return await _dbSet.FindAsync(id) != null;
        }

        public async Task<bool> ExistAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await _dbSet.FirstOrDefaultAsync(predicate) != null;
        }

    }
}

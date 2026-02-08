using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ICMS.Application.Interfaces.Repositories
{
    public interface IRepository<TEntity, TKey>
    {
        Task<IReadOnlyList<TEntity>> GetAllAsync(bool track = false,CancellationToken cancellationToken = default, params Expression<Func<TEntity, object>>[] includes);

        Task<TEntity?> GetByIdAsync(TKey id, CancellationToken cancellationToken = default);

        Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default);

        Task<IEnumerable<TEntity>> AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

        Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);
        void UpdateRange(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);
        Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);

        Task<bool> ExistAsync(TKey id, CancellationToken cancellationToken = default);
        Task<bool> ExistAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

        IQueryable<TEntity> GetQueryable(bool track = false, CancellationToken cancellationToken = default, params Expression<Func<TEntity, object>>[] includes);

    }
}

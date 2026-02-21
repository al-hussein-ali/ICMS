using ICMS.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ICMS.Application.Interfaces.Repositories
{
    public interface IDoseRepository : IRepository<Dose, int>
    {
        Task<IReadOnlyList<Dose>> GetAllAsync(int? vaccineId, CancellationToken ct = default);

        Task<Dose?> GetByAsync(Expression<Func<Dose,bool>>expression,CancellationToken ct = default);
    }
}

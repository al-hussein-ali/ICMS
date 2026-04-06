using ICMS.Domain.Entites.Common;
using ICMS.Domain.Entites.Identity;
using ICMS.Domain.Entites.Clinical;
using ICMS.Domain.Entites.Maternal;
using ICMS.Domain.Entites.Visits;
using ICMS.Domain.Entites.Audit;
using ICMS.Domain.Entites.Geography;
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

        Task<Dose?> GetNextDoseInSequenceAsync(int vaccineId, byte currentDoseOrder, CancellationToken ct = default);
    }
}

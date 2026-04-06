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
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ICMS.Application.Interfaces.Repositories
{
    public interface IPregnantWomanRepository : IRepository<PregnantWoman, int>
    {
        Task<PregnantWoman?> GetByPersonIdWithDetailsAsync(int personId, CancellationToken ct = default);
        Task<PregnantWoman?> GetByIdWithDetailsAsync(int id, CancellationToken ct = default);
    }
}

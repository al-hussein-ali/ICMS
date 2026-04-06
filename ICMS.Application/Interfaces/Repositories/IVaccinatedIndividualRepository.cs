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
using System.Threading.Tasks;

namespace ICMS.Application.Interfaces.Repositories
{
    public interface IVaccinatedIndividualRepository : IRepository<VaccinatedIndividual,int>
    {
        new Task<VaccinatedIndividual?> GetByIdAsync(int id, CancellationToken ct = default);

        Task<VaccinatedIndividual?> GetDetailsById(int id, CancellationToken ct = default);

        Task<VaccinatedIndividual?> GetDetailsByCardNumber(string cardNumber, CancellationToken ct = default);

        Task<VaccinatedIndividual?> GetByPersonIdAsync(int personId, CancellationToken ct = default);

        Task BulkInsertAsync(List<VaccinatedIndividual> vaccinatedIndividuals, CancellationToken ct = default);

        Task<List<VaccinatedIndividual>> GetByIdsWithImmunizationRecordsAsync(List<int> ids, CancellationToken ct = default);

        Task<VaccinatedIndividual?> GetIndividualWithSchedulesAsync(int id, CancellationToken ct = default);
    }
}

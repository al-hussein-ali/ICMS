using ICMS.Domain.Entites;
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

        Task BulkInsertAsync(List<VaccinatedIndividual> vaccinatedIndividuals, CancellationToken ct = default);


    }
}

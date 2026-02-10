using ICMS.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMS.Application.Interfaces.Repositories
{
    public interface IVaccineRepository : IRepository<Vaccine,int>
    {

        new Task<Vaccine?> GetByIdAsync(int id, CancellationToken ct = default);
    }
}

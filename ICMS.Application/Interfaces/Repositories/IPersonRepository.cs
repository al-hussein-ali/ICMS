using ICMS.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMS.Application.Interfaces.Repositories
{
    public interface IPersonRepository : IRepository<Person, int>
    {
        new Task<Person?> GetByIdAsync(int personId, CancellationToken ct = default);

        Task<Person?> GetByPhoneNumberAsync(string phoneNumber, CancellationToken ct = default);

    }
}

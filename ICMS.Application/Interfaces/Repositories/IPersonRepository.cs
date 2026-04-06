using ICMS.Application.DTOs.Person;
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
    public interface IPersonRepository : IRepository<Person, int>
    {
        new Task<Person?> GetByIdAsync(int personId, CancellationToken ct = default);

        Task<Person?> GetByPhoneNumberAsync(string phoneNumber, CancellationToken ct = default);

        Task<Person?> GetByAsync(string firstName, string lastName, string phoneNumber, DateOnly dateOfBirth,CancellationToken ct = default);
 

    }
}

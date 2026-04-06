using ICMS.Application.DTOs.Person;
using ICMS.Application.Interfaces.Repositories;
using ICMS.Domain.Entites.Common;
using ICMS.Domain.Entites.Identity;
using ICMS.Domain.Entites.Clinical;
using ICMS.Domain.Entites.Maternal;
using ICMS.Domain.Entites.Visits;
using ICMS.Domain.Entites.Audit;
using ICMS.Domain.Entites.Geography;
using ICMS.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL.Query.ExpressionTranslators.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace ICMS.Infrastructure.Repositories.Identity
{
    public class PersonRepository : Repository<Person, int>, IPersonRepository
    {

        public PersonRepository(AppDbContext context) : base(context)
        {
        }

        public new async Task<Person?> GetByIdAsync(int personId, CancellationToken ct = default)
        {
            return await _dbSet.FirstOrDefaultAsync(p => p.Id == personId && !p.IsDeleted);
        }
        public async Task<Person?> GetByPhoneNumberAsync(string phoneNumber, CancellationToken ct = default)
        {
            return await _dbSet.FirstOrDefaultAsync(p => p.PhoneNumber == phoneNumber && !p.IsDeleted);
        }

        public async Task<Person?> GetByAsync(string firstName, string lastName, string phoneNumber, DateOnly dateOfBirth, CancellationToken ct = default)
        {
            return await _dbSet.FirstOrDefaultAsync(p => p.FirstName == firstName
                                                      && p.LastName == lastName
                                                      && p.PhoneNumber == phoneNumber
                                                      && p.DateOfBirth == dateOfBirth && !p.IsDeleted);
        }
    }
}

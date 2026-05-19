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

        public async Task<List<Person>> GetByName(string fullName, CancellationToken ct = default)
        {
            var search = fullName.Trim().ToLower();
            var terms = search.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (terms.Length == 0)
            {
                return new List<Person>();
            }

            var query = _dbSet.Where(p => !p.IsDeleted);

            foreach (var term in terms)
            {
                query = query.Where(p =>
                    p.FirstName.ToLower().Contains(term) ||
                    p.SecondName.ToLower().Contains(term) ||
                    (p.ThirdName != null && p.ThirdName.ToLower().Contains(term)) ||
                    p.LastName.ToLower().Contains(term)
                );
            }

            return await query.ToListAsync(ct);
        }

        public async Task<List<Person>> GetByPhone(string phoneNumber, CancellationToken ct = default)
        {
            var search = phoneNumber.Trim();
            
            // If search starts with '0', also try without '0'
            // If search doesn't start with '0', also try with '0' prefix
            var searchWithoutZero = search.StartsWith('0') ? search.Substring(1) : search;
            var searchWithZero = search.StartsWith('0') ? search : "0" + search;

            return await _dbSet
                .Where(p => !p.IsDeleted && p.PhoneNumber != null && 
                    (p.PhoneNumber.Contains(search) || 
                     p.PhoneNumber.Contains(searchWithoutZero) ||
                     p.PhoneNumber.Contains(searchWithZero)))
                .ToListAsync(ct);
        }
    }
}

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
using System.Text;
using System.Threading.Tasks;
using ICMS.Application.DTOs.Pagination;
using ICMS.Domain.ValueObjects;
using System.Linq.Expressions;

namespace ICMS.Application.Interfaces.Services
{

    public interface IPersonService
    {
        Task<PagedResult<PersonReadDto>> GetAllAsync(PaginationParams paginationParams, CancellationToken ct = default);

        Task<PersonReadDto> GetByIdAsync(int id, CancellationToken ct = default);

        Task<PersonReadDto> GetByPhoneNumberAsync(string phoneNumber, CancellationToken ct = default);

        //Task<PersonReadDto?> GetByAsync(Expression<Func<PersonReadDto, bool>> predicate, CancellationToken ct = default);
        Task<PersonReadDto> AddAsync(PersonCreateDto entity, CancellationToken ct = default);

        Task<bool> UpdateAsync(int id, PersonCreateDto updatedEntity, CancellationToken ct = default);

        Task<bool> DeleteAsync(int id, CancellationToken ct = default);

    }
}

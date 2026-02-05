using ICMS.Application.DTOs.Person;
using ICMS.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICMS.Application.DTOs.Pagination;
using ICMS.Domain.ValueObjects;

namespace ICMS.Application.Interfaces.Services
{

    public interface IPersonService
    {
        PagedResult<PersonReadDto> GetAll(PaginationParams paginationParams, CancellationToken ct = default);

        Task<PersonReadDto?> GetByIdAsync(int id, CancellationToken ct = default);

        Task<PersonReadDto?> GetByPhoneNumberAsync(string phoneNumber, CancellationToken ct = default);
        Task<PersonReadDto> AddAsync(PersonCreateDto entity, CancellationToken ct = default);

        Task<bool> UpdateAsync(int id, PersonCreateDto updatedEntity, CancellationToken ct = default);

        Task<bool> DeleteAsync(int id, CancellationToken ct = default);

    }
}

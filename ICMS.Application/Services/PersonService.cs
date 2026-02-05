using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICMS.Application.DTOs.Person;
using ICMS.Application.DTOs.Pagination;
using ICMS.Application.Extensions;
using ICMS.Application.Interfaces;
using ICMS.Application.Interfaces.Services;
using ICMS.Domain.ValueObjects;
using System.Runtime.InteropServices;
using System.Net.Http.Headers;
using ICMS.Domain.Exceptions;
using System.Diagnostics;

namespace ICMS.Application.Services
{
    public class PersonService : IPersonService
    {
        private readonly IUnitOfWork _unitOfWork;
        public PersonService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public PagedResult<PersonReadDto> GetAll(PaginationParams paginationParams, CancellationToken ct = default)
        {
            var people = _unitOfWork.PersonRepository.GetQueryable().Select(p => p.ToReadDto());

            ct.ThrowIfCancellationRequested();

            return people.ApplyPagination(pageNumber: paginationParams.PageNumber, pageSize: paginationParams.PageSize);
        }
        public async Task<PersonReadDto?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var person = await _unitOfWork.PersonRepository.GetByIdAsync(id, ct);
            ct.ThrowIfCancellationRequested();

            return person?.ToReadDto();
        }
        public async Task<PersonReadDto?> GetByPhoneNumberAsync(string phoneNumber, CancellationToken ct = default)
        {
            var person = await _unitOfWork.PersonRepository.GetByPhoneNumberAsync(phoneNumber, ct);
            ct.ThrowIfCancellationRequested();

            return person?.ToReadDto();
        }
        public async Task<PersonReadDto> AddAsync(PersonCreateDto entity, CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();

            var person = entity.ToDomain();

            await _unitOfWork.PersonRepository.AddAsync(person, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            return person.ToReadDto();
        }
        public async Task<bool> UpdateAsync(int id, PersonCreateDto updatedEntity, CancellationToken ct = default)
        {
            var oldRecord = await _unitOfWork.PersonRepository.GetByIdAsync(id, ct);

            if (oldRecord is null)
                throw new RecordNotFoundException("Record was not found!");

            oldRecord.UpdatePersonInfo(
                updatedEntity.FirstName,
                updatedEntity.SecondName,
                updatedEntity.ThirdName,
                updatedEntity.LastName,
                updatedEntity.Gender,
                updatedEntity.DateOfBirth
                );

            oldRecord.ChangeContactInfo(updatedEntity.PhoneNumber);

            return await _unitOfWork.SaveChangesAsync(ct) > 0;
        }
        public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        {
            var exsitingPerson = await _unitOfWork.PersonRepository.GetByIdAsync(id, ct);

            if (exsitingPerson is null)
                throw new RecordNotFoundException("Record was not found!");

            exsitingPerson.MarkAsDeleted();

            return await _unitOfWork.SaveChangesAsync() > 0;
        }
    }
}

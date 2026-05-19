using FluentValidation;
using ICMS.Application.DTOs.Pagination;
using ICMS.Application.DTOs.Person;
using ICMS.Application.Extensions;
using ICMS.Application.Interfaces;
using ICMS.Application.Interfaces.Services;
using ICMS.Domain.Exceptions;
using ICMS.Domain.ValueObjects;

namespace ICMS.Application.Services
{
    public class PersonService(IUnitOfWork unitOfWork,
        IValidator<PaginationParams> paginationValidator,
        IValidator<PersonCreateDto> createDtoValidator) : IPersonService
    {
        public async Task<PagedResult<PersonReadDto>> GetAllAsync(PaginationParams paginationParams,
            CancellationToken ct = default)
        {
            await paginationValidator.ValidateAndThrowAsync(paginationParams, cancellationToken: ct);

            var query = unitOfWork.PersonRepository.GetQueryable().Where(p => !p.IsDeleted);

            if (!string.IsNullOrWhiteSpace(paginationParams.Search))
            {
                var search = paginationParams.Search.Trim().ToLower();
                var terms = search.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                foreach (var term in terms)
                {
                    query = query.Where(p => 
                        p.FirstName.ToLower().Contains(term) || 
                        p.SecondName.ToLower().Contains(term) ||
                        (p.ThirdName != null && p.ThirdName.ToLower().Contains(term)) ||
                        p.LastName.ToLower().Contains(term) || 
                        p.PhoneNumber.Contains(term)
                    );
                }
            }

            var people = query.Select(p => p.ToReadDto());

            ct.ThrowIfCancellationRequested();

            return people.ApplyPagination(pageNumber: paginationParams.PageNumber, pageSize: paginationParams.PageSize);
        }

        public async Task<PersonReadDto> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var person = await unitOfWork.PersonRepository.GetByIdAsync(id, ct);
            ct.ThrowIfCancellationRequested();

            return person?.ToReadDto() ?? throw new NotFoundException("NotFound");
        }

        public async Task<PersonReadDto> GetByPhoneNumberAsync(string phoneNumber, CancellationToken ct = default)
        {
            var person = await unitOfWork.PersonRepository.GetByPhoneNumberAsync(phoneNumber, ct);
            ct.ThrowIfCancellationRequested();

            return person?.ToReadDto() ?? throw new NotFoundException("NotFound");
        }

        public async Task<PersonReadDto> AddAsync(PersonCreateDto entity, CancellationToken ct = default)
        {
            await createDtoValidator.ValidateAndThrowAsync(entity, cancellationToken: ct);

            ct.ThrowIfCancellationRequested();

            var person = entity.ToDomain();

            await unitOfWork.PersonRepository.AddAsync(person, ct);
            await unitOfWork.SaveChangesAsync(ct);

            return person.ToReadDto();
        }

        public async Task<bool> UpdateAsync(int id, PersonCreateDto updatedEntity, CancellationToken ct = default)
        {
            var oldRecord = await unitOfWork.PersonRepository.GetByIdAsync(id, ct);

            if (oldRecord is null)
                throw new NotFoundException("NotFound");

            oldRecord.UpdatePersonInfo(
                updatedEntity.FirstName,
                updatedEntity.SecondName,
                updatedEntity.ThirdName,
                updatedEntity.LastName,
                updatedEntity.Gender.FromStringToGenderEnum(),
                updatedEntity.DateOfBirth,
                updatedEntity.PhoneNumber
            );


            return await unitOfWork.SaveChangesAsync(ct) > 0;
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        {
            var exsitingPerson = await unitOfWork.PersonRepository.GetByIdAsync(id, ct);

            if (exsitingPerson is null)
                throw new NotFoundException("NotFound");

            exsitingPerson.MarkAsDeleted();

            return await unitOfWork.SaveChangesAsync(ct) > 0;
        }

        public async Task<List<PersonReadDto>> GetByName(string fullName, CancellationToken ct = default)
        {
            var results = await unitOfWork.PersonRepository.GetByName(fullName, ct);
            return results.Select(p => p.ToReadDto()).ToList();
        }

        public async Task<List<PersonReadDto>> GetByPhone(string phoneNumber, CancellationToken ct = default)
        {
            var results = await unitOfWork.PersonRepository.GetByPhone(phoneNumber, ct);
            return results.Select(p => p.ToReadDto()).ToList();
        }
    }
}

using FluentValidation;
using ICMS.Application.DTOs.Pagination;
using ICMS.Application.DTOs.Person;
using ICMS.Application.Extensions;
using ICMS.Application.Interfaces;
using ICMS.Application.Interfaces.Services;
using ICMS.Domain.Exceptions;
using ICMS.Domain.ValueObjects;
using System.Linq.Expressions;

namespace ICMS.Application.Services
{
    public class PersonService : IPersonService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<PaginationParams> pagnationValidator;
        private readonly IValidator<PersonCreateDto> createDTOValidator;
        public PersonService(IUnitOfWork unitOfWork,
            IValidator<PaginationParams> pagnationValidator,
            IValidator<PersonCreateDto> createDTOValidator)
        {
            _unitOfWork = unitOfWork;
            this.pagnationValidator = pagnationValidator;
            this.createDTOValidator = createDTOValidator;
        }
        public async Task<PagedResult<PersonReadDto>> GetAllAsync(PaginationParams paginationParams, CancellationToken ct = default)
        {
            await pagnationValidator.ValidateAndThrowAsync(paginationParams);

            var people = _unitOfWork.PersonRepository.GetQueryable().Where(p => !p.IsDeleted).Select(p => p.ToReadDto());

            ct.ThrowIfCancellationRequested();

            return people.ApplyPagination(pageNumber: paginationParams.PageNumber, pageSize: paginationParams.PageSize);

        }
        public async Task<PersonReadDto> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var person = await _unitOfWork.PersonRepository.GetByIdAsync(id, ct);
            ct.ThrowIfCancellationRequested();

            return person?.ToReadDto() ?? throw new NotFoundException("This person was not found");
        }
        public async Task<PersonReadDto> GetByPhoneNumberAsync(string phoneNumber, CancellationToken ct = default)
        {
            var person = await _unitOfWork.PersonRepository.GetByPhoneNumberAsync(phoneNumber, ct);
            ct.ThrowIfCancellationRequested();

            return person?.ToReadDto() ?? throw new NotFoundException("This person was not found");
        }

        public async Task<PersonReadDto> AddAsync(PersonCreateDto entity, CancellationToken ct = default)
        {
            await createDTOValidator.ValidateAndThrowAsync(entity);

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
                throw new NotFoundException("Record was not found!");

            oldRecord.UpdatePersonInfo(
                updatedEntity.FirstName,
                updatedEntity.SecondName,
                updatedEntity.ThirdName,
                updatedEntity.LastName,
                updatedEntity.Gender.FromStringToGenderEnum(),
                updatedEntity.DateOfBirth,
                updatedEntity.PhoneNumber
                );

        
            return await _unitOfWork.SaveChangesAsync(ct) > 0;
        }
        public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        {
            var exsitingPerson = await _unitOfWork.PersonRepository.GetByIdAsync(id, ct);

            if (exsitingPerson is null)
                throw new NotFoundException("Record was not found!");

            exsitingPerson.MarkAsDeleted();

            return await _unitOfWork.SaveChangesAsync(ct) > 0;
        }
    }
}

using ICMS.Application.DTOs.Person;
using ICMS.Domain.Entites;

namespace ICMS.Application.Extensions
{
    public static class PersonExtensions
    {
        public static PersonReadDto ToReadDto(this Person person)
            => new(person.Id, person.FirstName, person.SecondName, person.ThirdName, person.LastName, person.Gender, person.DateOfBirth, person.PhoneNumber);

        public static PersonDetailsDto ToDetailsDto(this Person person)
            => new(person.Id, person.FirstName, person.SecondName, person.ThirdName, person.LastName, person.Gender, person.DateOfBirth, person.PhoneNumber, person.CreatedAt, person.User?.Id, person.PregnantWoman?.Id, person.VaccinatedIndividual?.Id);

        public static Person ToDomain(this PersonCreateDto dto)
            => Person.Create(dto.FirstName, dto.SecondName, dto.ThirdName, dto.LastName, dto.Gender, dto.DateOfBirth, dto.PhoneNumber);
    }
}

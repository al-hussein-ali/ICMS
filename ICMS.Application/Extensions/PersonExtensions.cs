using ICMS.Application.DTOs.Person;
using ICMS.Domain.Entites;
using ICMS.Domain.Enums;
using System.Net.NetworkInformation;
using System.Reflection.Metadata.Ecma335;

namespace ICMS.Application.Extensions
{
    public static class PersonExtensions
    {
        public static PersonReadDto ToReadDto(this Person person)
            => new(person.Id, person.FirstName, person.SecondName, person.ThirdName, person.LastName, person.Gender.ToString(), person.DateOfBirth, person.PhoneNumber);

        public static PersonDetailsDto ToDetailsDto(this Person person)
            => new(person.Id, person.FirstName, person.SecondName, person.ThirdName, person.LastName, person.Gender, person.DateOfBirth, person.PhoneNumber, person.CreatedAt, person.User?.Id, person.PregnantWoman?.Id, person.VaccinatedIndividual?.Id);

        public static Person ToDomain(this PersonCreateDto dto)
            => Person.Create(dto.FirstName, dto.SecondName, dto.ThirdName, dto.LastName, dto.Gender.FromStringToGenderEnum(), dto.DateOfBirth, dto.PhoneNumber);



    }

}

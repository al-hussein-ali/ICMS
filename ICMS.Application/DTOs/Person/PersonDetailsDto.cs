using System;
using ICMS.Domain.Enums;

namespace ICMS.Application.DTOs.Person
{
    public record PersonDetailsDto(int Id, string FirstName, string SecondName, string? ThirdName, string LastName, string Gender, DateOnly DateOfBirth, string PhoneNumber, DateTime CreatedAt, int? UserId, int? PregnantWomanId, int? VaccinatedIndividualId);
}

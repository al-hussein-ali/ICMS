using System;
using ICMS.Domain.Enums;

namespace ICMS.Application.DTOs.Person
{
    public record PersonCreateDto(string FirstName, string SecondName, string? ThirdName, string LastName, Gender Gender, DateOnly DateOfBirth, string PhoneNumber);
}

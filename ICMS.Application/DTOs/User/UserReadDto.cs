using System;

namespace ICMS.Application.DTOs.User
{
    public record UserReadDto(
        int Id,
        string UserName,
        bool IsActive,
        int PersonId,
        string? FirstName,
        string? SecondName,
        string? ThirdName,
        string? LastName,
        string? Phone,
        DateOnly? DateOfBirth,
        string? Gender,
        IEnumerable<string>? Roles = null
    );
}

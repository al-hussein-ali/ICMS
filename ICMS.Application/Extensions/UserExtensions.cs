using ICMS.Application.DTOs.User;
using ICMS.Domain.Entites.Identity;
using System.Collections.Generic;

namespace ICMS.Application.Extensions
{
    public static class UserExtensions
    {
        public static UserReadDto ToReadDto(this User u, IEnumerable<string> roles)
            => new(
                u.Id, 
                u.UserName, 
                u.IsActive, 
                u.PersonId, 
                u.Person?.FirstName, 
                u.Person?.SecondName,
                u.Person?.ThirdName,
                u.Person?.LastName, 
                u.Person?.PhoneNumber, 
                u.Person?.DateOfBirth.ToString("yyyy-MM-dd"),
                u.Person?.Gender.ToString().ToLower(),
                roles
            );

        public static UserDetailsDto ToDetailsDto(this User u)
            => new(u.Id, u.UserName, u.IsActive, u.PersonId, u.CreatedAt);

        public static User ToDomain(this UserCreateDto dto, string hashedPassword, int personId)
            => User.Create(dto.UserName, hashedPassword, personId, dto.IsActive);
    }
}

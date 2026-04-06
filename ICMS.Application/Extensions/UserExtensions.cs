using ICMS.Application.DTOs.User;
using ICMS.Domain.Entites.Identity;
using System.Collections.Generic;

namespace ICMS.Application.Extensions
{
    public static class UserExtensions
    {
        public static UserReadDto ToReadDto(this User u, IEnumerable<string> roles)
            => new(u.Id, u.UserName, u.IsActive, u.PersonId, roles);

        public static UserDetailsDto ToDetailsDto(this User u)
            => new(u.Id, u.UserName, u.IsActive, u.PersonId, u.CreatedAt);

        public static User ToDomain(this UserCreateDto dto)
            => User.Create(dto.UserName, dto.PasswordHash, dto.PersonId, dto.IsActive);
    }
}

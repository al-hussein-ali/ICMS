using ICMS.Application.DTOs.User;
using ICMS.Domain.Entites;

namespace ICMS.Application.Extensions
{
    public static class UserExtensions
    {
        public static UserReadDto ToReadDto(this User u)
            => new(u.Id, u.UserName, u.IsActive, u.PersonId);

        public static UserDetailsDto ToDetailsDto(this User u)
            => new(u.Id, u.UserName, u.IsActive, u.PersonId, u.CreatedAt);

        public static User ToDomain(this UserCreateDto dto)
            => User.Create(dto.UserName, dto.PasswordHash, dto.PersonId, dto.IsActive);
    }
}

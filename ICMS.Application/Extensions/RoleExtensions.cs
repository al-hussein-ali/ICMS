using ICMS.Application.DTOs.Role;
using ICMS.Domain.Entites;

namespace ICMS.Application.Extensions
{
    public static class RoleExtensions
    {
        public static RoleReadDto ToReadDto(this Role r)
            => new(r.Id, r.RoleName);

        public static RoleDetailsDto ToDetailsDto(this Role r)
            => new(r.Id, r.RoleName);

        public static Role ToDomain(this RoleCreateDto dto)
            => Role.Create(dto.RoleName);
    }
}

using ICMS.Application.DTOs.Role;
using ICMS.Domain.Entites.Common;
using ICMS.Domain.Entites.Identity;
using ICMS.Domain.Entites.Clinical;
using ICMS.Domain.Entites.Maternal;
using ICMS.Domain.Entites.Visits;
using ICMS.Domain.Entites.Audit;
using ICMS.Domain.Entites.Geography;

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

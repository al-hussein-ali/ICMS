using FluentValidation;
using ICMS.Application.DTOs.Pagination;
using ICMS.Application.DTOs.Role;
using ICMS.Application.Extensions;
using ICMS.Application.Interfaces;
using ICMS.Application.Interfaces.Services;
using ICMS.Domain.Exceptions;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ICMS.Application.Services
{
    public class RoleService(IUnitOfWork unitOfWork, IValidator<RoleCreateDto> roleCreateValidator) : IRoleService
    {
        public async Task<IReadOnlyList<RoleReadDto>> GetAllAsync(PaginationParams paginationParams, CancellationToken ct = default)
        {
            var roles = await unitOfWork.RoleRepository.GetPagedAsync(paginationParams.PageNumber, paginationParams.PageSize, false, ct);
            return roles.Select(r => r.ToReadDto()).ToList();
        }

        public async Task<RoleReadDto?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var role = await unitOfWork.RoleRepository.GetByIdAsync(id, ct);
            return role?.ToReadDto();
        }

        public async Task<RoleReadDto> AddAsync(RoleCreateDto entity, CancellationToken ct = default)
        {
            await roleCreateValidator.ValidateAndThrowAsync(entity, ct);

            var role = entity.ToDomain();
            await unitOfWork.RoleRepository.AddAsync(role, ct);
            await unitOfWork.SaveChangesAsync(ct);

            return role.ToReadDto();
        }

        public async Task<bool> UpdateAsync(int id, RoleCreateDto updatedEntity, CancellationToken ct = default)
        {
            await roleCreateValidator.ValidateAndThrowAsync(updatedEntity, ct);

            var role = await unitOfWork.RoleRepository.GetByIdAsync(id, ct);
            if (role == null) throw new NotFoundException($"Role with ID {id} not found.");

            role.UpdateRoleName(updatedEntity.RoleName);

            await unitOfWork.RoleRepository.UpdateAsync(role, ct);
            await unitOfWork.SaveChangesAsync(ct);

            return true;
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        {
            var role = await unitOfWork.RoleRepository.GetByIdAsync(id, ct);
            if (role == null) throw new NotFoundException($"Role with ID {id} not found.");

            await unitOfWork.RoleRepository.DeleteAsync(role, ct);
            await unitOfWork.SaveChangesAsync(ct);

            return true;
        }
    }
}
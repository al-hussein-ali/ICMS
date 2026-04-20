using FluentValidation;
using ICMS.Application.DTOs.Pagination;
using ICMS.Application.DTOs.User;
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
    public class UserService(
        IUnitOfWork unitOfWork, 
        IIdentityService identityService,
        IValidator<UserCreateDto> userCreateValidator,
        IValidator<UserReadDto> userUpdateValidator,
        IRefreshTokenService refreshTokenService) : IUserService
    {
        public async Task<IReadOnlyList<UserReadDto>> GetAllAsync(PaginationParams paginationParams, CancellationToken ct = default)
        {
            var users = await unitOfWork.UserRepository.GetPagedAsync(paginationParams.PageNumber, paginationParams.PageSize, false, ct);

            var userDtos = new List<UserReadDto>();
            foreach (var user in users)
            {
                var roles = await identityService.GetUserRolesAsync(user.Id, ct);
                userDtos.Add(user.ToReadDto(roles));
            }

            return userDtos;
        }

        public async Task<UserReadDto?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var user = await unitOfWork.UserRepository.GetByIdAsync(id, ct);
            if (user == null) return null;

            var roles = await identityService.GetUserRolesAsync(user.Id, ct);
            return user.ToReadDto(roles);
        }

        public async Task<UserReadDto> AddAsync(UserCreateDto entity, CancellationToken ct = default)
        {
            await userCreateValidator.ValidateAndThrowAsync(entity, ct);

            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(entity.Password);
            var user = entity.ToDomain(hashedPassword);
            
            await unitOfWork.UserRepository.AddAsync(user, ct);
            await unitOfWork.SaveChangesAsync(ct);

            if (entity.Roles != null && entity.Roles.Any())
            {
                await identityService.AssignRolesToUserAsync(user.Id, entity.Roles, ct);
            }

            var roles = await identityService.GetUserRolesAsync(user.Id, ct);
            return user.ToReadDto(roles);
        }

        public async Task<bool> UpdateAsync(UserReadDto updatedEntity, CancellationToken ct = default)
        {
            await userUpdateValidator.ValidateAndThrowAsync(updatedEntity, ct);

            var user = await unitOfWork.UserRepository.GetByIdAsync(updatedEntity.Id, ct);
            if (user == null) throw new NotFoundException("NotFound");

            // Sync Roles if provided
            if (updatedEntity.Roles != null)
            {
                var targetRoles = updatedEntity.Roles.ToList(); 
                var existingRoles = (await identityService.GetUserRolesAsync(user.Id, ct)).ToList();

                var rolesToAdd = targetRoles.Except(existingRoles).ToList();
                var rolesToRemove = existingRoles.Except(targetRoles).ToList();

                if (rolesToAdd.Any()) await identityService.AssignRolesToUserAsync(user.Id, rolesToAdd, ct);
                if (rolesToRemove.Any()) await identityService.RemoveRolesFromUserAsync(user.Id, rolesToRemove, ct);
            }

            // Update IsActive status if it changed
            if (updatedEntity.IsActive != user.IsActive)
            {
                if (!updatedEntity.IsActive)
                {
                    user.DeactivateUser();
                }
            }

            await unitOfWork.UserRepository.UpdateAsync(user, ct);
            await unitOfWork.SaveChangesAsync(ct);

            return true;
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        {
            var user = await unitOfWork.UserRepository.GetByIdAsync(id, ct);
            if (user == null) return false;

            await unitOfWork.UserRepository.DeleteAsync(user, ct);
            await unitOfWork.SaveChangesAsync(ct);

            return true;
        }

        public async Task<bool> ActivateAsync(int id, CancellationToken ct = default)
        {
            var user = await unitOfWork.UserRepository.GetByIdAsync(id, ct);
            if (user == null) return false;

            user.ActivateUser();
            await unitOfWork.UserRepository.UpdateAsync(user, ct);
            await unitOfWork.SaveChangesAsync(ct);
            return true;
        }

        public async Task<bool> DeactivateAsync(int id, CancellationToken ct = default)
        {
            var user = await unitOfWork.UserRepository.GetByIdAsync(id, ct);
            if (user == null) return false;

            user.DeactivateUser();
            await unitOfWork.UserRepository.UpdateAsync(user, ct);
            await refreshTokenService.InvalidateUserRefreshTokensAsync(id, ct);
            await unitOfWork.SaveChangesAsync(ct);
            return true;
        }

        public async Task<bool> ChangePasswordAsync(int id, string newPassword, CancellationToken ct = default)
        {
            var user = await unitOfWork.UserRepository.GetByIdAsync(id, ct);
            if (user == null) return false;

            user.ChangePassword(BCrypt.Net.BCrypt.HashPassword(newPassword));
            await unitOfWork.UserRepository.UpdateAsync(user, ct);
            await refreshTokenService.InvalidateUserRefreshTokensAsync(id, ct);
            await unitOfWork.SaveChangesAsync(ct);
            return true;
        }
    }
}

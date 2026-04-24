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
            var users = await unitOfWork.UserRepository.GetPagedAsync(paginationParams.PageNumber, paginationParams.PageSize, false, ct, u => u.Person);

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

            int personId = entity.PersonId ?? 0;

            if (personId == 0 && entity.PersonCreateDto != null)
            {
                var person = entity.PersonCreateDto.ToDomain();
                await unitOfWork.PersonRepository.AddAsync(person, ct);
                await unitOfWork.SaveChangesAsync(ct);
                personId = person.Id;
            }

            if (personId == 0) throw new DomainException("Person details or PersonId must be provided");

            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(entity.Password);
            var user = entity.ToDomain(hashedPassword, personId);
            
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

            try
            {
                await unitOfWork.UserRepository.DeleteAsync(user, ct);
                await unitOfWork.SaveChangesAsync(ct);
                return true;
            }
            catch (Exception ex) when (ex.InnerException?.Message.Contains("FOREIGN KEY", StringComparison.OrdinalIgnoreCase) == true || 
                                       ex.InnerException?.Message.Contains("REFERENCE", StringComparison.OrdinalIgnoreCase) == true ||
                                       ex.Message.Contains("FOREIGN KEY", StringComparison.OrdinalIgnoreCase) ||
                                       ex.Message.Contains("REFERENCE", StringComparison.OrdinalIgnoreCase))
            {
                throw new DomainException("Cannot delete this user because they are linked to existing system records. Please deactivate them instead.");
            }
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

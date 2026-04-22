using ICMS.Application.Interfaces;
using ICMS.Application.Interfaces.Services;
using ICMS.Domain.Entites.Identity;
using ICMS.Domain.Exceptions;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ICMS.Application.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly IUnitOfWork _unitOfWork;

        public IdentityService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task AssignRolesToUserAsync(int userId, IEnumerable<string> roleNames, CancellationToken ct = default)
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(userId, ct);
            if (user == null) throw new NotFoundException("NotFound");

            foreach (var roleName in roleNames)
            {
                var role = await _unitOfWork.RoleRepository.FirstOrDefaultAsync(r => r.RoleName == roleName, ct);

                if (role == null) throw new DomainException("RoleNotFound");

                if (!user.UserRoles.Any(ur => ur.RoleId == role.Id))
                {
                    var userRole = UserRole.Create(userId, role.Id);
                    user.AddUserRole(userRole);
                    await _unitOfWork.UserRoleRepository.AddAsync(userRole, ct);
                }
            }

            await _unitOfWork.SaveChangesAsync(ct);
        }

        public async Task RemoveRolesFromUserAsync(int userId, IEnumerable<string> roleNames, CancellationToken ct = default)
        {
             var user = await _unitOfWork.UserRepository.GetByIdAsync(userId, ct);
            if (user == null) throw new NotFoundException("NotFound");

            foreach (var roleName in roleNames)
            {
                var role = await _unitOfWork.RoleRepository.FirstOrDefaultAsync(r => r.RoleName == roleName, ct);

                if (role != null)
                {
                    user.RemoveUserRole(role.Id);
                    await _unitOfWork.UserRoleRepository.DeleteAsync(userId, role.Id, ct);
                }
            }

            await _unitOfWork.SaveChangesAsync(ct);
        }

        public async Task<IEnumerable<string>> GetUserRolesAsync(int userId, CancellationToken ct = default)
        {
            var userRoles = await _unitOfWork.UserRoleRepository.GetByUserIdAsync(userId, ct);
            return userRoles.Select(ur => ur.Role.RoleName);
        }

        public async Task<bool> IsUserInRoleAsync(int userId, string roleName, CancellationToken ct = default)
        {
            var role = await _unitOfWork.RoleRepository.FirstOrDefaultAsync(r => r.RoleName == roleName, ct);

            if (role == null) return false;

            return await _unitOfWork.UserRoleRepository.ExistsAsync(userId, role.Id, ct);
        }
    }
}

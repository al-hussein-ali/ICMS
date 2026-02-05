using ICMS.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMS.Domain.Entites
{
    public class Role : BaseEntity<int>
    {
        private readonly List<UserRole> _userRoles = new();
        public IReadOnlyList<UserRole> UserRoles => _userRoles.AsReadOnly();
        public string RoleName { get; private set; } = string.Empty;

        private Role()
        {
        }

        public static Role Create(string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName)) throw new DomainException("Role name is required");
            return new Role { RoleName = roleName };
        }

        public void AddUserRole(UserRole ur)
        {
            if (ur == null) throw new DomainException("UserRole is required");
            if (_userRoles.Any(x => x.RoleId == ur.RoleId && x.UserId == ur.UserId)) throw new DomainException("UserRole already added");

            _userRoles.Add(ur);
        }
    }
}

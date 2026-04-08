using ICMS.Domain.Entites.Geography;
using ICMS.Domain.Entites.Audit;
using ICMS.Domain.Entites.Visits;
using ICMS.Domain.Entites.Maternal;
using ICMS.Domain.Entites.Clinical;
using ICMS.Domain.Entites.Identity;
using ICMS.Domain.Entites.Common;
using ICMS.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMS.Domain.Entites.Identity
{
    public class User : BaseEntity<int>
    {
        private readonly List<UserRole> _userRoles = new();

        private readonly List<FieldVisitUser> _fieldVisitUsers = new();

        public IReadOnlyList<UserRole> UserRoles => _userRoles.AsReadOnly();
        public IReadOnlyList<FieldVisitUser> FieldVisitUsers => _fieldVisitUsers.AsReadOnly();
        public string UserName { get; private set; } = string.Empty;
        public string PasswordHash { get; private set; } = string.Empty;
        public bool IsActive { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public int PersonId { get; private set; }
        public Person? Person { get; private set; }

        public ICollection<RefreshToken> RefreshTokens { get; private set; } = new List<RefreshToken>();

        private User()
        {
        }

        public static User Create(string username, string passwordHash, int personId, bool isActive = true)
        {
            if (string.IsNullOrWhiteSpace(username)) throw new DomainException("Username is required");
            if (string.IsNullOrWhiteSpace(passwordHash)) throw new DomainException("Password hash is required");
            if (personId <= 0) throw new DomainException("Invalid person id");

            return new User
            {
                UserName = username,
                PasswordHash = passwordHash,
                PersonId = personId,
                IsActive = isActive,
                CreatedAt = DateTime.UtcNow
            };
        }

        public void AssignPerson(Person person)
        {
            if (person == null) throw new DomainException("Person is required");
            if (Person != null) throw new DomainException("Person already assigned");
            if (person.Id != 0 && person.Id != PersonId) throw new DomainException("Person id mismatch");

            Person = person;
            PersonId = person.Id;
        }

        public void DeactivateUser()
        {
            if (!IsActive)
                return;

            IsActive = false;
        }

        public void ActivateUser()
        {
            if (IsActive)
                return;

            IsActive = true;
        }

        public void ChangePassword(string newPasswordHash)
        {
            if (string.IsNullOrEmpty(newPasswordHash))
                throw new DomainException("The New Password cannot be empty!");


            PasswordHash = newPasswordHash;

        }

        public void AddUserRole(UserRole ur)
        {
            if (ur == null) throw new DomainException("User role is required");
            if (_userRoles.Any(x => x.RoleId == ur.RoleId)) throw new DomainException("User role already added");

            _userRoles.Add(ur);
        }

        public void RemoveUserRole(int roleId)
        {
            var ur = _userRoles.FirstOrDefault(x => x.RoleId == roleId);
            if (ur != null)
            {
                _userRoles.Remove(ur);
            }
        }

        public void ClearUserRoles()
        {
            _userRoles.Clear();
        }

        public void AddFieldVisitUser(FieldVisitUser fvu)
        {
            if (fvu == null) throw new DomainException("Field Visit User is required");
            if (_fieldVisitUsers.Any(x => x.UserId == fvu.UserId && x.FieldVisitId == fvu.FieldVisitId)) throw new DomainException("Field Visit User already added");

            _fieldVisitUsers.Add(fvu);
        }
    }
}

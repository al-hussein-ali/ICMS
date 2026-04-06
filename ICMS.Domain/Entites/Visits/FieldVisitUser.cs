using ICMS.Domain.Entites.Common;
using ICMS.Domain.Entites.Identity;
using ICMS.Domain.Entites.Clinical;
using ICMS.Domain.Entites.Maternal;
using ICMS.Domain.Entites.Visits;
using ICMS.Domain.Entites.Audit;
using ICMS.Domain.Entites.Geography;
using ICMS.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMS.Domain.Entites.Visits
{
    public class FieldVisitUser
    {
        public int FieldVisitId { get; private set; }
        public int UserId { get; private set; }
        public User? FieldWorker { get; private set; }
        public FieldVisit? FieldVisit { get; private set; }

        private FieldVisitUser()
        {
        }

        public static FieldVisitUser Create(int fieldVisitId, int userId)
        {
            if (fieldVisitId <= 0) throw new DomainException("Invalid field visit id");
            if (userId <= 0) throw new DomainException("Invalid user id");

            return new FieldVisitUser { FieldVisitId = fieldVisitId, UserId = userId };
        }

        public void AssignFieldWorker(User user)
        {
            if (user == null) throw new DomainException("User is required");
            if (FieldWorker != null) throw new DomainException("Field worker already assigned");
            if (user.Id != 0 && user.Id != UserId) throw new DomainException("User id mismatch");

            FieldWorker = user;
            UserId = user.Id;
        }

        public void AssignFieldVisit(FieldVisit fieldVisit)
        {
            if (fieldVisit == null) throw new DomainException("FieldVisit is required");
            if (FieldVisit != null) throw new DomainException("FieldVisit already assigned");
            if (fieldVisit.Id != 0 && fieldVisit.Id != FieldVisitId) throw new DomainException("FieldVisit id mismatch");

            FieldVisit = fieldVisit;
            FieldVisitId = fieldVisit.Id;
        }
    }
}

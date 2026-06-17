using ICMS.Domain.Entites.Identity;

namespace ICMS.Domain.Entites.Visits
{
    public class FieldVisitWorker
    {
        public int FieldVisitId { get; set; }
        public FieldVisit FieldVisit { get; set; } = null!;

        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public bool IsGoing { get; set; } = true;

        private FieldVisitWorker() { }

        public static FieldVisitWorker Create(int fieldVisitId, int userId)
        {
            return new FieldVisitWorker
            {
                FieldVisitId = fieldVisitId,
                UserId = userId,
                IsGoing = true
            };
        }
    }
}

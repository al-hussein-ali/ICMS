using ICMS.Domain.Entites.Common;
using ICMS.Domain.Exceptions;

namespace ICMS.Domain.Entites.Visits
{
    public class FetalDetails : BaseEntity<int>
    {
        public int VisitDetailsId { get; private set; }
        public string FetusLabel { get; private set; } = string.Empty; // "Fetus A", "Fetus B", etc.
        public string FetalHeartbeat { get; private set; } = string.Empty;
        public string? FetalHeartbeatValue { get; private set; }
        public string FetalMovement { get; private set; } = string.Empty;
        public string FetalPosition { get; private set; } = string.Empty;

        public VisitDetails? VisitDetails { get; private set; }

        private FetalDetails()
        {
        }

        public static FetalDetails Create(int visitDetailsId, string fetusLabel, string fetalHeartbeat, string fetalMovement, string fetalPosition, string? fetalHeartbeatValue = null)
        {
            if (string.IsNullOrWhiteSpace(fetusLabel)) throw new DomainException("Fetus label is required");

            return new FetalDetails
            {
                VisitDetailsId = visitDetailsId,
                FetusLabel = fetusLabel,
                FetalHeartbeat = fetalHeartbeat,
                FetalMovement = fetalMovement,
                FetalPosition = fetalPosition,
                FetalHeartbeatValue = fetalHeartbeatValue
            };
        }

        public void Update(string fetusLabel, string fetalHeartbeat, string fetalMovement, string fetalPosition, string? fetalHeartbeatValue = null)
        {
            if (string.IsNullOrWhiteSpace(fetusLabel)) throw new DomainException("Fetus label is required");

            FetusLabel = fetusLabel;
            FetalHeartbeat = fetalHeartbeat;
            FetalMovement = fetalMovement;
            FetalPosition = fetalPosition;
            FetalHeartbeatValue = fetalHeartbeatValue;
        }
    }
}

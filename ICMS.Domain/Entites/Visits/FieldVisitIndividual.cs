using ICMS.Domain.Entites.Identity;

namespace ICMS.Domain.Entites.Visits
{
    public class FieldVisitIndividual
    {
        public int FieldVisitId { get; set; }
        public FieldVisit FieldVisit { get; set; } = null!;

        public int VaccinatedIndividualId { get; set; }
        public VaccinatedIndividual VaccinatedIndividual { get; set; } = null!;

        private FieldVisitIndividual() { }

        public static FieldVisitIndividual Create(int fieldVisitId, int vaccinatedIndividualId)
        {
            return new FieldVisitIndividual
            {
                FieldVisitId = fieldVisitId,
                VaccinatedIndividualId = vaccinatedIndividualId
            };
        }
    }
}

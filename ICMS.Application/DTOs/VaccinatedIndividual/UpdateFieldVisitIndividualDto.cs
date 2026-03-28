using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMS.Application.DTOs.VaccinatedIndividual
{
    public  record UpdateFieldVisitIndividualDto(int IndividualId,int DoseId,
        int FieldVisitId, DateOnly VaccinationDate, string TakenIn, string? Note);
}

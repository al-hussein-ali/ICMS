using ICMS.Application.DTOs.ImmunizationRecord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMS.Application.DTOs.VaccinatedIndividual
{
    public record NewFieldVaccinatedIndividualDto(string Directorate, string Area, string Neighborhood, string FirstName,
        string SecondName, string? ThirdName, string LastName,
        string Gender, DateOnly DateOfBirth, string PhoneNumber, int DoseId, 
        int FieldVisitId, DateOnly VaccinationDate, string TakenIn, string? Note);
}

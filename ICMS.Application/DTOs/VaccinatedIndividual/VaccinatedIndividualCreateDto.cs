using System;

namespace ICMS.Application.DTOs.VaccinatedIndividual
{
    public record VaccinatedIndividualCreateDto(string CardNumber, string Directorate, string Area, string Neighborhood, int? UserId, int PersonId);
}

using System;

namespace ICMS.Application.DTOs.VaccinatedIndividual
{
    public record VaccinatedIndividualReadDto(int Id, string CardNumber, string Directorate, string Area, string Neighborhood, int PersonId);
}

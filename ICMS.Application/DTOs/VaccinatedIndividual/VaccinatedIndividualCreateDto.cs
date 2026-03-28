using ICMS.Application.DTOs.Person;
using System;

namespace ICMS.Application.DTOs.VaccinatedIndividual
{
    public record VaccinatedIndividualCreateDto(string Directorate, string Area, string Neighborhood, int? UserId,PersonCreateDto? PersonCreateDto,int? PersonId);
}

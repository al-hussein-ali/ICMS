using ICMS.Application.DTOs.Person;
using ICMS.Application.DTOs.Schedules;
using System.Collections.Generic;

namespace ICMS.Application.DTOs.VaccinatedIndividual
{
    public record VaccinatedIndividualDetailsDto(int Id, string CardNumber, int GovernorateId, string GovernorateName, int DirectorateId, string DirectorateName, int NeighborhoodId, string NeighborhoodName, int? SubNeighborhoodId, int PersonId, int? UserId, PersonReadDto Person, List<ScheduleReadDto> Schedules);
}

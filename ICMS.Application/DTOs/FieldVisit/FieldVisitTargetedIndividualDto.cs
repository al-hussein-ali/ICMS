using System.Collections.Generic;

namespace ICMS.Application.DTOs.FieldVisit
{
    public record FieldVisitTargetedIndividualDto(
        int Id,
        string FullName,
        string CardNumber,
        string PhoneNumber,
        List<string> DelayedDoseNames);
}

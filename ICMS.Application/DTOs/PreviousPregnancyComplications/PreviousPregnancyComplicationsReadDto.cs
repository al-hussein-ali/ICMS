using System;

namespace ICMS.Application.DTOs.PreviousPregnancyComplications
{
    public record PreviousPregnancyComplicationsReadDto(int Id, bool VaginalBleedingDuringPregnancy, int PregnancyDetailId);
}

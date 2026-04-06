using System;

namespace ICMS.Application.DTOs.PreviousPostpartumComplications
{
    public record PreviousPostpartumComplicationsReadDto(int Id, bool VaginalBleeding, int PregnancyDetailId);
}

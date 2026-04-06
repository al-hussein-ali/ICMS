using System;

namespace ICMS.Application.DTOs.PreviousPostpartumComplications
{
    public record PreviousPostpartumComplicationsCreateDto(bool VaginalBleeding,int PregnancyDetailId, 
        bool PlacentaRetention,
        bool VaginalFistula ,bool PuerperalSepsis, bool NeonatalDeathWithinFirstWeek);
}

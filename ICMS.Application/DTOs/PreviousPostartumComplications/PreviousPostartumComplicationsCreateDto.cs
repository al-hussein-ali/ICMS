using System;

namespace ICMS.Application.DTOs.PreviousPostartumComplications
{
    public record PreviousPostartumComplicationsCreateDto(bool VaginalBleeding,int PregnancyDetailId, 
        bool PlacentaRetention,
        bool VaginalFistula ,bool PuerperalSepsis, bool NeonatalDeathWithinFirstWeek);


}

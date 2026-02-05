using System;

namespace ICMS.Application.DTOs.PreviousPregnancyDeliveryComplications
{
    public record PreviousPregnancyDeliveryComplicationsCreateDto(bool CesareanSection,
        bool AssistedDeliver,
        bool StillbirthOrMultipleDeaths,
        int PregnancyDetailId);


}

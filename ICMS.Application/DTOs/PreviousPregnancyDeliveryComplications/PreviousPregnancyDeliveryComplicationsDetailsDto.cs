using System;

namespace ICMS.Application.DTOs.PreviousPregnancyDeliveryComplications
{
    public record PreviousPregnancyDeliveryComplicationsDetailsDto(int Id, bool CesareanSection, int PregnancyDetailId);
}

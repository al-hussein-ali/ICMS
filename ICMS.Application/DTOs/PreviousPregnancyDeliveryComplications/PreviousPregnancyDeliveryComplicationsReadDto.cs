using System;

namespace ICMS.Application.DTOs.PreviousPregnancyDeliveryComplications
{
    public record PreviousPregnancyDeliveryComplicationsReadDto(int Id, bool CesareanSection, int PregnancyDetailId);
}

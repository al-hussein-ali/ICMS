using System;

namespace ICMS.Application.DTOs.PreviousPregnancyComplications
{
    public record PreviousPregnancyComplicationsCreateDto(bool VaginalBleedingDuringPregnancy,
        bool RecurrentMiscarriageMoreThanThree,
        bool Diabetes,
        bool Epilepsy,
        bool HeartDisease,
        bool Preeclampsia,
        bool PretermBirthBefore8Months,
        int PregnancyDetailId);

}

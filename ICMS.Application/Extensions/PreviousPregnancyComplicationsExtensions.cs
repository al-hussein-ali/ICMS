using ICMS.Application.DTOs.PreviousPregnancyComplications;
using ICMS.Domain.Entites.Common;
using ICMS.Domain.Entites.Identity;
using ICMS.Domain.Entites.Clinical;
using ICMS.Domain.Entites.Maternal;
using ICMS.Domain.Entites.Visits;
using ICMS.Domain.Entites.Audit;
using ICMS.Domain.Entites.Geography;

namespace ICMS.Application.Extensions
{
    public static class PreviousPregnancyComplicationsExtensions
    {
        public static PreviousPregnancyComplicationsReadDto ToReadDto(this PreviousPregnancyComplications p)
            => new(p.Id, p.VaginalBleedingDuringPregnancy, p.PregnancyDetailId);

        public static PreviousPregnancyComplicationsDetailsDto ToDetailsDto(this PreviousPregnancyComplications p)
            => new(p.Id, p.VaginalBleedingDuringPregnancy, p.PregnancyDetailId);

        public static PreviousPregnancyComplications ToDomain(this PreviousPregnancyComplicationsCreateDto dto)
            => PreviousPregnancyComplications.Create(dto.VaginalBleedingDuringPregnancy,
                dto.RecurrentMiscarriageMoreThanThree,
                dto.Diabetes,dto.Epilepsy,dto.HeartDisease,dto.Preeclampsia,dto.PretermBirthBefore8Months,dto.PregnancyDetailId); // factory not implemented
    }
}

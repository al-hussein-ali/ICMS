using ICMS.Application.DTOs.PreviousPregnancyDeliveryComplications;
using ICMS.Domain.Entites.Common;
using ICMS.Domain.Entites.Identity;
using ICMS.Domain.Entites.Clinical;
using ICMS.Domain.Entites.Maternal;
using ICMS.Domain.Entites.Visits;
using ICMS.Domain.Entites.Audit;
using ICMS.Domain.Entites.Geography;

namespace ICMS.Application.Extensions
{
    public static class PreviousPregnancyDeliveryComplicationsExtensions
    {
        public static PreviousPregnancyDeliveryComplicationsReadDto ToReadDto(this PreviousPregnancyDeliveryComplications p)
            => new(p.Id, p.CesareanSection, p.PregnancyDetailId);

        public static PreviousPregnancyDeliveryComplicationsDetailsDto ToDetailsDto(this PreviousPregnancyDeliveryComplications p)
            => new(p.Id, p.CesareanSection, p.PregnancyDetailId);

        public static PreviousPregnancyDeliveryComplications ToDomain(this PreviousPregnancyDeliveryComplicationsCreateDto dto)
            => PreviousPregnancyDeliveryComplications.Create(dto.CesareanSection,dto.AssistedDeliver,dto.StillbirthOrMultipleDeaths,dto.PregnancyDetailId);
    }
}

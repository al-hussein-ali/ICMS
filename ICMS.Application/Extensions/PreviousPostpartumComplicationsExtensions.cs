using ICMS.Application.DTOs.PreviousPostpartumComplications;
using ICMS.Domain.Entites.Common;
using ICMS.Domain.Entites.Identity;
using ICMS.Domain.Entites.Clinical;
using ICMS.Domain.Entites.Maternal;
using ICMS.Domain.Entites.Visits;
using ICMS.Domain.Entites.Audit;
using ICMS.Domain.Entites.Geography;

namespace ICMS.Application.Extensions
{
    public static class PreviousPostpartumComplicationsExtensions
    {
        public static PreviousPostpartumComplicationsReadDto ToReadDto(this PreviousPostpartumComplications p)
            => new(p.Id, p.VaginalBleeding, p.PregnancyDetailId);

        public static PreviousPostpartumComplicationsDetailsDto ToDetailsDto(this PreviousPostpartumComplications p)
            => new(p.Id, p.VaginalBleeding, p.PregnancyDetailId);

        public static PreviousPostpartumComplications ToDomain(this PreviousPostpartumComplicationsCreateDto dto)
            => PreviousPostpartumComplications.Create(dto.VaginalBleeding,dto.PlacentaRetention,dto.VaginalFistula,dto.PuerperalSepsis,dto.NeonatalDeathWithinFirstWeek,dto.PregnancyDetailId);
    }
}

using ICMS.Application.DTOs.PreviousPostartumComplications;
using ICMS.Domain.Entites;

namespace ICMS.Application.Extensions
{
    public static class PreviousPostartumComplicationsExtensions
    {
        public static PreviousPostartumComplicationsReadDto ToReadDto(this PreviousPostartumComplications p)
            => new(p.Id, p.VaginalBleeding, p.PregnancyDetailId);

        public static PreviousPostartumComplicationsDetailsDto ToDetailsDto(this PreviousPostartumComplications p)
            => new(p.Id, p.VaginalBleeding, p.PregnancyDetailId);

        public static PreviousPostartumComplications ToDomain(this PreviousPostartumComplicationsCreateDto dto)
            => PreviousPostartumComplications.Create(dto.VaginalBleeding,dto.PlacentaRetention,dto.VaginalFistula,dto.PuerperalSepsis,dto.NeonatalDeathWithinFirstWeek,dto.PregnancyDetailId); // factory not implemented
    }
}

using ICMS.Application.DTOs.ImmunizationRecord;
using ICMS.Domain.Entites.Common;
using ICMS.Domain.Entites.Identity;
using ICMS.Domain.Entites.Clinical;
using ICMS.Domain.Entites.Maternal;
using ICMS.Domain.Entites.Visits;
using ICMS.Domain.Entites.Audit;
using ICMS.Domain.Entites.Geography;

namespace ICMS.Application.Extensions
{
    public static class ImmunizationRecordExtensions
    {
        public static ImmunizationRecordReadDto ToReadDto(this ImmunizationRecord ir)
            => new(ir.Id, ir.IndividualId, ir.DoseId, ir.FieldVisitId, ir.VaccinationDate, ir.TakenIn);

        public static ImmunizationRecordDetailsDto ToDetailsDto(this ImmunizationRecord ir)
            => new(ir.Id, ir.IndividualId, ir.DoseId, ir.FieldVisitId, ir.VaccinationDate, ir.TakenIn, ir.Notes);

        public static ImmunizationRecord ToDomain(this ImmunizationRecordCreateDto dto)
            => ImmunizationRecord.Create(dto.IndividualId,dto.DoseId,dto.VaccinationDate,dto.TakenIn,dto.FieldVisitId,dto.Notes); // needs proper domain creation logic if required
    }
}

using ICMS.Application.DTOs.ImmunizationRecord;
using ICMS.Domain.Entites.Clinical;

namespace ICMS.Application.Extensions
{
    public static class ImmunizationRecordExtensions
    {
        public static ImmunizationRecordReadDto ToReadDto(this ImmunizationRecord ir)
            => new(ir.Id, ir.IndividualId, ir.DoseId, ir.FieldVisitId, ir.VaccinationDate, ir.TakenIn, ir.BatchId);

        public static ImmunizationRecordDetailsDto ToDetailsDto(this ImmunizationRecord ir)
            => new(ir.Id, ir.IndividualId, ir.DoseId, ir.FieldVisitId, ir.VaccinationDate, ir.TakenIn, ir.Notes);

        public static ImmunizationRecord ToDomain(this ImmunizationRecordCreateDto dto, int userId)
            => ImmunizationRecord.Create(dto.IndividualId, dto.DoseId, dto.VaccinationDate, dto.TakenIn, userId, dto.FieldVisitId, dto.Notes, dto.BatchId);
    }
}

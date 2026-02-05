using System;

namespace ICMS.Application.DTOs.Dose
{
    public record DoseReadDto(int Id, int VaccineId, string DoseName, byte DoseOrder);
}

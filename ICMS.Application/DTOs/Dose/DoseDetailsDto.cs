using System;

namespace ICMS.Application.DTOs.Dose
{
    public record DoseDetailsDto(int Id, int VaccineId, string DoseName, byte DoseOrder, string? Notes);
}

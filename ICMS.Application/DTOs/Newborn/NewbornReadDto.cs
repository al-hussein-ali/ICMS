using System;
using ICMS.Domain.Enums;

namespace ICMS.Application.DTOs.Newborn
{
    public record NewbornReadDto(int Id, int PregnancyDetailsId, NewbornStatus NewbornStatus, decimal NewbornWeightInGrams, ICMS.Domain.Enums.Gender NewbornGender);
}

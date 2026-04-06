using ICMS.Application.DTOs.Newborn;
using ICMS.Domain.Entites.Common;
using ICMS.Domain.Entites.Identity;
using ICMS.Domain.Entites.Clinical;
using ICMS.Domain.Entites.Maternal;
using ICMS.Domain.Entites.Visits;
using ICMS.Domain.Entites.Audit;
using ICMS.Domain.Entites.Geography;

namespace ICMS.Application.Extensions
{
    public static class NewbornExtensions
    {
        public static NewbornReadDto ToReadDto(this Newborn n)
            => new(n.Id, n.PregnancyDetailsId, n.NewbornStatus, n.NewbornWeightInGrams, n.NewbornGender);

        public static NewbornDetailsDto ToDetailsDto(this Newborn n)
            => new(n.Id, n.PregnancyDetailsId, n.NewbornStatus, n.NewbornWeightInGrams, n.NewbornGender);

        public static Newborn ToDomain(this NewbornCreateDto dto)
            => Newborn.Create(dto.PregnancyDetailsId,dto.NewbornStatus,dto.NewbornWeightInGrams,dto.NewbornGender);
    }
}

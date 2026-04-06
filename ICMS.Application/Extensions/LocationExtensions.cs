using ICMS.Application.DTOs.Geography;
using ICMS.Domain.Entites.Geography;

namespace ICMS.Application.Extensions
{
    public static class LocationExtensions
    {
        public static LocationReadDto ToReadDto(this Governorate g) => new(g.Id, g.Name);
        
        public static LocationReadDto ToReadDto(this Directorate d) => new(d.Id, d.Name);
        
        public static LocationReadDto ToReadDto(this Neighborhood n) => new(n.Id, n.Name);
        
        public static LocationReadDto ToReadDto(this SubNeighborhood s) => new(s.Id, s.Name);
    }
}

using ICMS.Domain.Entites.Geography;
using ICMS.Domain.Entites.Audit;
using ICMS.Domain.Entites.Visits;
using ICMS.Domain.Entites.Maternal;
using ICMS.Domain.Entites.Clinical;
using ICMS.Domain.Entites.Identity;
using ICMS.Domain.Entites.Common;
namespace ICMS.Domain.Entites.Geography
{
    public class SubNeighborhood : BaseEntity<int>
    {
        public string Name { get; private set; } = string.Empty;
        public int NeighborhoodId { get; private set; }
        public Neighborhood Neighborhood { get; private set; } = null!;

        private SubNeighborhood() { }

        public static SubNeighborhood Create(string name, int neighborhoodId)
        {
            return new SubNeighborhood { Name = name, NeighborhoodId = neighborhoodId };
        }
    }
}

using ICMS.Domain.Entites.Geography;
using ICMS.Domain.Entites.Audit;
using ICMS.Domain.Entites.Visits;
using ICMS.Domain.Entites.Maternal;
using ICMS.Domain.Entites.Clinical;
using ICMS.Domain.Entites.Identity;
using ICMS.Domain.Entites.Common;
using System.Collections.Generic;

namespace ICMS.Domain.Entites.Geography
{
    public class Neighborhood : BaseEntity<int>
    {
        public string Name { get; private set; } = string.Empty;
        public int DirectorateId { get; private set; }
        public Directorate Directorate { get; private set; } = null!;

        private readonly List<SubNeighborhood> _subNeighborhoods = new();
        public IReadOnlyCollection<SubNeighborhood> SubNeighborhoods => _subNeighborhoods.AsReadOnly();

        private Neighborhood() { }

        public static Neighborhood Create(string name, int directorateId)
        {
            return new Neighborhood { Name = name, DirectorateId = directorateId };
        }
    }
}

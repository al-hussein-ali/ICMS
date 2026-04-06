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
    public class Directorate : BaseEntity<int>
    {
        public string Name { get; private set; } = string.Empty;
        public int GovernorateId { get; private set; }
        public Governorate Governorate { get; private set; } = null!;

        private readonly List<Neighborhood> _neighborhoods = new();
        public IReadOnlyCollection<Neighborhood> Neighborhoods => _neighborhoods.AsReadOnly();

        private Directorate() { }

        public static Directorate Create(string name, int governorateId)
        {
            return new Directorate { Name = name, GovernorateId = governorateId };
        }
    }
}

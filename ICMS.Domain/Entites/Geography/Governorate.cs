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
    public class Governorate : BaseEntity<int>
    {
        public string Name { get; private set; } = string.Empty;

        private readonly List<Directorate> _directorates = new();
        public IReadOnlyCollection<Directorate> Directorates => _directorates.AsReadOnly();

        private Governorate() { }

        public static Governorate Create(string name)
        {
            return new Governorate { Name = name };
        }
    }
}

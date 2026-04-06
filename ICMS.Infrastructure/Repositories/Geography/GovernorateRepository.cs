using ICMS.Application.Interfaces.Repositories;
using ICMS.Domain.Entites.Geography;
using ICMS.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace ICMS.Infrastructure.Repositories.Geography
{
    public class GovernorateRepository : Repository<Governorate, int>, IGovernorateRepository
    {
        public GovernorateRepository(AppDbContext context) : base(context)
        {
        }
    }
}

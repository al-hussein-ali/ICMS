using ICMS.Application.Interfaces.Repositories;
using ICMS.Domain.Entites.Identity;
using ICMS.Infrastructure.Persistence.Data;

namespace ICMS.Infrastructure.Repositories.Identity
{
    public class UserDeviceRepository : Repository<UserDevice, int>, IUserDeviceRepository
    {
        public UserDeviceRepository(AppDbContext context) : base(context)
        {
        }
    }
}

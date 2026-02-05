using ICMS.Application.Interfaces.Repositories;
using ICMS.Domain.Entites;
using ICMS.Infrastructure.Persistence.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMS.Infrastructure.Repositories
{
    public class FieldVisitRepository : Repository<FieldVisit,int>,IFieldVisitRepository
    {
        public FieldVisitRepository(AppDbContext context) : base(context)
        {
            
        }
    }
}

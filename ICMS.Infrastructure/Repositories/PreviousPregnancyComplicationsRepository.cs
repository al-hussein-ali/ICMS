using ICMS.Application.Interfaces.Repositories;
using ICMS.Domain.Entites;
using ICMS.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore.Query.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMS.Infrastructure.Repositories
{
    public class PreviousPregnancyComplicationsRepository : Repository<PreviousPregnancyComplications,int>, IPreviousPregnancyComplicationsRepository
    {
        public PreviousPregnancyComplicationsRepository(AppDbContext context) : base(context)
        {
            
        }
    }
}

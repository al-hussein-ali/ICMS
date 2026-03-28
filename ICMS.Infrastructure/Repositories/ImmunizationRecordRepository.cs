using EFCore.BulkExtensions;
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
    public class ImmunizationRecordRepository : Repository<ImmunizationRecord, Guid>, IImmunizationRecordRepository
    {
        public ImmunizationRecordRepository(AppDbContext context) : base(context)
        {
            
        }

        public async Task BulkInsertAsync(List<ImmunizationRecord> immunizationRecords, CancellationToken ct = default)
        {
            await base._context.BulkInsertAsync(immunizationRecords,cancellationToken: ct);
        }
    }
}

using ICMS.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMS.Application.Interfaces.Repositories
{
    public interface IImmunizationRecordRepository : IRepository<ImmunizationRecord,Guid>
    {
        Task BulkInsertAsync(List<ImmunizationRecord> immunizationRecords, CancellationToken ct = default);
        
    }
}

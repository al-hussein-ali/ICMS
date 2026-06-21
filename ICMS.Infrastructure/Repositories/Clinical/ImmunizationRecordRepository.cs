using EFCore.BulkExtensions;
using ICMS.Application.Interfaces.Repositories;
using ICMS.Domain.Entites.Common;
using ICMS.Domain.Entites.Identity;
using ICMS.Domain.Entites.Clinical;
using ICMS.Domain.Entites.Maternal;
using ICMS.Domain.Entites.Visits;
using ICMS.Domain.Entites.Audit;
using ICMS.Domain.Entites.Geography;
using ICMS.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMS.Infrastructure.Repositories.Clinical
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

        public async Task<IReadOnlyList<ImmunizationRecord>> GetRecordsWithDetailsForVisitAsync(int fieldVisitId, CancellationToken ct = default)
        {
            return await _dbSet
                .Where(ir => ir.FieldVisitId == fieldVisitId)
                .Include(ir => ir.VaccinatedIndividual)
                    .ThenInclude(vi => vi.Person)
                .Include(ir => ir.Dose)
                .Include(ir => ir.User)
                    .ThenInclude(u => u.Person)
                .ToListAsync(ct);
        }
    }
}

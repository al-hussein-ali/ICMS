using ICMS.Application.Interfaces;
using ICMS.Application.Interfaces.Repositories;
using ICMS.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMS.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public IBatchRepository BatchRepository { get; }

        public IDoseReportRepository DoseReportRepository { get; }

        public IDoseRepository DoseRepository { get; }

        public IFieldVisitRepository FieldVisitRepository { get; }

        public IHealthAdvisoryRepository HealthAdvisoryRepository { get; }

        public IImmunizationRecordRepository ImmunizationRecordRepository { get; }

        public INewbornRepository NewbornRepository { get; }

        public IPersonRepository PersonRepository { get; }

        public IPregnancyDetailsRepository PregnancyDetailsRepository { get; }

        public IPregnantWomanRepository PregnantWomanRepository { get; }

        public IPreviousPregnancyComplicationsRepository PreviousPregnancyComplicationsRepository { get; }

        public IPreviousPostartumComplicationsRepository PreviousPostartumComplicationsRepository { get; }

        public IPreviousPregnancyDeliveryComplicationsRepository PreviousPregnancyDeliveryComplicationsRepository { get; }

        public IRoleRepository RoleRepository { get; }

        public ITransactionRepository TransactionRepository { get; }

        public IUserRepository UserRepository { get; }

        public IVaccinatedIndividualRepository VaccinatedIndividualRepository { get; }

        public IVaccineRepository VaccineRepository { get; }

        public IVisitDetailsRepository VisitDetailsRepository { get; }



        public UnitOfWork(AppDbContext context)
        {
            _context = context;

            BatchRepository = new BatchRepository(_context);
            DoseRepository = new DoseRepository(_context);
            DoseReportRepository = new DoseReportRepository(_context);
            DoseRepository = new DoseRepository(_context);
            FieldVisitRepository = new FieldVisitRepository(_context);
            HealthAdvisoryRepository = new HealthAdvisoryRepository(_context);
            ImmunizationRecordRepository = new ImmunizationRecordRepository(_context);
            NewbornRepository = new NewbornRepository(_context);
            PersonRepository = new PersonRepository(_context);
            PregnancyDetailsRepository = new PregnancyDetailsRepository(_context);
            PregnantWomanRepository = new PregnantWomanRepository(_context);
            PreviousPregnancyComplicationsRepository = new PreviousPregnancyComplicationsRepository(_context);
            PreviousPostartumComplicationsRepository = new PreviousPostartumComplicationsRepository(_context);
            PreviousPregnancyDeliveryComplicationsRepository = new PreviousPregnancyDeliveryComplicationsRepository(_context);
            RoleRepository = new RoleRepository(_context);
            TransactionRepository = new TransactionRepository(_context);
            UserRepository = new UserRepository(_context);
            VaccinatedIndividualRepository = new VaccinatedIndividualRepository(_context);
            VaccineRepository = new VaccineRepository(_context);
            VisitDetailsRepository = new VisitDetailsRepository(_context);
        }


        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task ExecuteInTransactionAsync(Func<Task> action)
        {
            if (_context.Database.CurrentTransaction != null)
            {
                // Already inside a transaction → just execute
                await action().ConfigureAwait(false);
                return;
            }

            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                await action().ConfigureAwait(false);
                await _context.SaveChangesAsync().ConfigureAwait(false);
                await transaction.CommitAsync().ConfigureAwait(false);
            }
            catch
            {
                await transaction.RollbackAsync().ConfigureAwait(false);
                throw;
            }
            finally
            {

                await transaction.DisposeAsync().ConfigureAwait(false);
                await DisposeAsync();
            }
        }

        public void Dispose()
        {
            _context?.Dispose();
            GC.SuppressFinalize(this);
        }

        public async ValueTask DisposeAsync()
        {
            if (_context != null)
                await _context.DisposeAsync().ConfigureAwait(false);

            GC.SuppressFinalize(this);
        }
    }
}

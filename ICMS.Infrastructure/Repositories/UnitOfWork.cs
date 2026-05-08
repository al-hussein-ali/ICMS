using ICMS.Application.Interfaces;
using ICMS.Application.Interfaces.Repositories;
using ICMS.Infrastructure.Persistence.Data;
using ICMS.Infrastructure.Repositories.Clinical;
using ICMS.Infrastructure.Repositories.Geography;
using ICMS.Infrastructure.Repositories.Identity;
using ICMS.Infrastructure.Repositories.Maternal;
using ICMS.Infrastructure.Repositories.Visits;
using ICMS.Infrastructure.Repositories.Audit; // Assuming TransactionRepository might be here
using System;
using System.Threading;
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
        public IPreviousPostpartumComplicationsRepository PreviousPostpartumComplicationsRepository { get; }
        public IPreviousPregnancyDeliveryComplicationsRepository PreviousPregnancyDeliveryComplicationsRepository { get; }
        public IRoleRepository RoleRepository { get; }
        public ITransactionRepository TransactionRepository { get; }
        public IUserRepository UserRepository { get; }
        public IUserDeviceRepository UserDeviceRepository { get; }
        public IUserRoleRepository UserRoleRepository { get; }
        public IVaccinatedIndividualRepository VaccinatedIndividualRepository { get; }
        public IVaccineRepository VaccineRepository { get; }
        public IGovernorateRepository GovernorateRepository { get; }
        public IDirectorateRepository DirectorateRepository { get; }
        public INeighborhoodRepository NeighborhoodRepository { get; }
        public ISubNeighborhoodRepository SubNeighborhoodRepository { get; }
        public IVisitDetailsRepository VisitDetailsRepository { get; }
        public IVaccinationScheduleRepository VaccinationScheduleRepository { get; }
        public IRefreshTokenRepository RefreshTokenRepository { get; }

        public UnitOfWork(AppDbContext context)
        {
            _context = context;

            BatchRepository = new BatchRepository(_context);
            DoseRepository = new DoseRepository(_context);
            DoseReportRepository = new DoseReportRepository(_context);
            FieldVisitRepository = new FieldVisitRepository(_context);
            HealthAdvisoryRepository = new HealthAdvisoryRepository(_context);
            ImmunizationRecordRepository = new ImmunizationRecordRepository(_context);
            NewbornRepository = new NewbornRepository(_context);
            PersonRepository = new PersonRepository(_context);
            PregnancyDetailsRepository = new PregnancyDetailsRepository(_context);
            PregnantWomanRepository = new PregnantWomanRepository(_context);
            PreviousPregnancyComplicationsRepository = new PreviousPregnancyComplicationsRepository(_context);
            PreviousPostpartumComplicationsRepository = new PreviousPostpartumComplicationsRepository(_context);
            PreviousPregnancyDeliveryComplicationsRepository = new PreviousPregnancyDeliveryComplicationsRepository(_context);
            RoleRepository = new RoleRepository(_context);
            TransactionRepository = new TransactionRepository(_context);
            UserRepository = new UserRepository(_context);
            UserDeviceRepository = new UserDeviceRepository(_context);
            UserRoleRepository = new UserRoleRepository(_context);
            VaccinatedIndividualRepository = new VaccinatedIndividualRepository(_context);
            VaccineRepository = new VaccineRepository(_context);
            GovernorateRepository = new GovernorateRepository(_context);
            DirectorateRepository = new DirectorateRepository(_context);
            NeighborhoodRepository = new NeighborhoodRepository(_context);
            SubNeighborhoodRepository = new SubNeighborhoodRepository(_context);
            VisitDetailsRepository = new VisitDetailsRepository(_context);
            VaccinationScheduleRepository = new VaccinationScheduleRepository(_context);
            RefreshTokenRepository = new RefreshTokenRepository(_context);
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task ExecuteInTransactionAsync(Func<Task> action)
        {
            var isInMemory = _context.Database.ProviderName == "Microsoft.EntityFrameworkCore.InMemory";
            if (isInMemory || _context.Database.CurrentTransaction != null)
            {
                await action();
                await _context.SaveChangesAsync();
                return;
            }

            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                await action();
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> action)
        {
            var isInMemory = _context.Database.ProviderName == "Microsoft.EntityFrameworkCore.InMemory";
            if (isInMemory || _context.Database.CurrentTransaction != null)
            {
                var result = await action();
                await _context.SaveChangesAsync();
                return result;
            }

            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var result = await action();
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return result;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
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
                await _context.DisposeAsync();

            GC.SuppressFinalize(this);
        }

        public void RollbackTracker()
        {
            _context.ChangeTracker.Clear();
        }
    }
}


using ICMS.Application.Interfaces.Repositories;

namespace ICMS.Application.Interfaces
{
    public interface IUnitOfWork : IDisposable, IAsyncDisposable
    {
        IBatchRepository BatchRepository { get; }
        IDoseReportRepository DoseReportRepository { get; }
        IDoseRepository DoseRepository { get; }
        IFieldVisitRepository FieldVisitRepository { get; }
        IHealthAdvisoryRepository HealthAdvisoryRepository { get; }
        IImmunizationRecordRepository ImmunizationRecordRepository { get; }
        INewbornRepository NewbornRepository { get; }
        IPersonRepository PersonRepository { get; }
        IPregnancyDetailsRepository PregnancyDetailsRepository { get; }
        IPregnantWomanRepository PregnantWomanRepository { get; }
        IPreviousPregnancyComplicationsRepository PreviousPregnancyComplicationsRepository { get; }
        IPreviousPostpartumComplicationsRepository PreviousPostpartumComplicationsRepository { get; }
        IPreviousPregnancyDeliveryComplicationsRepository PreviousPregnancyDeliveryComplicationsRepository { get; }
        IRoleRepository RoleRepository { get; }
        ITransactionRepository TransactionRepository { get; }
        IUserRepository UserRepository { get; }
        IUserDeviceRepository UserDeviceRepository { get; }
        IUserRoleRepository UserRoleRepository { get; }
        IVaccinatedIndividualRepository VaccinatedIndividualRepository { get; }
        IVaccineRepository VaccineRepository { get; }
        IGovernorateRepository GovernorateRepository { get; }
        IDirectorateRepository DirectorateRepository { get; }
        INeighborhoodRepository NeighborhoodRepository { get; }
        ISubNeighborhoodRepository SubNeighborhoodRepository { get; }
        IVisitDetailsRepository VisitDetailsRepository { get; }
        IVaccinationScheduleRepository VaccinationScheduleRepository { get; }
        IRefreshTokenRepository RefreshTokenRepository { get; }
        INotificationRepository NotificationRepository { get; }


        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        void RollbackTracker();


        Task ExecuteInTransactionAsync(Func<Task> action);
        Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> action);
    }
}

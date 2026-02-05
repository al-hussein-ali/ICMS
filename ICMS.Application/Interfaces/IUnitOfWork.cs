using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICMS.Application.Interfaces.Repositories;

namespace ICMS.Application.Interfaces
{
    public interface IUnitOfWork : IDisposable,IAsyncDisposable
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
        IPreviousPregnancyComplicationsRepository PreviousPregnancyComplicationsRepository{ get; }
        IPreviousPostartumComplicationsRepository PreviousPostartumComplicationsRepository { get; }
        IPreviousPregnancyDeliveryComplicationsRepository PreviousPregnancyDeliveryComplicationsRepository { get; }
        IRoleRepository RoleRepository { get; }
        ITransactionRepository TransactionRepository { get; }
        IUserRepository UserRepository { get; }
        IVaccinatedIndividualRepository VaccinatedIndividualRepository { get; }
        IVaccineRepository VaccineRepository { get;}
        IVisitDetailsRepository VisitDetailsRepository { get; }


        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);


        Task ExecuteInTransactionAsync(Func<Task> action);
    }
}

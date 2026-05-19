using EntityFramework.Exceptions.PostgreSQL;
using ICMS.Application.Interfaces;
using ICMS.Application.Interfaces.Reports;
using ICMS.Application.Interfaces.Repositories;
using ICMS.Application.Interfaces.Services;
using ICMS.Application.Services;
using ICMS.Infrastructure.ExternalServices;
using ICMS.Infrastructure.Persistence.Data;
using ICMS.Infrastructure.Reports;
using ICMS.Infrastructure.Reports.DataFetchers;
using ICMS.Infrastructure.Reports.Templates;
using ICMS.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;


namespace ICMS.Infrastructure.Extensions
{
    public static class InfrastructureExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString, bool isTesting = false)
        {
            if (!isTesting)
            {
                services.AddDbContextPool<AppDbContext>(options =>
                {
                    options.UseNpgsql(connectionString, o => 
                        o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery))
                        .UseExceptionProcessor();
                });
            }
            services.AddLocalization(options => options.ResourcesPath = "Resources");
            services.AddScoped<ILocalizer, ResourceLocalizer>();

            services.AddMemoryCache();
            services.AddSingleton<ICMS.Application.Interfaces.Services.ICacheService, MemoryCacheService>();

            services.AddScoped(typeof(IRepository<,>), typeof(Repository<,>));


            services.AddScoped<IUnitOfWork, UnitOfWork>();
            //services.AddScoped<IPersonRepository, PersonRepository>();

            services.AddScoped<IPersonService, PersonService>();
            services.AddScoped<IVaccinatedIndividualService, VaccinatedIndividualService>();
            services.AddScoped<IVaccineService, VaccineService>();
            services.AddScoped<IDoseService, DoseService>();
            services.AddScoped<IImmunizationRecordService, ImmunizationRecordService>();
            services.AddScoped<IImmunizationService, ImmunizationService>();
            services.AddScoped<IFieldVisitService, FieldVisitService>();
            services.AddScoped<ILocationService, LocationService>();
            services.AddScoped<IReproductiveHealthService, ReproductiveHealthService>();
            services.AddScoped<ISchedulesService, SchedulesService>();
            services.AddScoped<IIdentityService, IdentityService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IMissedDoseTrackerService, MissedDoseTrackerService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IRefreshTokenService, RefreshTokenService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IBatchService, BatchService>();
            services.AddScoped<ITransactionService, TransactionService>();
            services.AddScoped<IDoseReportService, DoseReportService>();
            services.AddScoped<IPushNotificationService, PushNotificationService>();
            services.AddScoped<IHealthAdvisoryService, HealthAdvisoryService>();
            services.AddScoped<IUserDeviceService, UserDeviceService>();
            services.AddScoped<IAdvisoryDispatchBackgroundService, AdvisoryDispatchBackgroundService>();
            services.AddScoped<IBatchExpirationTrackerService, BatchExpirationTrackerService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<ISystemSettingService, SystemSettingService>();
            services.AddScoped<IDatabaseBackupService, DatabaseBackupService>();

            // Report generation: strategy pattern — fetchers and renderers
            services.AddScoped<IReportDataFetcher, VaccinatedIndividualsReportFetcher>();
            services.AddScoped<IReportDataFetcher, PregnantWomenReportFetcher>();
            services.AddScoped<IReportDataFetcher, InventoryReportFetcher>();
            services.AddScoped<IReportDataFetcher, DailyVaccinationReportFetcher>();


            services.AddScoped<IReportTemplateRenderer, VaccinatedIndividualsReportTemplate>();
            services.AddScoped<IReportTemplateRenderer, PregnantWomenReportTemplate>();
            services.AddScoped<IReportTemplateRenderer, InventoryReportTemplate>();
            services.AddScoped<IReportTemplateRenderer, DailyVaccinationReportTemplate>();


            services.AddScoped<IReportGeneratorJob, ReportGeneratorJob>();
            // Note: IReportNotificationService is registered in API layer (Program.cs)
            // because it needs IHubContext<ReportHub> which lives in API project
            services.AddScoped<IReportService, ICMS.Infrastructure.Reports.ReportService>();

            return services;
        }
    }
}

using EntityFramework.Exceptions.PostgreSQL;
using ICMS.Application.Interfaces;
using ICMS.Application.Interfaces.Repositories;
using ICMS.Application.Interfaces.Services;
using ICMS.Application.Services;
using ICMS.Infrastructure.Persistence.Data;
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
                    options.UseNpgsql(connectionString).UseExceptionProcessor();
                });
            }


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

            return services;
        }
    }
}

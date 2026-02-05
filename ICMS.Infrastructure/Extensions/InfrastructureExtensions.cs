using ICMS.Application.Interfaces;
using ICMS.Application.Interfaces.Repositories;
using ICMS.Application.Interfaces.Services;
using ICMS.Application.Services;
using ICMS.Infrastructure.Persistence.Data;
using ICMS.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMS.Infrastructure.Extensions
{
    public static class InfrastructureExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services,string connectionString)
        {
            services.AddDbContextPool<AppDbContext>(options =>  {
                
                options.UseNpgsql(connectionString);
            });


            services.AddScoped(typeof(IRepository<,>), typeof(Repository<,>));


            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IPersonRepository, PersonRepository>();

            services.AddScoped<IPersonService, PersonService>();

            return services;
        }
    }
}

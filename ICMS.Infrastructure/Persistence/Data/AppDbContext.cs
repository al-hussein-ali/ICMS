using ICMS.Domain.Entites.Common;
using ICMS.Domain.Entites.Identity;
using ICMS.Domain.Entites.Clinical;
using ICMS.Domain.Entites.Maternal;
using ICMS.Domain.Entites.Visits;
using ICMS.Domain.Entites.Audit;
using ICMS.Domain.Entites.Geography;
using ICMS.Infrastructure.Persistence.Config;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMS.Infrastructure.Persistence.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Governorate> Governorates { get; set; }
        public DbSet<Directorate> Directorates { get; set; }
        public DbSet<Neighborhood> Neighborhoods { get; set; }
        public DbSet<SubNeighborhood> SubNeighborhoods { get; set; }
        public DbSet<VaccinationSchedule> VaccinationSchedules { get; set; }

        public DbSet<Batch> Batches { get; set; }
        public DbSet<Dose> Doses { get; set; }
        public DbSet<DoseReport> DoseReports { get; set; }
        public DbSet<FieldVisit> FieldVisits { get; set; }
        public DbSet<FieldVisitUser> FieldVisitUsers { get; set; }
        public DbSet<HealthAdvisory> HealthAdvisories { get; set; }
        public DbSet<ImmunizationRecord> ImmunizationRecords { get; set; }
        public DbSet<Newborn> Newborns { get; set; }
        public DbSet<Person> People { get; set; }
        public DbSet<PregnancyDetails> PregnancyDetails { get; set; }
        public DbSet<PregnantWoman> PregnantWomen { get; set; }
        public DbSet<PreviousPostpartumComplications> PreviousPostpartumComplications { get; set; }
        public DbSet<PreviousPregnancyComplications> PreviousPregnancyComplications { get; set; }
        public DbSet<PreviousPregnancyDeliveryComplications> PreviousPregnancyDeliveryComplications { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<VaccinatedIndividual> VaccinatedIndividuals { get; set; }
        public DbSet<Vaccine> Vaccines { get; set; }
        public DbSet<VisitDetails> VisitDetails { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ICMS.Infrastructure.Persistence.Config.Identity.PersonConfig).Assembly);
            modelBuilder.SeedGeographicalData();

        }

    }
}

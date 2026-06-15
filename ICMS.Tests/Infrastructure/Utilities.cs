using ICMS.Domain.Entites.Clinical;
using ICMS.Domain.Entites.Identity;
using ICMS.Domain.Entites.Geography;
using ICMS.Domain.Enums;
using ICMS.Infrastructure.Persistence.Data;
using System;
using System.Collections.Generic;

namespace ICMS.Tests.Infrastructure
{
    public static class Utilities
    {
        public static void InitializeDbForTests(AppDbContext db)
        {
            // Clear all relevant tables
            db.VaccinationSchedules.RemoveRange(db.VaccinationSchedules);
            db.Doses.RemoveRange(db.Doses);
            db.Vaccines.RemoveRange(db.Vaccines);
            db.VaccinatedIndividuals.RemoveRange(db.VaccinatedIndividuals);
            db.People.RemoveRange(db.People.Where(p => p.Id != 999));
            db.SubNeighborhoods.RemoveRange(db.SubNeighborhoods);
            db.Neighborhoods.RemoveRange(db.Neighborhoods);
            db.Directorates.RemoveRange(db.Directorates);
            db.Governorates.RemoveRange(db.Governorates);
            db.SaveChanges();

            // Seed Geography
            var gov = Governorate.Create("Test Governorate");
            db.Governorates.Add(gov);
            db.SaveChanges();

            var dir = Directorate.Create("Test Directorate", gov.Id);
            db.Directorates.Add(dir);
            db.SaveChanges();

            var neighborhood = Neighborhood.Create("Test Neighborhood", dir.Id);
            db.Neighborhoods.Add(neighborhood);
            db.SaveChanges();

            var subNeighborhood = SubNeighborhood.Create("Test SubNeighborhood", neighborhood.Id);
            db.SubNeighborhoods.Add(subNeighborhood);
            db.SaveChanges();

            // Seed Clinical
            var vaccine = Vaccine.Create("BCG", "BCG-01", "Tuberculosis vaccine", true, 5, 0, 12, TargetAudience.InfantRoutine);
            db.Vaccines.Add(vaccine);
            db.SaveChanges();

            var dose = Dose.Create(vaccine.Id, "BCG Dose 1", 1, 0, "At Birth", true, "First dose");
            db.Doses.Add(dose);
            db.SaveChanges();

            // Seed Citizen
            var person = Person.Create("Test", "Child", "One", "ICMS", Gender.Male, new DateOnly(2024, 1, 1), "0000000000");
            db.People.Add(person);
            db.SaveChanges();

            var individual = VaccinatedIndividual.Create(dir.Id, neighborhood.Id, subNeighborhood.Id);
            individual.AssignPerson(person);
            
            // Generate initial schedules so AdministerDose has targets
            var allDoses = new List<Dose> { dose };
            individual.ScheduleInitialVaccines(allDoses, person.DateOfBirth);
            
            db.VaccinatedIndividuals.Add(individual);
            db.SaveChanges();
        }
    }
}

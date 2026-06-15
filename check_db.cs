using System;
using System.Linq;
using ICMS.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = Host.CreateDefaultBuilder()
    .ConfigureServices((context, services) =>
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql("Host=localhost;Port=5432;Database=ICMSDB;Username=postgres;Password=sa123456;"));
    })
    .Build();

using var scope = host.Services.CreateScope();
var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

try {
    var individualCount = db.VaccinatedIndividuals.Count();
    var visitCount = db.FieldVisits.Count();
    var relationCount = db.FieldVisitIndividuals.Count();
    var workerRelationCount = db.FieldVisitWorkers.Count();

    Console.WriteLine($"VaccinatedIndividuals: {individualCount}");
    Console.WriteLine($"FieldVisits: {visitCount}");
    Console.WriteLine($"FieldVisitIndividuals: {relationCount}");
    Console.WriteLine($"FieldVisitWorkers: {workerRelationCount}");

    var firstVisit = db.FieldVisits
        .Include(v => v.FieldVisitIndividuals)
        .Include(v => v.FieldVisitWorkers)
        .OrderByDescending(v => v.Id)
        .FirstOrDefault();

    if (firstVisit != null) {
        Console.WriteLine($"\nLatest FieldVisit: ID={firstVisit.Id}, Campaign={firstVisit.CampaignName}");
        Console.WriteLine($"Individuals count: {firstVisit.FieldVisitIndividuals.Count}");
        Console.WriteLine($"Workers count: {firstVisit.FieldVisitWorkers.Count}");
    }

    Console.WriteLine("\nAll Vaccinated Individuals in DB:");
    var individuals = db.VaccinatedIndividuals.Include(vi => vi.Person).Take(10).ToList();
    foreach (var ind in individuals) {
        Console.WriteLine($"ID={ind.Id}, Name={ind.Person?.FullName}, Card={ind.CardNumber}");
    }
} catch (Exception ex) {
    Console.WriteLine($"Error: {ex.Message}");
    if (ex.InnerException != null) {
        Console.WriteLine($"Inner Error: {ex.InnerException.Message}");
    }
}

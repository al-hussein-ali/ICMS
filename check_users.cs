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
            options.UseNpgsql("Host=localhost;Database=icms_db;Username=postgres;Password=postgres")); // Adjust if needed
    })
    .Build();

using var scope = host.Services.CreateScope();
var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

try {
    var userCount = db.Users.Count();
    var firstUser = db.Users.FirstOrDefault();
    Console.WriteLine($"User count: {userCount}");
    if (firstUser != null) {
        Console.WriteLine($"First User ID: {firstUser.Id}, Name: {firstUser.UserName}");
    }
} catch (Exception ex) {
    Console.WriteLine($"Error: {ex.Message}");
}

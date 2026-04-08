using FluentValidation;
using Hangfire;
using Hangfire.PostgreSql;
using ICMS.API.Handlers;
using ICMS.Application.Validators;
using ICMS.Infrastructure.Extensions;
using Scalar.AspNetCore;
using ICMS.Application.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Configuration.AddJsonFile("appsettings.json");


builder.Services.AddInfrastructure(builder.Configuration.GetConnectionString("DefaultConnection")!,
    builder.Environment.IsEnvironment("Testing"));

builder.Services.AddHangfire(configuration => configuration
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UsePostgreSqlStorage(c =>
        c.UseNpgsqlConnection(builder.Configuration.GetConnectionString("DefaultConnection"))));

builder.Services.AddHangfireServer();

builder.Services.AddControllers();

var jwtOptions = builder.Configuration.GetSection("JwtOptions").Get<JwtOptions>() ?? throw new InvalidOperationException("JwtOptions are missing");
builder.Services.AddSingleton(jwtOptions);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtOptions.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtOptions.Audience,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SigningKey))
        };
    });


builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services
    .AddValidatorsFromAssemblyContaining<
        PaginationValidator>(includeInternalTypes: true); // fluent validation registration

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

app.UseExceptionHandler();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.WithTitle("ICMS API");
        options.WithTheme(ScalarTheme.Default);
        options.WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
    });
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseHangfireDashboard("/hangfire");

using (var scope = app.Services.CreateScope())
{
    var recurringJobManager = scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();

    // Register the missed dose tracker to run daily at midnight
    recurringJobManager.AddOrUpdate<ICMS.Application.Interfaces.Services.IMissedDoseTrackerService>(
        "DailyMissedDoseTracker",
        service => service.MarkMissedDosesAsync(default),
        Cron.Daily);
}

app.MapControllers();

app.Run();

public partial class Program
{
}

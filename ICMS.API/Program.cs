using Serilog;
using ICMS.API.Handlers;
using ICMS.API.Extensions;
using ICMS.Infrastructure.Extensions;
using ICMS.Application.Validators;
using FluentValidation;
using Hangfire;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services));

// Add services to the container.
builder.Configuration.AddJsonFile("appsettings.json");

builder.Services.AddInfrastructure(builder.Configuration.GetConnectionString("DefaultConnection")!,
    builder.Environment.IsEnvironment("Testing"));

builder.Services.AddHangfireServices(builder.Configuration);

builder.Services.AddSignalR();

// Register SignalR notification service here because it depends on IHubContext<ReportHub>.
builder.Services.AddScoped<ICMS.Application.Interfaces.Services.IReportNotificationService>(sp =>
{
    var hubContext = sp.GetRequiredService<Microsoft.AspNetCore.SignalR.IHubContext<ICMS.API.Hubs.ReportHub>>();
    return new ICMS.Infrastructure.ExternalServices.SignalRReportNotificationService(hubContext);
});

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    });

builder.Services.AddApiAuthentication(builder.Configuration);

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddValidatorsFromAssemblyContaining<PaginationValidator>(includeInternalTypes: true);

builder.Services.AddApiRateLimiter();

builder.Services.AddOpenApi();

builder.Services.AddApiCors();

var app = builder.Build();

app.UseSerilogRequestLogging();
app.UseExceptionHandler();
app.UseRouting();
app.UseCors("AllowFrontend");

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseStaticFiles();

app.UseRequestLocalization(options =>
{
    var supportedCultures = new[] { "en-US", "ar-YE", "ar" };
    options.SetDefaultCulture("en-US")
        .AddSupportedCultures(supportedCultures)
        .AddSupportedUICultures(supportedCultures);
});

app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();

app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = new[] { new HangfireDashboardAuthorizationFilter() }
});

app.InitializeFirebase();

await app.RegisterRecurringJobsAndStartupTasksAsync();

// Ensure reports output directory exists
Directory.CreateDirectory(Path.Combine("wwwroot", "reports"));

app.MapControllers();
app.MapHub<ICMS.API.Hubs.ReportHub>("/hubs/reports");

app.Run();

public partial class Program
{
}

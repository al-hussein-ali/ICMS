using FluentValidation;
using Hangfire;
using Hangfire.PostgreSql;
using ICMS.API.Handlers;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using ICMS.Application.Validators;
using ICMS.Infrastructure.Extensions;
// using Scalar.AspNetCore;
using ICMS.Application.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Serilog;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services));
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

builder.Services.AddSignalR();

// Register SignalR notification service here because it depends on IHubContext<ReportHub>.
// We inject the concrete hub context via a factory and supply it as IHubContext<Hub>
// (the base type used in SignalRReportNotificationService), avoiding a circular dep.
builder.Services.AddScoped<ICMS.Application.Interfaces.Services.IReportNotificationService>(sp =>
{
    var hubContext = sp.GetRequiredService<IHubContext<ICMS.API.Hubs.ReportHub>>();
    return new ICMS.Infrastructure.ExternalServices.SignalRReportNotificationService(hubContext);
});

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    });

var jwtOptions = builder.Configuration.GetSection("JwtOptions").Get<JwtOptions>() ??
                 throw new InvalidOperationException("JwtOptions are missing");
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
        // Allow JWT via query param for SignalR connections
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;
                if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
                {
                    context.Token = accessToken;
                }

                return Task.CompletedTask;
            }
        };
    });


builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddValidatorsFromAssemblyContaining<
    PaginationValidator>(includeInternalTypes: true); // fluent validation registration

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    // Add Retry-After header so the frontend knows exactly how long to wait
    options.OnRejected = async (context, cancellationToken) =>
    {
        context.HttpContext.Response.Headers.RetryAfter = "10";
        await Task.CompletedTask;
    };

    // Default policy: 300 requests per 1 minute per IP
    // Increased from 100 to handle SPA burst fetching (e.g. after PDF save dialog closes)
    options.AddFixedWindowLimiter("fixed", opt =>
    {
        opt.Window = TimeSpan.FromMinutes(1);
        opt.PermitLimit = 300;
        opt.QueueLimit = 5;
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
    });

    // Stricter policy: 20 requests per 1 minute (e.g. for report generation or auth)
    options.AddFixedWindowLimiter("stricter", opt =>
    {
        opt.Window = TimeSpan.FromMinutes(1);
        opt.PermitLimit = 20;
        opt.QueueLimit = 2;
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
    });

    // Per IP address limiting (Global) - 300 req/min is sufficient for a single-user SPA in dev
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? context.Request.Headers.Host.ToString(),
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 300,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0
            }));
});

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Configure Firebase
var firebaseCredentialsPath = Path.Combine(builder.Environment.ContentRootPath, "firebase-service-account.json");
if (File.Exists(firebaseCredentialsPath))
{
    FirebaseApp.Create(new AppOptions
    {
        Credential = CredentialFactory
            .FromFile<ServiceAccountCredential>(firebaseCredentialsPath)
            .ToGoogleCredential()
    });
}
else
{
    // Log warning or throw exception if Firebase is required for startup
    Console.WriteLine(
        $"Firebase configuration file not found at {firebaseCredentialsPath}. Push notifications will fail.");
}

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
                "http://localhost:5173", "https://localhost:5173",
                "http://localhost:5174", "https://localhost:5174",
                "http://localhost:5175", "https://localhost:5175",
                "http://localhost:5176", "https://localhost:5176",
                "http://127.0.0.1:5173", "https://127.0.0.1:5173",
                "http://127.0.0.1:5174", "https://127.0.0.1:5174",
                "http://127.0.0.1:5175", "https://127.0.0.1:5175",
                "http://127.0.0.1:5176", "https://127.0.0.1:5176",
                "http://192.168.1.101:5173", "https://192.168.1.101:5173",
                "http://192.168.1.101:5174", "https://192.168.1.101:5174",
                "http://192.168.1.101:5175", "https://192.168.1.101:5175",
                "http://192.168.1.101:5176", "https://192.168.1.101:5176"
            )
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

var app = builder.Build();

app.UseSerilogRequestLogging();

app.UseExceptionHandler();
app.UseRouting();
app.UseCors("AllowFrontend");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    /*app.MapScalarApiReference(options =>
    {
        options.WithTitle("ICMS API");
        options.WithTheme(ScalarTheme.Default);
        options.WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
    });*/
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

app.UseHangfireDashboard("/hangfire");

using (var scope = app.Services.CreateScope())
{
    var recurringJobManager = scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();

    // Register the missed dose tracker to run daily at midnight
    recurringJobManager.AddOrUpdate<ICMS.Application.Interfaces.Services.IMissedDoseTrackerService>(
        "DailyMissedDoseTracker",
        service => service.MarkMissedDosesAsync(default),
        Cron.Daily);

    // Register Health Advisory dispatcher to run daily at 8:00 AM (UTC+3 => 05:00 UTC)
    recurringJobManager.AddOrUpdate<ICMS.Application.Interfaces.Services.IAdvisoryDispatchBackgroundService>(
        "DailyHealthAdvisoryDispatcher",
        service => service.DispatchPendingAdvisoriesAsync(default),
        "0 5 * * *");

    // Register Batch Expiration tracker to run every 5 minutes for testing
    recurringJobManager.AddOrUpdate<ICMS.Application.Interfaces.Services.IBatchExpirationTrackerService>(
        "DailyBatchExpirationTracker",
        service => service.TrackExpiringBatchesAsync(default),
        "*/5 * * * *");
}

// Ensure reports output directory exists
Directory.CreateDirectory(Path.Combine("wwwroot", "reports"));

app.MapControllers();
app.MapHub<ICMS.API.Hubs.ReportHub>("/hubs/reports");

app.Run();

public partial class Program
{
}

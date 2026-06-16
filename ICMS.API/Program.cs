using FluentValidation;
using Hangfire;
using Hangfire.PostgreSql;
using ICMS.API.Handlers;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Http;
using System.Net;
using System.Net.Sockets;
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

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

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

builder.Services.AddAuthorization();


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
var firebaseCredentialsPath = Path.Combine(builder.Environment.ContentRootPath, "wwwroot", "firebase-service-account.json");
if (File.Exists(firebaseCredentialsPath))
{
    try 
    {
        FirebaseApp.Create(new AppOptions
        {
            Credential = GoogleCredential.FromFile(firebaseCredentialsPath),
            HttpClientFactory = new IPv4HttpClientFactory()
        });
        Console.WriteLine(">>> Firebase Admin SDK initialized successfully.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($">>> Firebase initialization failed: {ex.Message}");
    }
}
else
{
    Console.WriteLine($">>> Firebase configuration file NOT FOUND at: {firebaseCredentialsPath}");
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

app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = new[] { new HangfireDashboardAuthorizationFilter() }
});

using (var scope = app.Services.CreateScope())
{
    var recurringJobManager = scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();

    // Register the missed dose tracker to run daily at midnight
    recurringJobManager.AddOrUpdate<ICMS.Application.Interfaces.Services.IMissedDoseTrackerService>(
        "DailyMissedDoseTracker",
        service => service.MarkMissedDosesAsync(default),
        Cron.Daily);

    var settingsService = scope.ServiceProvider.GetRequiredService<ICMS.Application.Interfaces.Services.ISystemSettingService>();
    var advisoryTime = await settingsService.GetValueAsync("Advisory.DailyBroadcastTime", "08:00");
    
    // Parse HH:mm and convert to Cron (assuming server local time is UTC+3)
    string cron = "0 5 * * *"; 
    if (TimeSpan.TryParse(advisoryTime, out var ts))
    {
        var utcTime = ts.Subtract(TimeSpan.FromHours(3));
        if (utcTime < TimeSpan.Zero) utcTime = utcTime.Add(TimeSpan.FromHours(24));
        cron = $"{utcTime.Minutes} {utcTime.Hours} * * *";
    }

    // Register Health Advisory dispatcher to run daily at the configured time
    recurringJobManager.AddOrUpdate<ICMS.Application.Interfaces.Services.IAdvisoryDispatchBackgroundService>(
        "DailyHealthAdvisoryDispatcher",
        service => service.DispatchPendingAdvisoriesAsync(default),
        cron);

    // Register Batch Expiration tracker to run every 5 minutes for testing
    recurringJobManager.AddOrUpdate<ICMS.Application.Interfaces.Services.IBatchExpirationTrackerService>(
        "DailyBatchExpirationTracker",
        service => service.TrackExpiringBatchesAsync(default),
        "*/5 * * * *");

    // Register Field Visit reminder to run daily
    recurringJobManager.AddOrUpdate<ICMS.Application.Interfaces.Services.IFieldVisitReminderService>(
        "DailyFieldVisitReminder",
        service => service.SendScheduledRemindersAsync(default),
        Cron.Daily);

    // Register Field Visit auto closer to run daily
    recurringJobManager.AddOrUpdate<ICMS.Application.Interfaces.Services.IFieldVisitService>(
        "DailyFieldVisitAutoCloser",
        service => service.CloseExpiredVisitsAsync(default),
        Cron.Daily);

    // Run the expired field visits auto closer immediately on startup to ensure old visits are closed
    try
    {
        var fieldVisitService = scope.ServiceProvider.GetRequiredService<ICMS.Application.Interfaces.Services.IFieldVisitService>();
        await fieldVisitService.CloseExpiredVisitsAsync(default);

        // Also clean up any mock/invalid tokens in the DB if real sending is enabled
        var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        var isMock = bool.TryParse(config["Firebase:Mock"], out var mock) && mock;
        if (!isMock)
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ICMS.Infrastructure.Persistence.Data.AppDbContext>();
            var invalidDevices = await dbContext.UserDevices
                .Where(ud => ud.FcmToken.Length < 100 || 
                             ud.FcmToken.ToLower().Contains("mock") || 
                             ud.FcmToken.ToLower().Contains("dummy") || 
                             ud.FcmToken.ToLower().Contains("test") || 
                             ud.FcmToken.ToLower().Contains("fake") || 
                             ud.FcmToken.ToLower().Contains("temp"))
                .ToListAsync();

            if (invalidDevices.Any())
            {
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
                logger.LogInformation("Startup cleanup: Removing {Count} pre-existing invalid/mock FCM tokens from database.", invalidDevices.Count);
                dbContext.UserDevices.RemoveRange(invalidDevices);
                await dbContext.SaveChangesAsync();
            }
        }
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Failed to run CloseExpiredVisitsAsync or startup token cleanup on application startup.");
    }
}

// Ensure reports output directory exists
Directory.CreateDirectory(Path.Combine("wwwroot", "reports"));

app.MapControllers();
app.MapHub<ICMS.API.Hubs.ReportHub>("/hubs/reports");

app.Run();

public partial class Program
{
}

public class IPv4HttpClientFactory : Google.Apis.Http.HttpClientFactory
{
    protected override HttpMessageHandler CreateHandler(CreateHttpClientArgs args)
    {
        var handler = new SocketsHttpHandler
        {
            ConnectCallback = async (context, cancellationToken) =>
            {
                var ips = await System.Net.Dns.GetHostAddressesAsync(context.DnsEndPoint.Host, cancellationToken);
                var ipv4 = ips.FirstOrDefault(ip => ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);
                
                var socket = new System.Net.Sockets.Socket(System.Net.Sockets.SocketType.Stream, System.Net.Sockets.ProtocolType.Tcp);
                try
                {
                    if (ipv4 != null)
                    {
                        await socket.ConnectAsync(new System.Net.IPEndPoint(ipv4, context.DnsEndPoint.Port), cancellationToken);
                    }
                    else
                    {
                        await socket.ConnectAsync(context.DnsEndPoint, cancellationToken);
                    }
                    return new System.Net.Sockets.NetworkStream(socket, ownsSocket: true);
                }
                catch
                {
                    socket.Dispose();
                    throw;
                }
            }
        };
        return handler;
    }
}

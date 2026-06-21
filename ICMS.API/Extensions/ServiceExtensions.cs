using Hangfire;
using Hangfire.PostgreSql;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Http;
using System.Net;
using System.Net.Sockets;
using System.Text;
using ICMS.Application.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;
using Microsoft.EntityFrameworkCore;
using ICMS.Application.Interfaces.Services;
using ICMS.Infrastructure.Persistence.Data;

namespace ICMS.API.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddHangfireServices(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddHangfire(config => config
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UsePostgreSqlStorage(c =>
                    c.UseNpgsqlConnection(configuration.GetConnectionString("DefaultConnection"))));

            services.AddHangfireServer();
            return services;
        }

        public static IServiceCollection AddApiAuthentication(this IServiceCollection services,
            IConfiguration configuration)
        {
            var jwtOptions = configuration.GetSection("JwtOptions").Get<JwtOptions>() ??
                             throw new InvalidOperationException("JwtOptions are missing");
            services.AddSingleton(jwtOptions);

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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

            services.AddAuthorization();
            return services;
        }

        public static IServiceCollection AddApiRateLimiter(this IServiceCollection services)
        {
            services.AddRateLimiter(options =>
            {
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

                options.OnRejected = async (context, _) =>
                {
                    context.HttpContext.Response.Headers.RetryAfter = "10";
                    await Task.CompletedTask;
                };

                options.AddFixedWindowLimiter("fixed", opt =>
                {
                    opt.Window = TimeSpan.FromMinutes(1);
                    opt.PermitLimit = 300;
                    opt.QueueLimit = 5;
                    opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                });

                options.AddFixedWindowLimiter("stricter", opt =>
                {
                    opt.Window = TimeSpan.FromMinutes(1);
                    opt.PermitLimit = 20;
                    opt.QueueLimit = 2;
                    opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                });

                options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: context.Connection.RemoteIpAddress?.ToString() ??
                                      context.Request.Headers.Host.ToString(),
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 300,
                            Window = TimeSpan.FromMinutes(1),
                            QueueLimit = 0
                        }));
            });

            return services;
        }

        public static IServiceCollection AddApiCors(this IServiceCollection services)
        {
            services.AddCors(options =>
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

            return services;
        }

        public static WebApplication InitializeFirebase(this WebApplication app)
        {
            var firebaseCredentialsPath =
                Path.Combine(app.Environment.ContentRootPath, "wwwroot", "firebase-service-account.json");
            if (File.Exists(firebaseCredentialsPath))
            {
                try
                {
                    FirebaseApp.Create(new AppOptions
                    {
                        Credential = CredentialFactory.FromFile<ServiceAccountCredential>(firebaseCredentialsPath)
                            .ToGoogleCredential(),
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

            return app;
        }

        public static async Task RegisterRecurringJobsAndStartupTasksAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var recurringJobManager = scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();

            recurringJobManager.AddOrUpdate<IMissedDoseTrackerService>(
                "DailyMissedDoseTracker",
                service => service.MarkMissedDosesAsync(CancellationToken.None),
                Cron.Daily);

            var settingsService = scope.ServiceProvider.GetRequiredService<ISystemSettingService>();
            var advisoryTime = await settingsService.GetValueAsync("Advisory.DailyBroadcastTime", "08:00");

            string cron = "0 5 * * *";
            if (TimeSpan.TryParse(advisoryTime, out var ts))
            {
                var utcTime = ts.Subtract(TimeSpan.FromHours(3));
                if (utcTime < TimeSpan.Zero) utcTime = utcTime.Add(TimeSpan.FromHours(24));
                cron = $"{utcTime.Minutes} {utcTime.Hours} * * *";
            }

            recurringJobManager.AddOrUpdate<IAdvisoryDispatchBackgroundService>(
                "DailyHealthAdvisoryDispatcher",
                service => service.DispatchPendingAdvisoriesAsync(CancellationToken.None),
                cron);

            recurringJobManager.AddOrUpdate<IBatchExpirationTrackerService>(
                "DailyBatchExpirationTracker",
                service => service.TrackExpiringBatchesAsync(CancellationToken.None),
                "*/5 * * * *");

            recurringJobManager.AddOrUpdate<IFieldVisitReminderService>(
                "DailyFieldVisitReminder",
                service => service.SendScheduledRemindersAsync(CancellationToken.None),
                "25 17 * * *"); // 17:25 UTC = 8:25 PM local time (UTC+3)

            recurringJobManager.AddOrUpdate<IFieldVisitService>(
                "DailyFieldVisitAutoCloser",
                service => service.CloseExpiredVisitsAsync(CancellationToken.None),
                Cron.Daily);

            try
            {
                var fieldVisitService = scope.ServiceProvider.GetRequiredService<IFieldVisitService>();
                await fieldVisitService.CloseExpiredVisitsAsync();

                var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
                var isMock = bool.TryParse(config["Firebase:Mock"], out var mock) && mock;
                if (!isMock)
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
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
                        logger.LogInformation(
                            "Startup cleanup: Removing {Count} pre-existing invalid/mock FCM tokens from database.",
                            invalidDevices.Count);
                        dbContext.UserDevices.RemoveRange(invalidDevices);
                        await dbContext.SaveChangesAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex,
                    "Failed to run CloseExpiredVisitsAsync or startup token cleanup on application startup.");
            }
        }
    }

    public class IPv4HttpClientFactory : HttpClientFactory
    {
        protected override HttpMessageHandler CreateHandler(CreateHttpClientArgs args)
        {
            var handler = new SocketsHttpHandler
            {
                ConnectCallback = async (context, cancellationToken) =>
                {
                    var ips = await Dns.GetHostAddressesAsync(context.DnsEndPoint.Host, cancellationToken);
                    var ipv4 = ips.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);

                    var socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
                    try
                    {
                        if (ipv4 != null)
                        {
                            await socket.ConnectAsync(new IPEndPoint(ipv4, context.DnsEndPoint.Port),
                                cancellationToken);
                        }
                        else
                        {
                            await socket.ConnectAsync(context.DnsEndPoint, cancellationToken);
                        }

                        return new NetworkStream(socket, ownsSocket: true);
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
}

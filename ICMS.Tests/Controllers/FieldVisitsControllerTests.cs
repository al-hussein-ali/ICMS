using FluentAssertions;
using ICMS.Tests.Infrastructure;
using System.Net;
using ICMS.Application.DTOs.FieldVisit;
using ICMS.Application.DTOs.Auth;
using ICMS.Domain.ValueObjects;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using ICMS.Infrastructure.Persistence.Data;
using ICMS.Domain.Entites.Identity;
using ICMS.Domain.Entites.Clinical;
using ICMS.Domain.Entites.Visits;
using ICMS.Domain.Enums;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace ICMS.Tests.Controllers
{
    public class FieldVisitsControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory<Program> _factory;
        private readonly JsonSerializerOptions _jsonOptions;

        public FieldVisitsControllerTests(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
            _jsonOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);
            _jsonOptions.Converters.Add(new JsonStringEnumConverter());
        }

        private async Task AuthenticateAsync()
        {
            _client.DefaultRequestHeaders.Authorization = null;
            var loginDto = new LoginDto("admin", "Admin@123");
            var loginRes = await _client.PostAsJsonAsync("/api/auth/login", loginDto);
            loginRes.StatusCode.Should().Be(HttpStatusCode.OK);
            var authResult = await loginRes.Content.ReadFromJsonAsync<AuthResponseDto>(_jsonOptions);
            _client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authResult.AccessToken);
        }

        [Fact]
        public async Task GetAllFieldVisits_FallbackTargetedCount_ReturnsCorrectCount()
        {
            // Arrange
            int subNeighborhoodId;
            int dirId;
            int neighborhoodId;
            Dose dose;

            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                var subNeigh = db.SubNeighborhoods.First();
                subNeighborhoodId = subNeigh.Id;
                neighborhoodId = subNeigh.NeighborhoodId;
                dirId = db.Neighborhoods.First(n => n.Id == neighborhoodId).DirectorateId;
                dose = db.Doses.First(); // BCG, MaxEligibleAgeInMonths = 12

                // Create a person born 6 months ago (eligible for BCG)
                var dob = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(-6));
                var person = Person.Create("FallbackTarget", "Child", "Test", "ICMS", Gender.Male, dob, "9998887776");
                db.People.Add(person);
                db.SaveChanges();

                var individual = VaccinatedIndividual.Create(dirId, neighborhoodId, subNeighborhoodId, registrationDate: dob);
                individual.AssignPerson(person);
                individual.ScheduleInitialVaccines(new List<Dose> { dose }, person.DateOfBirth);

                var schedule = individual.Schedules.First();
                schedule.MarkAsMissed(); // Make sure it's missed

                db.VaccinatedIndividuals.Add(individual);
                db.SaveChanges();
            }

            await AuthenticateAsync();

            // Create Field Visit DTO without explicitly selected individuals
            var createDto = new FieldVisitCreateDto(
                CampaignName: "Test Campaign Fallback",
                VisitDate: DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
                SubNeighborhoodId: subNeighborhoodId,
                FromDate: DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(-12)),
                ToDate: DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7)),
                SelectedIndividualIds: new List<int>(), // Empty!
                SelectedWorkerIds: new List<int>()
            );

            var createResponse = await _client.PostAsJsonAsync("/api/field-visits/create", createDto);
            createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

            // Act: Query field visits list
            var listResponse = await _client.GetAsync("/api/field-visits?PageNumber=1&PageSize=50");
            listResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var list = await listResponse.Content.ReadFromJsonAsync<PagedResult<FieldVisitReadDto>>(_jsonOptions);
            list.Should().NotBeNull();

            // Assert
            var addedVisit = list.Items.FirstOrDefault(v => v.CampaignName == "Test Campaign Fallback");
            addedVisit.Should().NotBeNull();
            addedVisit.TargetedCount.Should().Be(1); // Should fall back to the 1 missed schedule individual in that sub-neighborhood
        }

        [Fact]
        public async Task SendReminders_ShouldDeliverNotifications_And_MarkReminderSent()
        {
            // Arrange
            int subNeighborhoodId;
            int dirId;
            int neighborhoodId;
            Dose dose;
            int individualId;
            int userId;
            int visitId;

            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                var subNeigh = db.SubNeighborhoods.First();
                subNeighborhoodId = subNeigh.Id;
                neighborhoodId = subNeigh.NeighborhoodId;
                dirId = db.Neighborhoods.First(n => n.Id == neighborhoodId).DirectorateId;
                dose = db.Doses.First();

                var dob = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(-6));
                var person = Person.Create("ReminderTarget", "Child", "Test", "ICMS", Gender.Male, dob, "9998887771");
                db.People.Add(person);
                db.SaveChanges();

                var user = User.Create("remindertargetuser", "Target@123", person.Id);
                db.Users.Add(user);
                db.SaveChanges();

                user.AddDevice("fcm-token-test-12345");
                db.SaveChanges();

                userId = user.Id;

                var individual = VaccinatedIndividual.Create(dirId, neighborhoodId, subNeighborhoodId, userId: userId, registrationDate: dob);
                individual.AssignPerson(person);
                individual.ScheduleInitialVaccines(new List<Dose> { dose }, person.DateOfBirth);

                var schedule = individual.Schedules.First();
                schedule.MarkAsMissed();

                db.VaccinatedIndividuals.Add(individual);
                db.SaveChanges();

                individualId = individual.Id;

                var visit = FieldVisit.Create(
                    "Test Campaign Reminders",
                    DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
                    subNeighborhoodId,
                    DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(-12)),
                    DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7))
                );
                visit.AddIndividual(individualId);

                db.FieldVisits.Add(visit);
                db.SaveChanges();

                visitId = visit.Id;
            }

            await AuthenticateAsync();

            // Act
            var response = await _client.PostAsync($"/api/field-visits/{visitId}/send-reminders", null);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var updatedVisit = db.FieldVisits.Find(visitId);
                updatedVisit.Should().NotBeNull();
                updatedVisit!.ReminderSent.Should().BeTrue();
            }
        }

        [Fact]
        public async Task CreateFieldVisit_WithWorkers_SucceedsAndLinksWorkers()
        {
            // Arrange
            int subNeighborhoodId;
            int workerUserId;

            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                var subNeigh = db.SubNeighborhoods.First();
                subNeighborhoodId = subNeigh.Id;

                var dob = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-25));
                var person = Person.Create("Worker", "Test", "User", "ICMS", Gender.Male, dob, "9998881234");
                db.People.Add(person);
                db.SaveChanges();

                var user = User.Create("workeruser1", "Worker@123", person.Id);
                db.Users.Add(user);
                db.SaveChanges();

                user.AddDevice("worker-fcm-token-555");
                db.SaveChanges();

                workerUserId = user.Id;
            }

            await AuthenticateAsync();

            var createDto = new FieldVisitCreateDto(
                CampaignName: "Worker Campaign Notification Test",
                VisitDate: DateOnly.FromDateTime(DateTime.UtcNow.AddDays(2)),
                SubNeighborhoodId: subNeighborhoodId,
                FromDate: DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)),
                ToDate: DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5)),
                SelectedIndividualIds: new List<int>(),
                SelectedWorkerIds: new List<int> { workerUserId }
            );

            // Act
            var createResponse = await _client.PostAsJsonAsync("/api/field-visits/create", createDto);

            // Assert
            createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var createdVisit = db.FieldVisits
                    .Include(fv => fv.FieldVisitWorkers)
                    .FirstOrDefault(fv => fv.CampaignName == "Worker Campaign Notification Test");

                createdVisit.Should().NotBeNull();
                createdVisit!.FieldVisitWorkers.Should().ContainSingle(w => w.UserId == workerUserId);
            }
        }

        [Fact]
        public async Task CloseExpiredVisits_OnlyClosesVisitsExpiredMoreThan3Days()
        {
            // Arrange
            int subNeighborhoodId;
            int expiredId;
            int notExpiredId;
            int alreadyCompletedId;

            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                var subNeigh = db.SubNeighborhoods.First();
                subNeighborhoodId = subNeigh.Id;

                // 1. Uncompleted, expired 4 days ago (should be closed)
                var expiredVisit = FieldVisit.Create(
                    "Expired Campaign 4 Days",
                    DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-5)),
                    subNeighborhoodId,
                    DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-10)),
                    DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-4))
                );
                db.FieldVisits.Add(expiredVisit);

                // 2. Uncompleted, expired 2 days ago (should not be closed)
                var notExpiredVisit = FieldVisit.Create(
                    "Expired Campaign 2 Days",
                    DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-3)),
                    subNeighborhoodId,
                    DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-5)),
                    DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-2))
                );
                db.FieldVisits.Add(notExpiredVisit);

                // 3. Completed, expired 4 days ago (should remain unchanged)
                var alreadyCompletedVisit = FieldVisit.Create(
                    "Completed Campaign 4 Days",
                    DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-5)),
                    subNeighborhoodId,
                    DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-10)),
                    DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-4))
                );
                alreadyCompletedVisit.MarkCompleted();
                db.FieldVisits.Add(alreadyCompletedVisit);

                db.SaveChanges();

                expiredId = expiredVisit.Id;
                notExpiredId = notExpiredVisit.Id;
                alreadyCompletedId = alreadyCompletedVisit.Id;
            }

            // Act
            using (var scope = _factory.Services.CreateScope())
            {
                var service = scope.ServiceProvider.GetRequiredService<ICMS.Application.Interfaces.Services.IFieldVisitService>();
                var closedCount = await service.CloseExpiredVisitsAsync(default);
                closedCount.Should().BeGreaterThan(0);
            }

            // Assert
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                var expiredVisit = db.FieldVisits.Find(expiredId);
                expiredVisit.Should().NotBeNull();
                expiredVisit!.IsCompleted.Should().BeTrue();

                var notExpiredVisit = db.FieldVisits.Find(notExpiredId);
                notExpiredVisit.Should().NotBeNull();
                notExpiredVisit!.IsCompleted.Should().BeFalse();

                var alreadyCompletedVisit = db.FieldVisits.Find(alreadyCompletedId);
                alreadyCompletedVisit.Should().NotBeNull();
                alreadyCompletedVisit!.IsCompleted.Should().BeTrue();
            }
        }

        [Fact]
        public async Task GetFieldVisitById_WorkerPartitioning_DividesDelayedPeopleCorrectly()
        {
            // Arrange
            int subNeighborhoodId;
            int dirId;
            int neighborhoodId;
            Dose dose;
            int worker1UserId;
            int worker2UserId;
            int visitId;
            List<int> individualIds = new();

            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                var subNeigh = db.SubNeighborhoods.First();
                subNeighborhoodId = subNeigh.Id;
                neighborhoodId = subNeigh.NeighborhoodId;
                dirId = db.Neighborhoods.First(n => n.Id == neighborhoodId).DirectorateId;
                dose = db.Doses.First();

                // Create 5 people
                for (int i = 1; i <= 5; i++)
                {
                    var dob = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(-6));
                    var person = Person.Create($"Child{i}", "Test", "User", "ICMS", Gender.Male, dob, $"999000111{i}");
                    db.People.Add(person);
                    db.SaveChanges();

                    var individual = VaccinatedIndividual.Create(dirId, neighborhoodId, subNeighborhoodId, registrationDate: dob);
                    individual.AssignPerson(person);
                    individual.ScheduleInitialVaccines(new List<Dose> { dose }, person.DateOfBirth);
                    individual.Schedules.First().MarkAsMissed();
                    db.VaccinatedIndividuals.Add(individual);
                    db.SaveChanges();

                    individualIds.Add(individual.Id);
                }

                // Create 2 workers
                var worker1Person = Person.Create("WorkerOne", "Test", "User", "ICMS", Gender.Male, DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-25)), "9991112221");
                db.People.Add(worker1Person);
                var worker2Person = Person.Create("WorkerTwo", "Test", "User", "ICMS", Gender.Male, DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-25)), "9991112222");
                db.People.Add(worker2Person);
                db.SaveChanges();

                var passwordHash = BCrypt.Net.BCrypt.HashPassword("Worker@123");
                var worker1User = User.Create("workeroneuser", passwordHash, worker1Person.Id);
                db.Users.Add(worker1User);
                var worker2User = User.Create("workertwouser", passwordHash, worker2Person.Id);
                db.Users.Add(worker2User);
                db.SaveChanges();

                var workerRole = db.Roles.First(r => r.RoleName == ICMS.Domain.Constants.Roles.FieldVisitWorker);
                db.UserRoles.Add(UserRole.Create(worker1User.Id, workerRole.Id));
                db.UserRoles.Add(UserRole.Create(worker2User.Id, workerRole.Id));
                db.SaveChanges();

                worker1UserId = worker1User.Id;
                worker2UserId = worker2User.Id;

                var visit = FieldVisit.Create(
                    "Partitioning Campaign Test",
                    DateOnly.FromDateTime(DateTime.UtcNow.AddDays(2)),
                    subNeighborhoodId,
                    DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)),
                    DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5))
                );

                // Add individuals sorted to match deterministic partition ordering
                foreach (var indId in individualIds.OrderBy(id => id))
                {
                    visit.AddIndividual(indId);
                }

                // Add workers sorted
                var sortedWorkerIds = new List<int> { worker1UserId, worker2UserId }.OrderBy(id => id).ToList();
                visit.AddWorker(sortedWorkerIds[0]);
                visit.AddWorker(sortedWorkerIds[1]);

                db.FieldVisits.Add(visit);
                db.SaveChanges();

                visitId = visit.Id;
            }

            async Task AuthenticateAsUserAsync(string username, string password)
            {
                _client.DefaultRequestHeaders.Authorization = null;
                var loginDto = new LoginDto(username, password);
                var loginRes = await _client.PostAsJsonAsync("/api/auth/login", loginDto);
                loginRes.StatusCode.Should().Be(HttpStatusCode.OK);
                var authResult = await loginRes.Content.ReadFromJsonAsync<AuthResponseDto>(_jsonOptions);
                _client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authResult.AccessToken);
            }

            // Let's authenticate as Worker 1 (first worker in sorted list)
            var sortedWorkers = new List<(string Username, int Id)> 
            { 
                ("workeroneuser", worker1UserId), 
                ("workertwouser", worker2UserId) 
            }.OrderBy(w => w.Id).ToList();

            // Act & Assert for Worker 1
            await AuthenticateAsUserAsync(sortedWorkers[0].Username, "Worker@123");
            var response1 = await _client.GetAsync($"/api/field-visits/{visitId}");
            response1.StatusCode.Should().Be(HttpStatusCode.OK);
            var details1 = await response1.Content.ReadFromJsonAsync<FieldVisitDetailsDto>(_jsonOptions);
            details1.Should().NotBeNull();
            
            // 5 people / 2 workers = 2 base. Remainder = 1.
            // First worker (index 0) gets 2 + 1 = 3 people.
            details1!.SelectedIndividuals.Should().HaveCount(3);

            // Act & Assert for Worker 2
            await AuthenticateAsUserAsync(sortedWorkers[1].Username, "Worker@123");
            var response2 = await _client.GetAsync($"/api/field-visits/{visitId}");
            response2.StatusCode.Should().Be(HttpStatusCode.OK);
            var details2 = await response2.Content.ReadFromJsonAsync<FieldVisitDetailsDto>(_jsonOptions);
            details2.Should().NotBeNull();
            
            // Second worker (index 1) gets 2 people.
            details2!.SelectedIndividuals.Should().HaveCount(2);

            // Now let's test toggle-going:
            // Admin sets Worker 1 as not going.
            await AuthenticateAsync(); // Admin
            var toggleResponse = await _client.PutAsync($"/api/field-visits/{visitId}/workers/{sortedWorkers[0].Id}/toggle-going", null);
            toggleResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

            // Now log in as Worker 2 again. Since Worker 1 is not going, Worker 2 is the ONLY active worker!
            // All 5 people should shift to Worker 2.
            await AuthenticateAsUserAsync(sortedWorkers[1].Username, "Worker@123");
            var response2AfterShift = await _client.GetAsync($"/api/field-visits/{visitId}");
            response2AfterShift.StatusCode.Should().Be(HttpStatusCode.OK);
            var details2AfterShift = await response2AfterShift.Content.ReadFromJsonAsync<FieldVisitDetailsDto>(_jsonOptions);
            details2AfterShift.Should().NotBeNull();
            details2AfterShift!.SelectedIndividuals.Should().HaveCount(5);

            // Worker 1 is marked as not going, so if they load, they should get 0 people.
            await AuthenticateAsUserAsync(sortedWorkers[0].Username, "Worker@123");
            var response1AfterShift = await _client.GetAsync($"/api/field-visits/{visitId}");
            response1AfterShift.StatusCode.Should().Be(HttpStatusCode.OK);
            var details1AfterShift = await response1AfterShift.Content.ReadFromJsonAsync<FieldVisitDetailsDto>(_jsonOptions);
            details1AfterShift.Should().NotBeNull();
            details1AfterShift!.SelectedIndividuals.Should().HaveCount(0);
        }
    }
}

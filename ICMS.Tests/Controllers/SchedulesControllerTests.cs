using FluentAssertions;
using ICMS.Tests.Infrastructure;
using System.Net;
using ICMS.Application.DTOs.Schedules;
using ICMS.Application.DTOs.VaccinatedIndividual;
using ICMS.Application.DTOs.Auth;
using ICMS.Domain.ValueObjects;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using ICMS.Infrastructure.Persistence.Data;
using ICMS.Domain.Entites.Identity;
using ICMS.Domain.Entites.Clinical;
using ICMS.Domain.Enums;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ICMS.Tests.Controllers
{
    public class SchedulesControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory<Program> _factory;
        private readonly JsonSerializerOptions _jsonOptions;

        public SchedulesControllerTests(CustomWebApplicationFactory<Program> factory)
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
        public async Task GetIndividualSchedules_ExistingIndividual_ReturnsOk()
        {
            // Arrange
            await AuthenticateAsync();
            var listResponse = await _client.GetAsync("/api/vaccinated-individuals?PageNumber=1&PageSize=1");
            listResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var list =
                await listResponse.Content.ReadFromJsonAsync<PagedResult<VaccinatedIndividualReadDto>>(_jsonOptions);
            var testId = list.Items[0].Id;

            // Act
            var response = await _client.GetAsync($"/api/schedules/individual/{testId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var schedules = await response.Content.ReadFromJsonAsync<IEnumerable<ScheduleReadDto>>(_jsonOptions);
            schedules.Should().NotBeNull();
        }

        [Fact]
        public async Task GetIndividualSchedules_NonExistingIndividual_ReturnsNotFound()
        {
            // Arrange
            await AuthenticateAsync();

            // Act
            var response = await _client.GetAsync("/api/schedules/individual/999");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GetMissedSchedules_AgeRestriction_FiltersOutTooOld()
        {
            // Arrange
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                var dir = db.Directorates.First();
                var neighborhood = db.Neighborhoods.First();
                var dose = db.Doses.First(); // BCG, MaxEligibleAgeInMonths = 12

                // 1. Create a child born on 2024-01-01 (who will be 13 months old on 2025-02-01, hence too old)
                var oldPerson = Person.Create("TooOldAge", "Child", "One", "ICMS", Gender.Male,
                    new DateOnly(2024, 1, 1), "1234567890");
                db.People.Add(oldPerson);
                db.SaveChanges();

                var oldIndividual = VaccinatedIndividual.Create(dir.Id, neighborhood.Id,
                    registrationDate: new DateOnly(2024, 1, 1));
                oldIndividual.AssignPerson(oldPerson);
                oldIndividual.ScheduleInitialVaccines(new List<Dose> { dose }, oldPerson.DateOfBirth);

                var oldSchedule = oldIndividual.Schedules.First();
                oldSchedule.MarkAsMissed(); // Mark as Missed

                db.VaccinatedIndividuals.Add(oldIndividual);

                // 2. Create a child born on 2024-09-01 (who will be 5 months old on 2025-02-01, hence eligible)
                var youngPerson = Person.Create("YoungEligible", "Child", "Two", "ICMS", Gender.Male,
                    new DateOnly(2024, 9, 1), "0987654321");
                db.People.Add(youngPerson);
                db.SaveChanges();

                var youngIndividual = VaccinatedIndividual.Create(dir.Id, neighborhood.Id,
                    registrationDate: new DateOnly(2024, 9, 1));
                youngIndividual.AssignPerson(youngPerson);
                youngIndividual.ScheduleInitialVaccines(new List<Dose> { dose }, youngPerson.DateOfBirth);

                var youngSchedule = youngIndividual.Schedules.First();
                youngSchedule.MarkAsMissed(); // Mark as Missed

                db.VaccinatedIndividuals.Add(youngIndividual);
                db.SaveChanges();
            }

            await AuthenticateAsync();

            // Act: Query missed schedules on 2025-02-01
            var fromDate = "2020-01-01";
            var toDate = "2025-02-01";
            var response = await _client.GetAsync($"/api/schedules/missed?fromDate={fromDate}&toDate={toDate}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var missedSchedules = await response.Content.ReadFromJsonAsync<List<MissedScheduleReadDto>>(_jsonOptions);
            missedSchedules.Should().NotBeNull();

            // The young individual should be returned, but the old one should be filtered out
            var testSchedules = missedSchedules.Where(s => s.FirstName == "YoungEligible" || s.FirstName == "TooOldAge")
                .ToList();
            testSchedules.Should().ContainSingle();
            testSchedules.First().FirstName.Should().Be("YoungEligible");
        }
    }
}

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
using ICMS.Domain.Enums;
using System.Text.Json;
using System.Text.Json.Serialization;

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
    }
}

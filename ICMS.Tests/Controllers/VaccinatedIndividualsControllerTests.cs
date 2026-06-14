using FluentAssertions;
using ICMS.Tests.Infrastructure;
using System.Net;
using ICMS.Application.DTOs.VaccinatedIndividual;
using ICMS.Application.DTOs.Person;
using System.Net.Http.Json;
using ICMS.Domain.ValueObjects;
using Microsoft.Extensions.DependencyInjection;

namespace ICMS.Tests.Controllers
{
    public class VaccinatedIndividualsControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory<Program> _factory;
        private readonly System.Text.Json.JsonSerializerOptions _jsonOptions;

        public VaccinatedIndividualsControllerTests(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
            _jsonOptions = new System.Text.Json.JsonSerializerOptions(System.Text.Json.JsonSerializerDefaults.Web);
            _jsonOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
        }

        [Fact]
        public async Task GetAll_ReturnsPagedResult()
        {
            // Act
            var response = await _client.GetAsync("/api/vaccinated-individuals?PageNumber=1&PageSize=10");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<PagedResult<VaccinatedIndividualReadDto>>(_jsonOptions);
            result.Should().NotBeNull();
        }

        [Fact]
        public async Task GetById_Existing_ReturnsOk()
        {
            // Arrange
            var listResponse = await _client.GetAsync("/api/vaccinated-individuals?PageNumber=1&PageSize=1");
            var list = await listResponse.Content.ReadFromJsonAsync<PagedResult<VaccinatedIndividualReadDto>>(_jsonOptions);
            var testId = list?.Items?[0]?.Id ?? 0;

            if (testId == 0) return; // Skip if no data

            // Act
            var response = await _client.GetAsync($"/api/vaccinated-individuals/{testId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var individual = await response.Content.ReadFromJsonAsync<VaccinatedIndividualReadDto>(_jsonOptions);
            individual.Should().NotBeNull();
            individual?.Id.Should().Be(testId);
        }

        [Fact]
        public async Task Add_ValidIndividual_ReturnsCreated()
        {
            // Arrange: look up the actual seeded Directorate/Neighborhood IDs from the DB
            // (InitializeDbForTests clears and re-seeds geography, so IDs are auto-incremented
            //  and may not be 1 — we must fetch the real values to avoid a 404.)
            int dirId, neighborhoodId;
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ICMS.Infrastructure.Persistence.Data.AppDbContext>();
                dirId = db.Directorates.First().Id;
                neighborhoodId = db.Neighborhoods.First().Id;
            }

            var personDto = new PersonCreateDto("New", "Patient", "Test", "ICMS", "Female", new DateOnly(2023, 5, 20), "1122334455");
            var dto = new VaccinatedIndividualCreateDto(dirId, neighborhoodId, null, null, personDto, null);

            // Act
            var response = await _client.PostAsJsonAsync("/api/vaccinated-individuals/create", dto);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
        }
    }
}

using FluentAssertions;
using ICMS.Tests.Infrastructure;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using ICMS.Application.DTOs.Vaccine;
using ICMS.Application.DTOs.Dose;
using System.Net.Http.Json;
using System.Collections.Generic;
using System.Linq;

namespace ICMS.Tests.Controllers
{
    public class VaccinesControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public VaccinesControllerTests(CustomWebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetAll_ReturnsOk()
        {
            var response = await _client.GetAsync("/api/vaccines");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task Add_Valid_ReturnsCreated()
        {
            var dto = new VaccineCreateDto("Polio", "OPV-01", "Oral Polio Vaccine", true, 3, 0, 60, ICMS.Domain.Enums.TargetAudience.InfantRoutine);
            var response = await _client.PostAsJsonAsync("/api/vaccines/create", dto);
            response.StatusCode.Should().Be(HttpStatusCode.Created);
        }
    }

    public class DosesControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly System.Text.Json.JsonSerializerOptions _jsonOptions;

        public DosesControllerTests(CustomWebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
            _jsonOptions = new System.Text.Json.JsonSerializerOptions(System.Text.Json.JsonSerializerDefaults.Web);
            _jsonOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
        }

        [Fact]
        public async Task GetAll_ReturnsOk()
        {
            var response = await _client.GetAsync("/api/doses");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task Add_Valid_ReturnsCreated()
        {
            // Vaccine 1 is seeded, but let's fetch it for robustness
            var vaccinesResponse = await _client.GetAsync("/api/vaccines");
            var vaccines = await vaccinesResponse.Content.ReadFromJsonAsync<IEnumerable<VaccineReadDto>>(_jsonOptions);
            var testVaccine = vaccines.First();

            var dto = new DoseCreateDto(testVaccine.Id, "New Test Dose", 2, 8, "2 Months", true, "Maintenance");
            var response = await _client.PostAsJsonAsync("/api/doses/create", dto);
            response.StatusCode.Should().Be(HttpStatusCode.Created);
        }
    }
}

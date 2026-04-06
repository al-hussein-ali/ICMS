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
            var response = await _client.GetAsync("/api/Vaccines");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task Add_Valid_ReturnsCreated()
        {
            var dto = new VaccineCreateDto("Polio", "OPV-01", "Oral Polio Vaccine", true, 3, ICMS.Domain.Enums.TargetAudience.InfantRoutine);
            var response = await _client.PostAsJsonAsync("/api/Vaccines", dto);
            response.StatusCode.Should().Be(HttpStatusCode.Created);
        }
    }

    public class DosesControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public DosesControllerTests(CustomWebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetAll_ReturnsOk()
        {
            var response = await _client.GetAsync("/api/Doses");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task Add_Valid_ReturnsCreated()
        {
            // Vaccine 1 is seeded, but let's fetch it for robustness
            var vaccinesResponse = await _client.GetAsync("/api/Vaccines");
            var vaccines = await vaccinesResponse.Content.ReadFromJsonAsync<IEnumerable<VaccineReadDto>>();
            var testVaccine = vaccines.First();

            var dto = new DoseCreateDto(testVaccine.Id, "New Test Dose", 2, 2, "2 Months", "Maintenance");
            var response = await _client.PostAsJsonAsync("/api/Doses", dto);
            response.StatusCode.Should().Be(HttpStatusCode.Created);
        }
    }
}

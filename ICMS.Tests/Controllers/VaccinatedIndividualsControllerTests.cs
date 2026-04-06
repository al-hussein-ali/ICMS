using FluentAssertions;
using ICMS.Tests.Infrastructure;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using ICMS.Application.DTOs.VaccinatedIndividual;
using ICMS.Application.DTOs.Person;
using System.Net.Http.Json;
using System;
using ICMS.Application.DTOs.Pagination;
using ICMS.Domain.ValueObjects;

namespace ICMS.Tests.Controllers
{
    public class VaccinatedIndividualsControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public VaccinatedIndividualsControllerTests(CustomWebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetAll_ReturnsPagedResult()
        {
            // Act
            var response = await _client.GetAsync("/api/VaccinatedIndividuals?PageNumber=1&PageSize=10");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<PagedResult<VaccinatedIndividualReadDto>>();
            result.Should().NotBeNull();
        }

        [Fact]
        public async Task GetById_Existing_ReturnsOk()
        {
            // Arrange
            var listResponse = await _client.GetAsync("/api/VaccinatedIndividuals?PageNumber=1&PageSize=1");
            var list = await listResponse.Content.ReadFromJsonAsync<PagedResult<VaccinatedIndividualReadDto>>();
            var testId = list.Items[0].Id;

            // Act
            var response = await _client.GetAsync($"/api/VaccinatedIndividuals/byId/{testId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var individual = await response.Content.ReadFromJsonAsync<VaccinatedIndividualReadDto>();
            individual.Should().NotBeNull();
            individual.Id.Should().Be(testId);
        }

        [Fact]
        public async Task Add_ValidIndividual_ReturnsCreated()
        {
            // Arrange
            var personDto = new PersonCreateDto("New", "Patient", "Test", "ICMS", "Female", new DateOnly(2023, 5, 20), "1122334455");
            var dto = new VaccinatedIndividualCreateDto(1, 1, null, null, personDto, null);

            // Act
            var response = await _client.PostAsJsonAsync("/api/VaccinatedIndividuals", dto);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
        }
    }
}

using FluentAssertions;
using ICMS.Tests.Infrastructure;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using ICMS.Application.DTOs.Pagination;
using ICMS.Domain.ValueObjects;
using ICMS.Application.DTOs.Person;
using ICMS.Application.DTOs.ImmunizationRecord;
using System.Net.Http.Json;
using System;

namespace ICMS.Tests.Controllers
{
    public class PeopleControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public PeopleControllerTests(CustomWebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetAll_ReturnsOk()
        {
            var response = await _client.GetAsync("/api/People");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task GetById_Existing_ReturnsOk()
        {
            // Arrange
            var listResponse = await _client.GetAsync("/api/People?PageNumber=1&PageSize=1");
            var list = await listResponse.Content.ReadFromJsonAsync<PagedResult<PersonReadDto>>();
            var testId = list.Items[0].Id;

            // Act
            var response = await _client.GetAsync($"/api/People/{testId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var person = await response.Content.ReadFromJsonAsync<PersonReadDto>();
            person.Should().NotBeNull();
            person.Id.Should().Be(testId);
        }

        [Fact]
        public async Task Add_Valid_ReturnsCreated()
        {
            var dto = new PersonCreateDto("John", "Middle", "Doe", "ICMS", "Male", new DateOnly(1990, 1, 1), "9988776655");
            var response = await _client.PostAsJsonAsync("/api/People", dto);
            response.StatusCode.Should().Be(HttpStatusCode.Created);
        }
    }

    public class ImmunizationRecordsControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public ImmunizationRecordsControllerTests(CustomWebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetAll_ReturnsOk()
        {
            var response = await _client.GetAsync("/api/ImmunizationRecords");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}

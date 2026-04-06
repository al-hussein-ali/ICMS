using FluentAssertions;
using ICMS.Tests.Infrastructure;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using System.Collections.Generic;
using ICMS.Application.DTOs.Schedules;
using ICMS.Application.DTOs.VaccinatedIndividual;
using ICMS.Domain.ValueObjects;
using System.Net.Http.Json;
using System;
using System.Linq;

namespace ICMS.Tests.Controllers
{
    public class SchedulesControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public SchedulesControllerTests(CustomWebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetCitizenSchedules_ExistingIndividual_ReturnsOk()
        {
            // Arrange
            var listResponse = await _client.GetAsync("/api/VaccinatedIndividuals?PageNumber=1&PageSize=1");
            var list = await listResponse.Content.ReadFromJsonAsync<PagedResult<VaccinatedIndividualReadDto>>();
            var testId = list.Items[0].Id;

            // Act
            var response = await _client.GetAsync($"/api/Schedules/citizen/{testId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var schedules = await response.Content.ReadFromJsonAsync<IEnumerable<ScheduleReadDto>>();
            schedules.Should().NotBeNull();
        }

        [Fact]
        public async Task GetCitizenSchedules_NonExistingIndividual_ReturnsNotFound()
        {
            // Act
            var response = await _client.GetAsync("/api/schedules/citizen/999");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}

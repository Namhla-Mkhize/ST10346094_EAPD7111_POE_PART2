using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Xunit;

namespace TechMove.Tests
{
    public class ApiIntegrationTests
    {
        private readonly HttpClient _client;
        private readonly string _baseUrl = "http://localhost:5228";
        private string? _token;

        public ApiIntegrationTests()
        {
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (_, _, _, _) => true
            };
            _client = new HttpClient(handler)
            {
                BaseAddress = new Uri("http://localhost:5228")
            };
        }

        private async Task AuthenticateAsync()
        {
            var loginData = new { username = "admin", password = "admin123" };
            var content = new StringContent(
                JsonSerializer.Serialize(loginData),
                Encoding.UTF8,
                "application/json");

            var response = await _client.PostAsync("/api/auth/login", content);
            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<JsonElement>(json);
            _token = result.GetProperty("token").GetString();
            _client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);
        }

        [Fact]
        public async Task Login_ValidCredentials_ReturnsToken()
        {
            // Arrange
            var loginData = new { username = "admin", password = "admin123" };
            var content = new StringContent(
                JsonSerializer.Serialize(loginData),
                Encoding.UTF8,
                "application/json");

            // Act
            var response = await _client.PostAsync("/api/auth/login", content);
            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<JsonElement>(json);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            result.GetProperty("token").GetString().Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task Login_InvalidCredentials_ReturnsUnauthorized()
        {
            // Arrange
            var loginData = new { username = "wrong", password = "wrong" };
            var content = new StringContent(
                JsonSerializer.Serialize(loginData),
                Encoding.UTF8,
                "application/json");

            // Act
            var response = await _client.PostAsync("/api/auth/login", content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task GetContracts_Authenticated_ReturnsOk()
        {
            // Arrange
            await AuthenticateAsync();

            // Act
            var response = await _client.GetAsync("/api/contracts");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var json = await response.Content.ReadAsStringAsync();
            json.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task GetContracts_NotAuthenticated_ReturnsUnauthorized()
        {
            // Arrange - no auth header
            _client.DefaultRequestHeaders.Authorization = null;

            // Act
            var response = await _client.GetAsync("/api/contracts");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task GetClients_Authenticated_ReturnsOk()
        {
            // Arrange
            await AuthenticateAsync();

            // Act
            var response = await _client.GetAsync("/api/clients");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var json = await response.Content.ReadAsStringAsync();
            json.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task CreateClient_ValidData_ReturnsCreated()
        {
            // Arrange
            await AuthenticateAsync();
            var client = new
            {
                name = "Integration Test Client",
                contactDetails = "test@test.com | 011 000 0000",
                region = "Gauteng"
            };
            var content = new StringContent(
                JsonSerializer.Serialize(client),
                Encoding.UTF8,
                "application/json");

            // Act
            var response = await _client.PostAsync("/api/clients", content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
        }

        [Fact]
        public async Task GetServiceRequests_Authenticated_ReturnsOk()
        {
            // Arrange
            await AuthenticateAsync();

            // Act
            var response = await _client.GetAsync("/api/servicerequests");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var json = await response.Content.ReadAsStringAsync();
            json.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task GetExchangeRate_Authenticated_ReturnsRate()
        {
            // Arrange
            await AuthenticateAsync();

            // Act
            var response = await _client.GetAsync("/api/servicerequests/exchangerate");
            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<JsonElement>(json);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            result.GetProperty("rate").GetDecimal().Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task UpdateContractStatus_ValidId_ReturnsOk()
        {
            // Arrange
            await AuthenticateAsync();
            var content = new StringContent("1", Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PatchAsync("/api/contracts/5/status", content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}
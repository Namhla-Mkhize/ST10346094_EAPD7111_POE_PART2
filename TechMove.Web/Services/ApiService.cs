using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using TechMove.Web.Models;

namespace TechMove.Web.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly JsonSerializerOptions _jsonOptions;

        public ApiService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles
            };
        }

        private void AddAuthHeader()
        {
            var token = _httpContextAccessor.HttpContext?.Session.GetString("JwtToken");
            if (!string.IsNullOrEmpty(token))
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        private StringContent ToJson(object obj) =>
            new StringContent(JsonSerializer.Serialize(obj), Encoding.UTF8, "application/json");

        // Auth
        public async Task<string?> LoginAsync(string username, string password)
        {
            var response = await _httpClient.PostAsync("api/auth/login",
                ToJson(new { username, password }));

            if (!response.IsSuccessStatusCode) return null;

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<JsonElement>(json);
            return result.GetProperty("token").GetString();
        }

        // Clients
        public async Task<List<Client>> GetClientsAsync()
        {
            AddAuthHeader();
            var response = await _httpClient.GetAsync("api/clients");
            if (!response.IsSuccessStatusCode) return new List<Client>();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<Client>>(json, _jsonOptions) ?? new List<Client>();
        }

        public async Task<Client?> GetClientAsync(int id)
        {
            AddAuthHeader();
            var response = await _httpClient.GetAsync($"api/clients/{id}");
            if (!response.IsSuccessStatusCode) return null;
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Client>(json, _jsonOptions);
        }

        public async Task<bool> CreateClientAsync(Client client)
        {
            AddAuthHeader();
            var response = await _httpClient.PostAsync("api/clients", ToJson(client));
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateClientAsync(Client client)
        {
            AddAuthHeader();
            var response = await _httpClient.PutAsync($"api/clients/{client.Id}", ToJson(client));
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteClientAsync(int id)
        {
            AddAuthHeader();
            var response = await _httpClient.DeleteAsync($"api/clients/{id}");
            return response.IsSuccessStatusCode;
        }

        // Contracts
        public async Task<List<Contract>> GetContractsAsync(DateTime? startDate = null, DateTime? endDate = null, ContractStatus? status = null)
        {
            AddAuthHeader();
            var query = "";
            if (startDate.HasValue) query += $"startDate={startDate.Value:yyyy-MM-dd}&";
            if (endDate.HasValue) query += $"endDate={endDate.Value:yyyy-MM-dd}&";
            if (status.HasValue) query += $"status={status.Value}&";

            var response = await _httpClient.GetAsync($"api/contracts?{query}");
            if (!response.IsSuccessStatusCode) return new List<Contract>();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<Contract>>(json, _jsonOptions) ?? new List<Contract>();
        }

        public async Task<Contract?> GetContractAsync(int id)
        {
            AddAuthHeader();
            var response = await _httpClient.GetAsync($"api/contracts/{id}");
            if (!response.IsSuccessStatusCode) return null;
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Contract>(json, _jsonOptions);
        }

        public async Task<bool> CreateContractAsync(Contract contract)
        {
            AddAuthHeader();
            var response = await _httpClient.PostAsync("api/contracts", ToJson(contract));
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateContractAsync(Contract contract)
        {
            AddAuthHeader();
            var response = await _httpClient.PutAsync($"api/contracts/{contract.Id}", ToJson(contract));
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateContractStatusAsync(int id, ContractStatus status)
        {
            AddAuthHeader();
            var response = await _httpClient.PatchAsync($"api/contracts/{id}/status", ToJson((int)status));
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteContractAsync(int id)
        {
            AddAuthHeader();
            var response = await _httpClient.DeleteAsync($"api/contracts/{id}");
            return response.IsSuccessStatusCode;
        }

        // Service Requests
        public async Task<List<ServiceRequest>> GetServiceRequestsAsync()
        {
            AddAuthHeader();
            var response = await _httpClient.GetAsync("api/servicerequests");
            if (!response.IsSuccessStatusCode) return new List<ServiceRequest>();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<ServiceRequest>>(json, _jsonOptions) ?? new List<ServiceRequest>();
        }

        public async Task<ServiceRequest?> GetServiceRequestAsync(int id)
        {
            AddAuthHeader();
            var response = await _httpClient.GetAsync($"api/servicerequests/{id}");
            if (!response.IsSuccessStatusCode) return null;
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ServiceRequest>(json, _jsonOptions);
        }

        public async Task<(bool success, string message)> CreateServiceRequestAsync(ServiceRequest serviceRequest)
        {
            AddAuthHeader();
            var response = await _httpClient.PostAsync("api/servicerequests", ToJson(serviceRequest));
            if (response.IsSuccessStatusCode) return (true, "Created successfully");
            var error = await response.Content.ReadAsStringAsync();
            return (false, error);
        }

        public async Task<bool> UpdateServiceRequestAsync(ServiceRequest serviceRequest)
        {
            AddAuthHeader();
            var response = await _httpClient.PutAsync($"api/servicerequests/{serviceRequest.Id}", ToJson(serviceRequest));
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteServiceRequestAsync(int id)
        {
            AddAuthHeader();
            var response = await _httpClient.DeleteAsync($"api/servicerequests/{id}");
            return response.IsSuccessStatusCode;
        }

        public async Task<decimal> GetExchangeRateAsync()
        {
            AddAuthHeader();
            var response = await _httpClient.GetAsync("api/servicerequests/exchangerate");
            if (!response.IsSuccessStatusCode) return 18.50m;
            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<JsonElement>(json);
            return result.GetProperty("rate").GetDecimal();
        }
    }
}
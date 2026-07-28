using System.Net.Http.Headers;
using System.Text.Json;
using DogoFinance.Integration.Interfaces;
using DogoFinance.Integration.Models.Dojah;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DogoFinance.Integration.Services
{
    public class DojahService : IDojahService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<DojahService> _logger;

        public DojahService(HttpClient httpClient, IConfiguration configuration, ILogger<DojahService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
        }

        private void PrepareHeaders()
        {
            var appId = _configuration["Dojah:AppId"];
            var secretKey = _configuration["Dojah:SecretKey"];

            _httpClient.DefaultRequestHeaders.Clear();
            if (!string.IsNullOrEmpty(appId))
            {
                _httpClient.DefaultRequestHeaders.Add("AppId", appId);
            }
            if (!string.IsNullOrEmpty(secretKey))
            {
                _httpClient.DefaultRequestHeaders.Add("Authorization", secretKey);
            }
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<DojahResponse<DojahBvnData>?> ValidateBvnAsync(string bvn, string? firstName = null, string? lastName = null, string? dateOfBirth = null)
        {
            try
            {
                var baseUrl = _configuration["Dojah:BaseUrl"] ?? "https://api.dojah.io";
                PrepareHeaders();

                var queryParams = new List<string> { $"bvn={Uri.EscapeDataString(bvn)}" };
                if (!string.IsNullOrEmpty(firstName)) queryParams.Add($"first_name={Uri.EscapeDataString(firstName)}");
                if (!string.IsNullOrEmpty(lastName)) queryParams.Add($"last_name={Uri.EscapeDataString(lastName)}");
                if (!string.IsNullOrEmpty(dateOfBirth)) queryParams.Add($"date_of_birth={Uri.EscapeDataString(dateOfBirth)}");

                var url = $"{baseUrl.TrimEnd('/')}/api/v1/kyc/bvn?{string.Join("&", queryParams)}";
                _logger.LogInformation("Calling Dojah BVN validation endpoint: {Url}", url);

                var response = await _httpClient.GetAsync(url);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return JsonSerializer.Deserialize<DojahResponse<DojahBvnData>>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                }

                _logger.LogError("Dojah BVN Validation Failed. StatusCode: {StatusCode}, Content: {Content}", response.StatusCode, responseContent);
                return JsonSerializer.Deserialize<DojahResponse<DojahBvnData>>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while calling Dojah BVN validation.");
                return null;
            }
        }

        public async Task<DojahResponse<DojahNinData>?> LookupNinAsync(string nin, string? firstName = null, string? lastName = null, string? dateOfBirth = null)
        {
            try
            {
                var baseUrl = _configuration["Dojah:BaseUrl"] ?? "https://api.dojah.io";
                PrepareHeaders();

                var queryParams = new List<string> { $"nin={Uri.EscapeDataString(nin)}" };
                if (!string.IsNullOrEmpty(firstName)) queryParams.Add($"first_name={Uri.EscapeDataString(firstName)}");
                if (!string.IsNullOrEmpty(lastName)) queryParams.Add($"last_name={Uri.EscapeDataString(lastName)}");
                if (!string.IsNullOrEmpty(dateOfBirth)) queryParams.Add($"date_of_birth={Uri.EscapeDataString(dateOfBirth)}");

                var url = $"{baseUrl.TrimEnd('/')}/api/v1/kyc/nin?{string.Join("&", queryParams)}";
                _logger.LogInformation("Calling Dojah NIN lookup endpoint: {Url}", url);

                var response = await _httpClient.GetAsync(url);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return JsonSerializer.Deserialize<DojahResponse<DojahNinData>>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                }

                _logger.LogError("Dojah NIN Lookup Failed. StatusCode: {StatusCode}, Content: {Content}", response.StatusCode, responseContent);
                return JsonSerializer.Deserialize<DojahResponse<DojahNinData>>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while calling Dojah NIN lookup.");
                return null;
            }
        }
    }
}

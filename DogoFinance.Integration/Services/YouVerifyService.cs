using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using DogoFinance.Integration.Interfaces;
using DogoFinance.Integration.Models.YouVerify;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DogoFinance.Integration.Services
{
    public class YouVerifyService : IYouVerifyService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<YouVerifyService> _logger;

        public YouVerifyService(HttpClient httpClient, IConfiguration configuration, ILogger<YouVerifyService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<YouVerifyResponse<IdentityVerificationData>?> VerifyBvn(BvnVerificationRequest request)
        {
            try
            {
                var apiKey = _configuration["YouVerify:ApiKey"];
                var baseUrl = _configuration["YouVerify:BaseUrl"];

                if (string.IsNullOrEmpty(apiKey))
                {
                    _logger.LogError("YouVerify API Key is not configured.");
                    return null;
                }

                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("token", apiKey);
                _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var url = $"{baseUrl.TrimEnd('/')}/v2/api/identity/ng/bvn";
                _logger.LogInformation("Calling YouVerify BVN verification endpoint: {Url}", url);

                var response = await _httpClient.PostAsync(url, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return JsonSerializer.Deserialize<YouVerifyResponse<IdentityVerificationData>>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                }

                _logger.LogError("YouVerify BVN Verification Failed. StatusCode: {StatusCode}, Content: {Content}", response.StatusCode, responseContent);
                return JsonSerializer.Deserialize<YouVerifyResponse<IdentityVerificationData>>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while calling YouVerify BVN verification.");
                return null;
            }
        }

        public async Task<YouVerifyResponse<IdentityVerificationData>?> VerifyNin(NinVerificationRequest request)
        {
            try
            {
                var apiKey = _configuration["YouVerify:ApiKey"];
                var baseUrl = _configuration["YouVerify:BaseUrl"];

                if (string.IsNullOrEmpty(apiKey))
                {
                    _logger.LogError("YouVerify API Key is not configured.");
                    return null;
                }

                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("token", apiKey);
                _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var url = $"{baseUrl.TrimEnd('/')}/v2/api/identity/ng/nin";
                _logger.LogInformation("Calling YouVerify NIN verification endpoint: {Url}", url);

                var response = await _httpClient.PostAsync(url, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return JsonSerializer.Deserialize<YouVerifyResponse<IdentityVerificationData>>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                }

                _logger.LogError("YouVerify NIN Verification Failed. StatusCode: {StatusCode}, Content: {Content}", response.StatusCode, responseContent);
                return JsonSerializer.Deserialize<YouVerifyResponse<IdentityVerificationData>>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while calling YouVerify NIN verification.");
                return null;
            }
        }
    }
}

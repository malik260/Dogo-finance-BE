using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using DogoFinance.Integration.Interfaces;
using DogoFinance.Integration.Models.Monnify;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DogoFinance.Integration.Services
{
    public class MonnifyService : IMonnifyService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<MonnifyService> _logger;
        private string? _accessToken;

        public MonnifyService(HttpClient httpClient, IConfiguration configuration, ILogger<MonnifyService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
        }

        private async Task Authenticate()
        {
            try
            {
                var apiKey = _configuration["Monnify:ApiKey"];
                var secretKey = _configuration["Monnify:SecretKey"];
                var baseUrl = _configuration["Monnify:BaseUrl"];

                var authString = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{apiKey}:{secretKey}"));
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authString);

                var response = await _httpClient.PostAsync($"{baseUrl}/api/v1/auth/login", null);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var authResponse = JsonSerializer.Deserialize<AuthResponse>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    _accessToken = authResponse?.ResponseBody?.AccessToken;
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<InitializeTransactionResponse?> InitializeTransaction(InitializeTransactionRequest request)
        {
            await Authenticate();
            if (string.IsNullOrEmpty(_accessToken)) return null;

            request.contractCode = _configuration["Monnify:ContractCode"];
            var baseUrl = _configuration["Monnify:BaseUrl"];

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{baseUrl}/api/v1/merchant/transactions/init-transaction", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
                return JsonSerializer.Deserialize<InitializeTransactionResponse>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            _logger.LogError("Monnify Initiation Failed: {Content}", responseContent);
            return null;
        }

        public async Task<SingleTransferResponse?> SingleTransfer(SingleTransferRequest request)
        {
            await Authenticate();
            if (string.IsNullOrEmpty(_accessToken)) return null;

            request.SourceAccountNumber = _configuration["Monnify:MonnifyAccountNumber"];
            var baseUrl = _configuration["Monnify:BaseUrl"];

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{baseUrl}/api/v1/disbursements/single", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
                return JsonSerializer.Deserialize<SingleTransferResponse>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            _logger.LogError("Monnify Transfer Failed: {Content}", responseContent);
            return null;
        }

        public async Task<dynamic?> ChargeCard(CardChargeRequest request)
        {
            await Authenticate();
            if (string.IsNullOrEmpty(_accessToken)) return null;

            var baseUrl = _configuration["Monnify:BaseUrl"];
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
            
            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{baseUrl}/api/v1/merchant/cards/charge", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<dynamic>(responseContent);
        }

        public async Task<bool> AuthorizeOtp(AuthorizeOtpRequest request)
        {
            await Authenticate();
            if (string.IsNullOrEmpty(_accessToken)) return false;

            var baseUrl = _configuration["Monnify:BaseUrl"];
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);

            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{baseUrl}/api/v1/merchant/cards/otp/authorize", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<ReservedAccountResponse?> CreateReservedAccount(CreateReservedAccountRequest request)
        {
            await Authenticate();
            if (string.IsNullOrEmpty(_accessToken)) return null;

            request.contractCode = _configuration["Monnify:ContractCode"];
            var baseUrl = _configuration["Monnify:BaseUrl"];

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
            var response = await _httpClient.PostAsJsonAsync($"{baseUrl}/api/v2/bank-transfer/reserved-accounts", request);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<ReservedAccountResponse>();
            }
            return null;
        }

        public async Task<TransactionStatusResponse?> VerifyTransaction(string reference)
        {
            await Authenticate();
            if (string.IsNullOrEmpty(_accessToken)) return null;

            var baseUrl = _configuration["Monnify:BaseUrl"];
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);

            var response = await _httpClient.GetAsync($"{baseUrl}/api/v2/transactions/{reference}");
            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
                return JsonSerializer.Deserialize<TransactionStatusResponse>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return null;
        }

        public async Task<BvnMatchResponse?> VerifyBvnMatch(BvnMatchRequest request)
        {
            await Authenticate();
            if (string.IsNullOrEmpty(_accessToken)) return null;

            var baseUrl = _configuration["Monnify:BaseUrl"];
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);

            var response = await _httpClient.PostAsJsonAsync($"{baseUrl}/api/v1/vas/bvn-details-match", request);
            var responseContent = response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<BvnMatchResponse>();
            }
            return null;
        }

        public async Task<NinVerifyResponse?> VerifyNin(NinVerifyRequest request)
        {
            await Authenticate();
            if (string.IsNullOrEmpty(_accessToken)) return null;

            var baseUrl = _configuration["Monnify:BaseUrl"];
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);

            var response = await _httpClient.PostAsJsonAsync($"{baseUrl}/api/v1/vas/nin-details", request);
            var reponseContent = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<NinVerifyResponse>();
            }
            return null;
        }
    }

    internal class AuthResponse
    {
        public bool RequestSuccessful { get; set; }
        public AuthResponseBody? ResponseBody { get; set; }
    }

    internal class AuthResponseBody
    {
        public string? AccessToken { get; set; }
        public int ExpiresIn { get; set; }
    }
}

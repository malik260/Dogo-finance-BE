using System.Text.Json;
using System.Text.Json.Serialization;
using DogoFinance.Integration.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DogoFinance.Integration.Services
{
    public class FxRateService : IFxRateService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<FxRateService> _logger;
        private const string CacheKey = "FxRate_NGN_USD";

        public FxRateService(HttpClient httpClient, IConfiguration configuration, IMemoryCache memoryCache, ILogger<FxRateService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _memoryCache = memoryCache;
            _logger = logger;
        }

        private class ExchangeRateApiResponse
        {
            [JsonPropertyName("result")]
            public string? Result { get; set; }

            [JsonPropertyName("rates")]
            public Dictionary<string, decimal>? Rates { get; set; }
        }

        public async Task<FxRateResult> GetNgnToUsdRateAsync()
        {
            var fallbackRate = _configuration.GetValue<decimal>("FxRateConfig:FallbackNgnPerUsdRate", 1550.00m);
            var marginPercent = _configuration.GetValue<decimal>("FxRateConfig:MarginPercentage", 1.5m);
            var cacheMinutes = _configuration.GetValue<int>("FxRateConfig:CacheDurationInMinutes", 15);
            var provider = _configuration.GetValue<string>("FxRateConfig:Provider") ?? "ExchangeRateApi";
            var url = _configuration.GetValue<string>("FxRateConfig:BaseUrl") ?? "https://open.er-api.com/v6/latest/USD";

            if (_memoryCache.TryGetValue(CacheKey, out FxRateResult? cachedResult) && cachedResult != null)
            {
                return cachedResult;
            }

            try
            {
                _logger.LogInformation("Fetching live FX exchange rate from: {Url}", url);
                var response = await _httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var data = JsonSerializer.Deserialize<ExchangeRateApiResponse>(content);

                    if (data?.Rates != null && data.Rates.TryGetValue("NGN", out decimal ngnRate) && ngnRate > 0)
                    {
                        var effectiveRate = ngnRate * (1m + (marginPercent / 100m));
                        var result = new FxRateResult
                        {
                            NgnPerUsdRate = Math.Round(ngnRate, 2),
                            EffectiveRateWithMargin = Math.Round(effectiveRate, 2),
                            Provider = provider,
                            IsFallback = false,
                            FetchedAt = DateTime.UtcNow
                        };

                        _memoryCache.Set(CacheKey, result, TimeSpan.FromMinutes(cacheMinutes));
                        return result;
                    }
                }

                _logger.LogWarning("Failed to fetch live FX rate from API. StatusCode: {Status}. Using fallback rate {FallbackRate}", response.StatusCode, fallbackRate);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching live FX rate from API. Using fallback rate {FallbackRate}", fallbackRate);
            }

            var fallbackEffective = fallbackRate * (1m + (marginPercent / 100m));
            var fallbackResult = new FxRateResult
            {
                NgnPerUsdRate = fallbackRate,
                EffectiveRateWithMargin = Math.Round(fallbackEffective, 2),
                Provider = "FallbackConfig",
                IsFallback = true,
                FetchedAt = DateTime.UtcNow
            };

            _memoryCache.Set(CacheKey, fallbackResult, TimeSpan.FromMinutes(5));
            return fallbackResult;
        }
    }
}

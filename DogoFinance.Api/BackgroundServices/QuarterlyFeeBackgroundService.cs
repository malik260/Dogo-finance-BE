using System;
using System.Threading;
using System.Threading.Tasks;
using DogoFinance.ProductManagement.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DogoFinance.Api.BackgroundServices
{
    public class QuarterlyFeeBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<QuarterlyFeeBackgroundService> _logger;

        public QuarterlyFeeBackgroundService(IServiceProvider serviceProvider, ILogger<QuarterlyFeeBackgroundService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("QuarterlyFeeBackgroundService started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var now = DateTime.UtcNow;

                    // Execute on/after the 10th day of the month following quarter end:
                    // Q1 (Jan-Mar) -> April 10
                    // Q2 (Apr-Jun) -> July 10
                    // Q3 (Jul-Sep) -> October 10
                    // Q4 (Oct-Dec) -> January 10
                    if (now.Day >= 10 && (now.Month == 4 || now.Month == 7 || now.Month == 10 || now.Month == 1))
                    {
                        int targetYear = now.Month == 1 ? now.Year - 1 : now.Year;
                        int targetQuarter = now.Month switch
                        {
                            4 => 1,
                            7 => 2,
                            10 => 3,
                            1 => 4,
                            _ => 1
                        };

                        _logger.LogInformation("Quarterly Fee Trigger Date Matched. Checking execution for Year {Year} Q{Quarter}", targetYear, targetQuarter);

                        using var scope = _serviceProvider.CreateScope();
                        var feeService = scope.ServiceProvider.GetRequiredService<IFeeComputationService>();

                        // Safe to call: Idempotency guard in FeeComputationService skips if already run
                        var result = await feeService.ExecuteQuarterlyFeeDeductionsAsync(targetYear, targetQuarter);
                        _logger.LogInformation("Quarterly Fee Background Execution Status: {Message}", result.Message);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in QuarterlyFeeBackgroundService execution loop.");
                }

                // Check every 12 hours
                await Task.Delay(TimeSpan.FromHours(12), stoppingToken);
            }
        }
    }
}

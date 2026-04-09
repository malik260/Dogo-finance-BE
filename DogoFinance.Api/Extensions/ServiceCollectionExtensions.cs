using DogoFinance.CustomerManagement.Interfaces;
using DogoFinance.CustomerManagement.Services;
using DogoFinance.Integration.Interfaces;
using DogoFinance.Integration.Services;
using DogoFinance.Authentication.Interfaces;
using DogoFinance.Authentication.Services;
using DogoFinance.AdminManagement.Interfaces;
using DogoFinance.AdminManagement.Services;
using DogoFinance.BusinessLogic.Layer.Helpers;
using DogoFinance.TransactionManagement.Interfaces;
using DogoFinance.TransactionManagement.Services;
using DogoFinance.ProductManagement.Interfaces;
using DogoFinance.ProductManagement.Services;
using DogoFinance.DataAccess.Layer;
using DogoFinance.DataAccess.Layer.Global;
using DogoFinance.DataAccess.Layer.Interfaces;
using DogoFinance.DataAccess.Layer.Repositories.Base;

namespace DogoFinance.Api.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDogoFinanceServices(this IServiceCollection services)
        {
            // Register Unit of Work
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Register BLL Services
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<INextOfKinService, NextOfKinService>();
            services.AddScoped<IBankService, BankService>();
            services.AddScoped<IEmailService, SmtpEmailService>();

            // Register Authentication & Admin
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IPinService, PinService>();
            services.AddScoped<IAdminService, AdminService>();
            services.AddScoped<ISystemSettingService, SystemSettingService>();
            services.AddSingleton<JwtHelper>();

            // Register Monnify
            services.AddHttpClient<IMonnifyService, MonnifyService>();

            // Register Transaction Services
            services.AddScoped<ITransactionService, TransactionService>();
            services.AddScoped<ICustomerPortfolioService, CustomerPortfolioService>();
            services.AddScoped<ICustomerHoldingService, CustomerHoldingService>();

            // Register Portfolio & Product Services
            services.AddScoped<IAssetClassService, AssetClassService>();
            services.AddScoped<IPortfolioTypeService, PortfolioTypeService>();
            services.AddScoped<IPortfolioService, PortfolioService>();
            services.AddScoped<IInstrumentService, InstrumentService>();
            services.AddScoped<IPortfolioInstrumentService, PortfolioInstrumentService>();
            services.AddScoped<IPortfolioAllocationRuleService, PortfolioAllocationRuleService>();
            services.AddScoped<IInstrumentPriceService, InstrumentPriceService>();

            // Register individual repositories if needed (Fintrak often does both)
            // services.AddScoped<IAccountRepository, AccountRepository>();

            // Set global services for reference in DAL if needed
            GlobalContext.Services = services;

            return services;
        }

        public static void ConfigureDogoFinance(this WebApplication app)
        {
            // Initialize static GlobalContext with configuration
            GlobalContext.Configuration = app.Configuration;
            GlobalContext.HostingEnvironment = app.Environment;
            GlobalContext.ServiceProvider = app.Services;

            // Set initial connection settings from SystemConfig
            GlobalContext.Provider = GlobalContext.SystemConfig?.DBProvider;
            GlobalContext.ConnectionString = GlobalContext.SystemConfig?.DBConnectionString;
            GlobalContext.CommandTimeoutSeconds = GlobalContext.SystemConfig?.DBCommandTimeout ?? 30;
        }
    }
}

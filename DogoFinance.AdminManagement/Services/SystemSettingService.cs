using DogoFinance.BusinessLogic.Layer.Response;
using DogoFinance.DataAccess.Layer.Interfaces;
using DogoFinance.DataAccess.Layer.Repositories.Base;
using DogoFinance.AdminManagement.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace DogoFinance.AdminManagement.Services
{
    public class SystemSettingService : DataRepository, ISystemSettingService
    {
        private readonly IUnitOfWork _uow;
        private readonly ILogger<SystemSettingService> _logger;

        public SystemSettingService(IUnitOfWork uow, ILogger<SystemSettingService> logger)
        {
            _uow = uow;
            _logger = logger;
        }

        public async Task<ApiResponse> GetSessionTimeout()
        {
            var response = new ApiResponse();
            try
            {
                var setting = await _uow.SystemSettings.GetSystemSetting();
                int timeout = setting?.SessionTimeoutInMinutes ?? 30; // Default to 30 if not found
                response.SetMessage("Session timeout retrieved", true, new { timeoutInMinutes = timeout });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving session timeout");
                response.SetError("Failed to retrieve session timeout", 500);
            }
            return response;
        }
    }
}

using DogoFinance.BusinessLogic.Layer.Response;
using DogoFinance.AdminManagement.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DogoFinance.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SystemSettingController : ControllerBase
    {
        private readonly ISystemSettingService _settingService;

        public SystemSettingController(ISystemSettingService settingService)
        {
            _settingService = settingService;
        }

        [HttpGet("timeout")]
        public async Task<ActionResult<ApiResponse>> GetSessionTimeout()
        {
            var result = await _settingService.GetSessionTimeout();
            return Ok(result);
        }
    }
}

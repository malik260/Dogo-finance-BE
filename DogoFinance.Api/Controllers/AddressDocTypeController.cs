using DogoFinance.DataAccess.Layer.Interfaces;
using DogoFinance.DataAccess.Layer.Models.Entities;
using DogoFinance.BusinessLogic.Layer.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace DogoFinance.Api.Controllers
{
    [ApiController]
    [Authorize] // Admin only in real app
    [Route("api/[controller]")]
    public class AddressDocTypeController : ControllerBase
    {
        private readonly IUnitOfWork _uow;

        public AddressDocTypeController(IUnitOfWork uow)
        {
            _uow = uow;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse>> GetAll()
        {
            var types = await _uow.GenericRepository.FindList<TblAddressDocType>(x => x.IsActive);
            return Ok(new ApiResponse { Success = true, Data = types, Boolean = true });
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse>> Create([FromBody] TblAddressDocType docType)
        {
            docType.CreatedAt = DateTime.UtcNow;
            docType.IsActive = true;
            await _uow.GenericRepository.Insert(docType);
            await _uow.SaveChangesAsync();
            return Ok(new ApiResponse { Success = true, Message = "Document type created", Boolean = true });
        }
    }
}

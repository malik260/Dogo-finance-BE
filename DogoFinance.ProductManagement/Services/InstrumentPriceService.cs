using DogoFinance.BusinessLogic.Layer.Response;
using DogoFinance.BusinessLogic.Layer.Models.Request;
using DogoFinance.DataAccess.Layer.DTO;
using DogoFinance.DataAccess.Layer.Interfaces;
using DogoFinance.DataAccess.Layer.Models.Entities;
using DogoFinance.DataAccess.Layer.Repositories.Base;
using DogoFinance.ProductManagement.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace DogoFinance.ProductManagement.Services
{
    public class InstrumentPriceService : DataRepository, IInstrumentPriceService
    {
        private readonly IUnitOfWork _uow;
        private readonly ILogger<InstrumentPriceService> _logger;

        public InstrumentPriceService(IUnitOfWork uow, ILogger<InstrumentPriceService> logger)
        {
            _uow = uow;
            _logger = logger;
        }

        public async Task<ApiResponse> GetList()
        {
            var response = new ApiResponse();
            try {
                var data = await _uow.Portfolios.GetInstrumentPrices(0);
                response.SetMessage("Retrieved", true, data);
            } catch (Exception ex) {
                _logger.LogError(ex, "Error getting prices");
                response.SetError("Internal server error", 500);
            }
            return response;
        }

        public async Task<ApiResponse> Save(InstrumentPriceDto model)
        {
            var response = new ApiResponse();
            try {
                var entity = model.Id == 0 ? new TblInstrumentPrice() : await _uow.Portfolios.GetInstrumentPriceById(model.Id);
                if (entity == null) { response.SetError("Not found", 404); return response; }
                entity.InstrumentId = model.InstrumentId;
                entity.PriceDate = model.PriceDate;
                entity.NAV = model.NAV;
                entity.PriceSource = model.PriceSource;
                
                if (model.Id == 0) entity.CreatedAt = DateTime.UtcNow;

                await _uow.Portfolios.SaveInstrumentPrice(entity);
                response.SetMessage("Saved successfully", true);
            } catch (Exception ex) {
                _logger.LogError(ex, "Error saving price");
                response.SetError("Internal server error", 500);
            }
            return response;
        }

        public async Task<ApiResponse> Delete(int id)
        {
            var response = new ApiResponse();
            try {
                await _uow.Portfolios.DeleteInstrumentPrice(id);
                response.SetMessage("Deleted successfully", true);
            } catch (Exception ex) {
                _logger.LogError(ex, "Error deleting price");
                response.SetError("Internal server error", 500);
            }
            return response;
        }
    }
}

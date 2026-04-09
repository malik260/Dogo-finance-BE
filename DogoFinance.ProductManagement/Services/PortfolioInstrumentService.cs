using DogoFinance.BusinessLogic.Layer.Response;
using DogoFinance.BusinessLogic.Layer.Models.Request;
using DogoFinance.DataAccess.Layer.Interfaces;
using DogoFinance.DataAccess.Layer.Models.Entities;
using DogoFinance.DataAccess.Layer.Repositories.Base;
using DogoFinance.ProductManagement.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace DogoFinance.ProductManagement.Services
{
    public class PortfolioInstrumentService : DataRepository, IPortfolioInstrumentService
    {
        private readonly IUnitOfWork _uow;
        private readonly ILogger<PortfolioInstrumentService> _logger;

        public PortfolioInstrumentService(IUnitOfWork uow, ILogger<PortfolioInstrumentService> logger)
        {
            _uow = uow;
            _logger = logger;
        }

        public async Task<ApiResponse> GetList()
        {
            var response = new ApiResponse();
            try {
                // Returns all as a fallback, usually filtered by PortfolioId
                var data = await _uow.Portfolios.GetPortfolioInstruments(0);
                response.SetMessage("Retrieved", true, data);
            } catch (Exception ex) {
                _logger.LogError(ex, "Error getting portfolio instruments");
                response.SetError("Internal server error", 500);
            }
            return response;
        }

        public async Task<ApiResponse> Save(PortfolioInstrumentDto model)
        {
            var response = new ApiResponse();
            try {
                var entity = model.Id == 0 ? new TblPortfolioInstrument() : await _uow.Portfolios.GetPortfolioInstrumentById(model.Id);
                if (entity == null) { response.SetError("Not found", 404); return response; }
                entity.PortfolioId = model.PortfolioId;
                entity.InstrumentId = model.InstrumentId;
                entity.TargetWeight = model.TargetWeight;
                await _uow.Portfolios.SavePortfolioInstrument(entity);
                response.SetMessage("Saved successfully", true);
            } catch (Exception ex) {
                _logger.LogError(ex, "Error saving portfolio instrument");
                response.SetError("Internal server error", 500);
            }
            return response;
        }

        public async Task<ApiResponse> Delete(int id)
        {
            var response = new ApiResponse();
            try {
                await _uow.Portfolios.DeletePortfolioInstrument(id);
                response.SetMessage("Deleted successfully", true);
            } catch (Exception ex) {
                _logger.LogError(ex, "Error deleting portfolio instrument");
                response.SetError("Internal server error", 500);
            }
            return response;
        }
    }
}

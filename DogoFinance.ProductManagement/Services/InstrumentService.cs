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
    public class InstrumentService : DataRepository, IInstrumentService
    {
        private readonly IUnitOfWork _uow;
        private readonly ILogger<InstrumentService> _logger;

        public InstrumentService(IUnitOfWork uow, ILogger<InstrumentService> logger)
        {
            _uow = uow;
            _logger = logger;
        }

        public async Task<ApiResponse> GetList()
        {
            var response = new ApiResponse();
            try
            {
                var data = await _uow.Portfolios.GetInstruments();
                response.SetMessage("Retrieved", true, data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting instruments");
                response.SetError("Internal server error", 500);
            }
            return response;
        }

        public async Task<ApiResponse> Save(InstrumentDto model)
        {
            var response = new ApiResponse();
            try
            {
                var entity = model.InstrumentId == 0 ? new TblInstrument() : await _uow.Portfolios.GetInstrumentById(model.InstrumentId);
                if (entity == null) { response.SetError("Not found", 404); return response; }

                entity.Name = model.Name;
                entity.Code = model.Code;
                entity.AssetClassId = model.AssetClassId;
                entity.IsShariahCompliant = model.IsShariahCompliant;
                entity.IsActive = model.IsActive;
                
                if (model.InstrumentId == 0) entity.CreatedAt = DateTime.UtcNow;

                await _uow.Portfolios.SaveInstrument(entity);
                response.SetMessage("Saved successfully", true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving instrument");
                response.SetError("Internal server error", 500);
            }
            return response;
        }

        public async Task<ApiResponse> Delete(int id)
        {
            var response = new ApiResponse();
            try
            {
                await _uow.Portfolios.DeleteInstrument(id);
                response.SetMessage("Deleted successfully", true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting instrument");
                response.SetError("Internal server error", 500);
            }
            return response;
        }
    }
}

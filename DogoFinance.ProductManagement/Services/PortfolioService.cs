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
    public class PortfolioService : DataRepository, IPortfolioService
    {
        private readonly IUnitOfWork _uow;
        private readonly ILogger<PortfolioService> _logger;

        public PortfolioService(IUnitOfWork uow, ILogger<PortfolioService> logger)
        {
            _uow = uow;
            _logger = logger;
        }

        public async Task<ApiResponse> GetList()
        {
            var response = new ApiResponse();
            try
            {
                var data = await _uow.Portfolios.GetPortfolios();
                response.SetMessage("Retrieved", true, data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting portfolios");
                response.SetError("Internal server error", 500);
            }
            return response;
        }

        public async Task<ApiResponse> Save(PortfolioDto model)
        {
            var response = new ApiResponse();
            try
            {
                var entity = model.PortfolioId == 0 ? new TblPortfolio() : await _uow.Portfolios.GetPortfolioById(model.PortfolioId);
                if (entity == null) { response.SetError("Not found", 404); return response; }

                entity.Name = model.Name;
                entity.Code = model.Code;
                entity.PortfolioTypeId = model.PortfolioTypeId;
                entity.RiskLevel = model.RiskLevel;
                entity.Description = model.Description;
                entity.ExpectedAnnualReturn = model.ExpectedAnnualReturn;
                entity.IsActive = model.IsActive;
                
                if (model.PortfolioId == 0) entity.CreatedAt = DateTime.UtcNow;

                await _uow.Portfolios.SavePortfolio(entity);
                response.SetMessage("Saved successfully", true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving portfolio");
                response.SetError("Internal server error", 500);
            }
            return response;
        }

        public async Task<ApiResponse> Delete(int id)
        {
            var response = new ApiResponse();
            try
            {
                await _uow.Portfolios.DeletePortfolio(id);
                response.SetMessage("Deleted successfully", true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting portfolio");
                response.SetError("Internal server error", 500);
            }
            return response;
        }
    }
}

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
    public class PortfolioAllocationRuleService : DataRepository, IPortfolioAllocationRuleService
    {
        private readonly IUnitOfWork _uow;
        private readonly ILogger<PortfolioAllocationRuleService> _logger;

        public PortfolioAllocationRuleService(IUnitOfWork uow, ILogger<PortfolioAllocationRuleService> logger)
        {
            _uow = uow;
            _logger = logger;
        }

        public async Task<ApiResponse> GetList()
        {
            var response = new ApiResponse();
            try {
                var data = await _uow.Portfolios.GetAllocationRules(0);
                response.SetMessage("Retrieved", true, data);
            } catch (Exception ex) {
                _logger.LogError(ex, "Error getting rules");
                response.SetError("Internal server error", 500);
            }
            return response;
        }

        public async Task<ApiResponse> Save(PortfolioAllocationRuleDto model)
        {
            var response = new ApiResponse();
            try {
                var entity = model.Id == 0 ? new TblPortfolioAllocationRule() : await _uow.Portfolios.GetAllocationRuleById(model.Id);
                if (entity == null) { response.SetError("Not found", 404); return response; }
                entity.PortfolioId = model.PortfolioId;
                entity.AssetClassId = model.AssetClassId;
                entity.TargetPercentage = model.TargetPercentage;
                entity.MinPercentage = model.MinPercentage;
                entity.MaxPercentage = model.MaxPercentage;
                entity.ExpectedReturn = model.ExpectedReturn;
                await _uow.Portfolios.SaveAllocationRule(entity);
                response.SetMessage("Saved successfully", true);
            } catch (Exception ex) {
                _logger.LogError(ex, "Error saving rule");
                response.SetError("Internal server error", 500);
            }
            return response;
        }

        public async Task<ApiResponse> Delete(int id)
        {
            var response = new ApiResponse();
            try {
                await _uow.Portfolios.DeleteAllocationRule(id);
                response.SetMessage("Deleted successfully", true);
            } catch (Exception ex) {
                _logger.LogError(ex, "Error deleting rule");
                response.SetError("Internal server error", 500);
            }
            return response;
        }
    }
}

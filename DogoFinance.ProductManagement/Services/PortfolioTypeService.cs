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
    public class PortfolioTypeService : DataRepository, IPortfolioTypeService
    {
        private readonly IUnitOfWork _uow;
        private readonly ILogger<PortfolioTypeService> _logger;

        public PortfolioTypeService(IUnitOfWork uow, ILogger<PortfolioTypeService> logger)
        {
            _uow = uow;
            _logger = logger;
        }

        public async Task<ApiResponse> GetList()
        {
            var response = new ApiResponse();
            try
            {
                var data = await _uow.Portfolios.GetPortfolioTypes();
                response.SetMessage("Retrieved", true, data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting portfolio types");
                response.SetError("Internal server error", 500);
            }
            return response;
        }

        public async Task<ApiResponse> Save(PortfolioTypeDto model)
        {
            var response = new ApiResponse();
            try
            {
                var entity = model.PortfolioTypeId == 0 ? new TblPortfolioType() : await _uow.Portfolios.GetPortfolioTypeById(model.PortfolioTypeId);
                if (entity == null) { response.SetError("Not found", 404); return response; }

                entity.Name = model.Name;
                
                // Auto-generate code if empty
                if (string.IsNullOrEmpty(model.Code))
                {
                    entity.Code = model.Name.Replace(" ", "_").ToUpper();
                }
                else
                {
                    entity.Code = model.Code;
                }

                entity.SupportsAllocation = model.SupportsAllocation;
                
                if (model.PortfolioTypeId == 0) entity.CreatedAt = DateTime.UtcNow;

                await _uow.Portfolios.SavePortfolioType(entity);
                response.SetMessage("Saved successfully", true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving portfolio type");
                response.SetError("Internal server error", 500);
            }
            return response;
        }

        public async Task<ApiResponse> Delete(int id)
        {
            var response = new ApiResponse();
            try
            {
                await _uow.Portfolios.DeletePortfolioType(id);
                response.SetMessage("Deleted successfully", true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting portfolio type");
                response.SetError("Internal server error", 500);
            }
            return response;
        }
    }
}

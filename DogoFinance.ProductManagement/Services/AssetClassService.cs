using DogoFinance.BusinessLogic.Layer.Response;
using DogoFinance.BusinessLogic.Layer.Models.Request;
using DogoFinance.DataAccess.Layer.Interfaces;
using DogoFinance.DataAccess.Layer.Models.Entities;
using DogoFinance.DataAccess.Layer.Repositories.Base;
using DogoFinance.ProductManagement.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DogoFinance.ProductManagement.Services
{
    public class AssetClassService : DataRepository, IAssetClassService
    {
        private readonly IUnitOfWork _uow;
        private readonly ILogger<AssetClassService> _logger;

        public AssetClassService(IUnitOfWork uow, ILogger<AssetClassService> logger)
        {
            _uow = uow;
            _logger = logger;
        }

        public async Task<ApiResponse> GetAssetClasses()
        {
            var response = new ApiResponse();
            try
            {
                var data = await _uow.Portfolios.GetAssetClasses();
                response.SetMessage("Retrieved", true, data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting asset classes");
                response.SetError("Internal server error", 500);
            }
            return response;
        }

        public async Task<ApiResponse> SaveAssetClass(AssetClassDto model)
        {
            var response = new ApiResponse();
            try
            {
                var entity = model.AssetClassId == 0 ? new TblAssetClass() : await _uow.Portfolios.GetAssetClassById(model.AssetClassId);
                if (entity == null) { response.SetError("Not found", 404); return response; }

                entity.Name = model.Name;
                entity.Code = model.Code;
                entity.IsShariahCompliant = model.IsShariahCompliant;
                
                if (model.AssetClassId == 0) entity.CreatedAt = DateTime.UtcNow;

                await _uow.Portfolios.SaveAssetClass(entity);
                response.SetMessage("Saved successfully", true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving asset class");
                response.SetError("Internal server error", 500);
            }
            return response;
        }

        public async Task<ApiResponse> DeleteAssetClass(int id)
        {
            var response = new ApiResponse();
            try
            {
                await _uow.Portfolios.DeleteAssetClass(id);
                response.SetMessage("Deleted successfully", true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting asset class");
                response.SetError("Internal server error", 500);
            }
            return response;
        }
    }
}

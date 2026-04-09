using DogoFinance.BusinessLogic.Layer.Response;
using DogoFinance.BusinessLogic.Layer.Models.Request;
using DogoFinance.DataAccess.Layer.Interfaces;
using DogoFinance.DataAccess.Layer.Models.Entities;
using DogoFinance.DataAccess.Layer.Repositories.Base;
using DogoFinance.TransactionManagement.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace DogoFinance.TransactionManagement.Services
{
    public class CustomerPortfolioService : DataRepository, ICustomerPortfolioService
    {
        private readonly IUnitOfWork _uow;
        private readonly ILogger<CustomerPortfolioService> _logger;

        public CustomerPortfolioService(IUnitOfWork uow, ILogger<CustomerPortfolioService> logger)
        {
            _uow = uow;
            _logger = logger;
        }

        public async Task<ApiResponse> GetByCustomer(long customerId)
        {
            var response = new ApiResponse();
            try {
                var data = await _uow.Portfolios.GetCustomerPortfolios(customerId);
                response.SetMessage("Retrieved", true, data);
            } catch (Exception ex) {
                _logger.LogError(ex, "Error getting customer portfolios");
                response.SetError("Internal server error", 500);
            }
            return response;
        }

        public async Task<ApiResponse> Save(CustomerPortfolioDto model)
        {
            var response = new ApiResponse();
            try {
                var entity = model.Id == 0 ? new TblCustomerPortfolio() : await _uow.Portfolios.GetCustomerPortfolioById(model.Id);
                if (entity == null) { response.SetError("Not found", 404); return response; }

                entity.CustomerId = model.CustomerId;
                entity.PortfolioId = model.PortfolioId;
                entity.TotalInvested = model.TotalInvested;
                
                if (model.Id == 0) entity.CreatedAt = DateTime.UtcNow;

                await _uow.Portfolios.SaveCustomerPortfolio(entity);
                response.SetMessage("Saved successfully", true);
            } catch (Exception ex) {
                _logger.LogError(ex, "Error saving customer portfolio");
                response.SetError("Internal server error", 500);
            }
            return response;
        }

        public async Task<ApiResponse> Delete(long id)
        {
            var response = new ApiResponse();
            try {
                await _uow.Portfolios.DeleteCustomerPortfolio(id);
                response.SetMessage("Deleted successfully", true);
            } catch (Exception ex) {
                _logger.LogError(ex, "Error deleting customer portfolio");
                response.SetError("Internal server error", 500);
            }
            return response;
        }
    }
}

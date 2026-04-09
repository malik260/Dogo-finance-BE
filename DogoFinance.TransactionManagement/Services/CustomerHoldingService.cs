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
    public class CustomerHoldingService : DataRepository, ICustomerHoldingService
    {
        private readonly IUnitOfWork _uow;
        private readonly ILogger<CustomerHoldingService> _logger;

        public CustomerHoldingService(IUnitOfWork uow, ILogger<CustomerHoldingService> logger)
        {
            _uow = uow;
            _logger = logger;
        }

        public async Task<ApiResponse> GetByCustomer(long customerId)
        {
            var response = new ApiResponse();
            try {
                var data = await _uow.Portfolios.GetCustomerHoldings(customerId);
                response.SetMessage("Retrieved", true, data);
            } catch (Exception ex) {
                _logger.LogError(ex, "Error getting customer holdings");
                response.SetError("Internal server error", 500);
            }
            return response;
        }

        public async Task<ApiResponse> Save(CustomerHoldingDto model)
        {
            var response = new ApiResponse();
            try {
                var entity = model.Id == 0 ? new TblCustomerHolding() : await _uow.Portfolios.GetCustomerHoldingById(model.Id);
                if (entity == null) { response.SetError("Not found", 404); return response; }

                entity.CustomerId = model.CustomerId;
                entity.InstrumentId = model.InstrumentId;
                entity.Units = model.Units;
                entity.InvestedAmount = model.InvestedAmount;
                
                if (model.Id == 0) entity.CreatedAt = DateTime.UtcNow;

                await _uow.Portfolios.SaveCustomerHolding(entity);
                response.SetMessage("Saved successfully", true);
            } catch (Exception ex) {
                _logger.LogError(ex, "Error saving customer holding");
                response.SetError("Internal server error", 500);
            }
            return response;
        }

        public async Task<ApiResponse> Delete(long id)
        {
            var response = new ApiResponse();
            try {
                await _uow.Portfolios.DeleteCustomerHolding(id);
                response.SetMessage("Deleted successfully", true);
            } catch (Exception ex) {
                _logger.LogError(ex, "Error deleting customer holding");
                response.SetError("Internal server error", 500);
            }
            return response;
        }
    }
}

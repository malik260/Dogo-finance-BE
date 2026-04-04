using DogoFinance.BusinessLogic.Layer.Models.Request;
using DogoFinance.BusinessLogic.Layer.Response;
using DogoFinance.CustomerManagement.Interfaces;
using DogoFinance.DataAccess.Layer.Interfaces;
using DogoFinance.DataAccess.Layer.Models.Entities;
using DogoFinance.DataAccess.Layer.Repositories.Base;
using Microsoft.Extensions.Logging;

namespace DogoFinance.CustomerManagement.Services
{
    public class NextOfKinService : DataRepository, INextOfKinService
    {
        private readonly IUnitOfWork _uow;
        private readonly ILogger<NextOfKinService> _logger;

        public NextOfKinService(IUnitOfWork uow, ILogger<NextOfKinService> logger)
        {
            _uow = uow;
            _logger = logger;
        }

        public async Task<ApiResponse> AddNextOfKin(long customerId, AddNextOfKinRequest request)
        {
            var response = new ApiResponse();
            var customerExists = await BaseRepository().FindEntity<TblCustomer>(customerId);
            if (customerExists == null) return new ApiResponse { Message = "Customer not found", Status = 404 };

            var nok = new TblNextOfKin
            {
                CustomerId = customerId,
                FullName = request.FullName,
                RelationshipTypeId = request.RelationshipTypeId,
                PhoneNumber = request.PhoneNumber,
                Email = request.Email,
                Address = request.Address,
                CreatedAt = DateTime.UtcNow
            };

            await _uow.NextOfKins.SaveNextOfKin(nok);
            response.SetMessage("Next of kin added successfully", true);
            return response;
        }

        public async Task<ApiResponse> GetNextOfKins(long customerId)
        {
            var noks = await _uow.NextOfKins.GetByCustomerId(customerId);
            return new ApiResponse { Success = true, Data = noks, Message = "Next of kin list retrieved" };
        }

        public async Task<ApiResponse> GetRelationshipTypes()
        {
            var types = await BaseRepository().FindList<TblRelationshipType>();
            return new ApiResponse { Success = true, Boolean = true, Data = types, Message = "Relationship types retrieved" };
        }
    }
}

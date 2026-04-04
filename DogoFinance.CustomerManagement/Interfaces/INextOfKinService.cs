using DogoFinance.BusinessLogic.Layer.Models.Request;
using DogoFinance.BusinessLogic.Layer.Response;

namespace DogoFinance.CustomerManagement.Interfaces
{
    public interface INextOfKinService
    {
        Task<ApiResponse> AddNextOfKin(long customerId, AddNextOfKinRequest request);
        Task<ApiResponse> GetNextOfKins(long customerId);
        Task<ApiResponse> GetRelationshipTypes();
    }
}

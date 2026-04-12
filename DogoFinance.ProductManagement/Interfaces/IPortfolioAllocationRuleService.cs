using DogoFinance.BusinessLogic.Layer.Response;
using DogoFinance.BusinessLogic.Layer.Models.Request;
using System.Threading.Tasks;
using DogoFinance.DataAccess.Layer.DTO;

namespace DogoFinance.ProductManagement.Interfaces
{
    public interface IPortfolioAllocationRuleService
    {
        Task<ApiResponse> GetList();
        Task<ApiResponse> Save(PortfolioAllocationRuleDto model);
        Task<ApiResponse> Delete(int id);
    }
}

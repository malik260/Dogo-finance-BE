using DogoFinance.BusinessLogic.Layer.Response;
using DogoFinance.BusinessLogic.Layer.Models.Request;
using DogoFinance.DataAccess.Layer.DTO;
using System.Threading.Tasks;

namespace DogoFinance.ProductManagement.Interfaces
{
    public interface IPortfolioTypeService
    {
        Task<ApiResponse> GetList();
        Task<ApiResponse> Save(PortfolioTypeDto model);
        Task<ApiResponse> Delete(int id);
    }
}

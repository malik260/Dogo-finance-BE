using DogoFinance.BusinessLogic.Layer.Response;
using DogoFinance.BusinessLogic.Layer.Models.Request;
using System.Threading.Tasks;

namespace DogoFinance.ProductManagement.Interfaces
{
    public interface IPortfolioInstrumentService
    {
        Task<ApiResponse> GetList();
        Task<ApiResponse> Save(PortfolioInstrumentDto model);
        Task<ApiResponse> Delete(int id);
    }
}

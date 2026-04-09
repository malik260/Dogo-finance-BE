using DogoFinance.BusinessLogic.Layer.Response;
using DogoFinance.BusinessLogic.Layer.Models.Request;
using System.Threading.Tasks;

namespace DogoFinance.ProductManagement.Interfaces
{
    public interface IInstrumentService
    {
        Task<ApiResponse> GetList();
        Task<ApiResponse> Save(InstrumentDto model);
        Task<ApiResponse> Delete(int id);
    }
}

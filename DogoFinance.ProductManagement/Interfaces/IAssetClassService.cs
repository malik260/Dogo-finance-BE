using DogoFinance.BusinessLogic.Layer.Response;
using DogoFinance.BusinessLogic.Layer.Models.Request;
using DogoFinance.DataAccess.Layer.DTO;
using System.Threading.Tasks;

namespace DogoFinance.ProductManagement.Interfaces
{
    public interface IAssetClassService
    {
        Task<ApiResponse> GetAssetClasses();
        Task<ApiResponse> SaveAssetClass(AssetClassDto model);
        Task<ApiResponse> DeleteAssetClass(int id);
    }
}

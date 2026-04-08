using DogoFinance.DataAccess.Layer.Models.Entities;

namespace DogoFinance.DataAccess.Layer.Interfaces.Repositories
{
    public interface ISystemSettingRepository
    {
        Task<TblSystemSetting?> GetSystemSetting();
        Task UpdateSystemSetting(TblSystemSetting setting);
    }
}

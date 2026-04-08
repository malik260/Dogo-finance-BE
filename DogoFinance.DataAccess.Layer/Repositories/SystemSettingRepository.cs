using DogoFinance.DataAccess.Layer.Interfaces.Repositories;
using DogoFinance.DataAccess.Layer.Models.Entities;
using DogoFinance.DataAccess.Layer.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace DogoFinance.DataAccess.Layer.Repositories
{
    public class SystemSettingRepository : DataRepository, ISystemSettingRepository
    {
        public async Task<TblSystemSetting?> GetSystemSetting()
        {
            return await BaseRepository().FindList<TblSystemSetting>().Result.FirstOrDefaultAsync();
        }

        public async Task UpdateSystemSetting(TblSystemSetting setting)
        {
            await BaseRepository().Update(setting);
        }
    }
}

using DogoFinance.DataAccess.Layer.Interfaces.Repositories;
using DogoFinance.DataAccess.Layer.Models.Entities;
using DogoFinance.DataAccess.Layer.Repositories.Base;

namespace DogoFinance.DataAccess.Layer.Repositories
{
    public class NextOfKinRepository : DataRepository, INextOfKinRepository
    {
        public async Task<IEnumerable<TblNextOfKin>> GetByCustomerId(long customerId)
            => await BaseRepository().FindList<TblNextOfKin>(n => n.CustomerId == customerId);

        public async Task SaveNextOfKin(TblNextOfKin nok)
        {
            if (nok.Id == 0)
            {
                nok.CreatedAt = DateTime.UtcNow;
                await BaseRepository().Insert(nok);
            }
            else
            {
                nok.ModifiedAt = DateTime.UtcNow;
                await BaseRepository().Update(nok);
            }
        }
    }
}

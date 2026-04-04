using DogoFinance.DataAccess.Layer.Models.Entities;

namespace DogoFinance.DataAccess.Layer.Interfaces.Repositories
{
    public interface INextOfKinRepository
    {
        Task SaveNextOfKin(TblNextOfKin nok);
        Task<IEnumerable<TblNextOfKin>> GetByCustomerId(long customerId);
    }
}

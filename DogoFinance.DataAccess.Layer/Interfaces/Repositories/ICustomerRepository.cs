using DogoFinance.DataAccess.Layer.Models.Entities;

namespace DogoFinance.DataAccess.Layer.Interfaces.Repositories
{
    public interface ICustomerRepository
    {
        Task<TblCustomer?> GetCustomerDetailed(long customerId);
        Task<TblCustomer?> GetByUserId(long userId);
        Task SaveCustomer(TblCustomer customer);
    }
}

using DogoFinance.DataAccess.Layer.Models.Entities;

namespace DogoFinance.DataAccess.Layer.Interfaces.Repositories
{
    public interface IPaymentRepository
    {
        Task<TblPayment?> GetByReference(string reference);
        Task SavePayment(TblPayment payment);
    }
}

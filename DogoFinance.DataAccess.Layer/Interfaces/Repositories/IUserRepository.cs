using DogoFinance.DataAccess.Layer.Models.Entities;

namespace DogoFinance.DataAccess.Layer.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task<TblUser?> GetById(long id);
        Task<TblUser?> GetByEmail(string email);
        Task<TblUser?> GetByPhoneNumber(string phoneNumber);
        Task SaveUser(TblUser user);
    }
}

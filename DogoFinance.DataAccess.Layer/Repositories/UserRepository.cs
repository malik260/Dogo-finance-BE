using DogoFinance.DataAccess.Layer.Interfaces.Repositories;
using DogoFinance.DataAccess.Layer.Models.Entities;
using DogoFinance.DataAccess.Layer.Repositories.Base;

namespace DogoFinance.DataAccess.Layer.Repositories
{
    public class UserRepository : DataRepository, IUserRepository
    {
        public async Task<TblUser?> GetById(long id)
            => await BaseRepository().FindEntity<TblUser>(id);

        public async Task<TblUser?> GetByEmail(string email)
            => await BaseRepository().FindEntity<TblUser>(u => u.Email == email && !u.IsDeleted);

        public async Task<TblUser?> GetByPhoneNumber(string phoneNumber)
            => await BaseRepository().FindEntity<TblUser>(u => u.PhoneNumber == phoneNumber && !u.IsDeleted);

        public async Task SaveUser(TblUser user)
        {
            try
            {
                if (user.UserId == 0)
                {
                    user.CreatedAt = DateTime.UtcNow;
                    await BaseRepository().Insert(user);
                }
                else
                {
                    user.ModifiedAt = DateTime.UtcNow;
                    await BaseRepository().Update(user);
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}

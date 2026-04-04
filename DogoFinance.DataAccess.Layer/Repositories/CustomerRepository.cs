using DogoFinance.DataAccess.Layer.Interfaces.Repositories;
using DogoFinance.DataAccess.Layer.Models.Entities;
using DogoFinance.DataAccess.Layer.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace DogoFinance.DataAccess.Layer.Repositories
{
    public class CustomerRepository : DataRepository, ICustomerRepository
    {
        public async Task<TblCustomer?> GetCustomerDetailed(long customerId)
        {
            return await BaseRepository().AsQueryable<TblCustomer>(x => x.CustomerId == customerId)
                .Include(x => x.User)
                .Include(x => x.TblNextOfKins)
                .FirstOrDefaultAsync();
        }

        public async Task<TblCustomer?> GetByUserId(long userId)
        {
            return await BaseRepository().FindEntity<TblCustomer>(x => x.UserId == userId);
        }

        public async Task SaveCustomer(TblCustomer customer)
        {
            if (customer.CustomerId == 0)
            {
                customer.CreatedAt = DateTime.UtcNow;
                await BaseRepository().Insert(customer);
            }
            else
            {
                customer.ModifiedAt = DateTime.UtcNow;
                await BaseRepository().Update(customer);
            }
        }
    }
}

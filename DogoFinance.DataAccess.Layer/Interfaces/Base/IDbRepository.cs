using System.Data;
using System.Data.Common;
using System.Linq.Expressions;

namespace DogoFinance.DataAccess.Layer.Interfaces.Base
{
    /// <summary>
    /// Generic repository contract — mirrors Fintrak's IDbRepository.
    /// All concrete repositories inherit from <see cref="DbRepository"/> which implements this.
    /// </summary>
    public interface IDbRepository
    {
        // ── Transaction ───────────────────────────────────────────────────
        Task<IDbRepository> BeginTrans();
        Task<int> CommitTrans();
        Task<int> SaveChanges();
        Task RollbackTrans();
        Task Close();

        // ── Raw SQL ───────────────────────────────────────────────────────
        Task<int> ExecuteBySql(string sql);
        Task<int> ExecuteBySql(string sql, params DbParameter[] parameters);
        Task<int> ExecuteAsync(string sql, object? parameters = null);
        Task<IEnumerable<T>> QueryAsync<T>(string sql, object? parameters = null) where T : class;

        // ── Insert ────────────────────────────────────────────────────────
        Task<int> Insert<T>(T entity) where T : class;
        Task<int> Insert<T>(IEnumerable<T> entities) where T : class;

        // ── Delete ────────────────────────────────────────────────────────
        Task<int> Delete<T>(T entity) where T : class;
        Task<int> Delete<T>(IEnumerable<T> entities) where T : class;
        Task<int> Delete<T>(Expression<Func<T, bool>> condition) where T : class, new();
        Task<int> Delete<T>(int id) where T : class;
        Task<int> Delete<T>(long id) where T : class;
        Task<int> Delete<T>(long[] ids) where T : class;

        // ── Update ────────────────────────────────────────────────────────
        Task<int> Update<T>(T entity) where T : class;
        Task<int> Update<T>(IEnumerable<T> entities) where T : class;
        Task<int> UpdateAllFields<T>(T entity) where T : class;

        // ── Query ─────────────────────────────────────────────────────────
        IQueryable<T> AsQueryable<T>(Expression<Func<T, bool>> condition = null) where T : class, new();
        Task<T?> FindEntity<T>(object keyValue) where T : class;
        Task<T?> FindEntity<T>(Expression<Func<T, bool>> condition) where T : class, new();

        Task<IEnumerable<T>> FindList<T>(bool asNoTracking = false) where T : class, new();
        Task<IEnumerable<T>> FindList<T>(Expression<Func<T, bool>> condition) where T : class, new();
        Task<IEnumerable<T>> FindList<T>(string sql) where T : class;
        Task<IEnumerable<T>> FindList<T>(string sql, DbParameter[] parameters) where T : class;

        Task<(int total, IEnumerable<T> list)> FindList<T>(
            Expression<Func<T, bool>> condition, string sort, bool isAsc,
            int pageSize, int pageIndex) where T : class, new();

        Task<DataTable> FindTable(string sql);
        Task<DataTable> FindTable(string sql, DbParameter[] parameters);

        Task<object?> FindObject(string sql);
        Task<T?> FindObject<T>(string sql) where T : class;
    }
}

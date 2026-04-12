using DogoFinance.DataAccess.Layer.Interfaces.Base;
using DogoFinance.DataAccess.Layer.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Collections;
using System.Data;
using System.Data.Common;
using System.Linq.Expressions;
using System.Reflection;

namespace DogoFinance.DataAccess.Layer.Repositories.Base
{
    /// <summary>
    /// Concrete implementation of <see cref="IDbRepository"/> backed by <see cref="DogoDbContext"/>.
    /// Mirrors Fintrak's DbRepository, simplified for a single-context app.
    /// </summary>
    public class DbRepository : IDbRepository, IDisposable
    {
        protected readonly DogoDbContext _context;
        private IDbContextTransaction? _transaction;

        public DbRepository(DogoDbContext context)
        {
            _context = context;
        }

        // ── Transaction ───────────────────────────────────────────────────

        public async Task<IDbRepository> BeginTrans()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
            return this;
        }

        public async Task<int> CommitTrans()
        {
            try
            {
                var rows = await _context.SaveChangesAsync();
                if (_transaction != null)
                    await _transaction.CommitAsync();
                return rows;
            }
            catch
            {
                if (_transaction != null)
                    await _transaction.RollbackAsync();
                throw;
            }
            finally
            {
                await Close();
            }
        }

        public async Task<int> SaveChanges()
            => await _context.SaveChangesAsync();

        public async Task RollbackTrans()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
            }
        }

        public async Task Close()
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        // ── Raw SQL ───────────────────────────────────────────────────────

        public async Task<int> ExecuteBySql(string sql)
            => await _context.Database.ExecuteSqlRawAsync(sql);

        public async Task<int> ExecuteBySql(string sql, params DbParameter[] parameters)
            => await _context.Database.ExecuteSqlRawAsync(sql, parameters);

        public async Task<int> ExecuteAsync(string sql, object? parameters = null)
        {
            if (string.IsNullOrWhiteSpace(sql))
                throw new ArgumentNullException(nameof(sql));

            if (parameters == null)
                return await _context.Database.ExecuteSqlRawAsync(sql);

            var dbParams = parameters.GetType()
                .GetProperties()
                .Select(p => new Microsoft.Data.SqlClient.SqlParameter(
                    $"@{p.Name}", p.GetValue(parameters) ?? DBNull.Value))
                .ToArray();

            return await _context.Database.ExecuteSqlRawAsync(sql, dbParams);
        }

        public async Task<IEnumerable<T>> QueryAsync<T>(string sql, object? parameters = null)
            where T : class
        {
            // Uses raw ADO.NET reader via the existing connection
            var conn = _context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open)
                await conn.OpenAsync();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = sql;

            if (parameters != null)
            {
                foreach (var prop in parameters.GetType().GetProperties())
                {
                    var param = cmd.CreateParameter();
                    param.ParameterName = $"@{prop.Name}";
                    param.Value = prop.GetValue(parameters) ?? DBNull.Value;
                    cmd.Parameters.Add(param);
                }
            }

            using var reader = await cmd.ExecuteReaderAsync();
            var results = new List<T>();
            var props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            while (await reader.ReadAsync())
            {
                var item = Activator.CreateInstance<T>();
                foreach (var prop in props)
                {
                    if (!reader.HasColumn(prop.Name)) continue;
                    var val = reader[prop.Name];
                    if (val != DBNull.Value)
                        prop.SetValue(item, Convert.ChangeType(val, prop.PropertyType));
                }
                results.Add(item);
            }
            return results;
        }

        // ── Insert ────────────────────────────────────────────────────────

        public async Task<int> Insert<T>(T entity) where T : class
        {
            _context.Set<T>().Add(entity);
            return _transaction != null
                ? await _context.SaveChangesAsync()
                : await CommitTrans();
        }

        public async Task<int> Insert<T>(IEnumerable<T> entities) where T : class
        {
            await _context.Set<T>().AddRangeAsync(entities);
            return await _context.SaveChangesAsync();
        }

        // ── Delete ────────────────────────────────────────────────────────

        public async Task<int> Delete<T>(T entity) where T : class
        {
            _context.Set<T>().Remove(entity);
            return _transaction != null
                ? await _context.SaveChangesAsync()
                : await CommitTrans();
        }

        public async Task<int> Delete<T>(IEnumerable<T> entities) where T : class
        {
            _context.Set<T>().RemoveRange(entities);
            return _transaction != null
                ? await _context.SaveChangesAsync()
                : await CommitTrans();
        }

        public async Task<int> Delete<T>(Expression<Func<T, bool>> condition) where T : class, new()
        {
            var entities = await _context.Set<T>().Where(condition).ToListAsync();
            if (!entities.Any()) return 0;
            return await Delete(entities);
        }

        public async Task<int> Delete<T>(int id) where T : class
        {
            var entity = await _context.Set<T>().FindAsync(id);
            if (entity == null) return 0;
            return await Delete(entity);
        }

        public async Task<int> Delete<T>(long id) where T : class
        {
            var entity = await _context.Set<T>().FindAsync(id);
            if (entity == null) return 0;
            return await Delete(entity);
        }

        public async Task<int> Delete<T>(long[] ids) where T : class
        {
            foreach (var id in ids)
            {
                var entity = await _context.Set<T>().FindAsync(id);
                if (entity != null)
                    _context.Set<T>().Remove(entity);
            }
            return await _context.SaveChangesAsync();
        }

        // ── Update ────────────────────────────────────────────────────────

        public async Task<int> Update<T>(T entity) where T : class
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            _context.Set<T>().Update(entity);
            return _transaction != null
                ? await _context.SaveChangesAsync()
                : await CommitTrans();
        }

        public async Task<int> Update<T>(IEnumerable<T> entities) where T : class
        {
            _context.Set<T>().UpdateRange(entities);
            return _transaction != null
                ? await _context.SaveChangesAsync()
                : await CommitTrans();
        }

        public async Task<int> UpdateAllFields<T>(T entity) where T : class
        {
            _context.Entry(entity).State = EntityState.Modified;
            return _transaction != null
                ? await _context.SaveChangesAsync()
                : await CommitTrans();
        }

        // ── Query ─────────────────────────────────────────────────────────

        public IQueryable<T> AsQueryable<T>(Expression<Func<T, bool>> condition = null)
            where T : class, new()
        {
            var query = _context.Set<T>().AsQueryable();
            return condition == null ? query : query.Where(condition);
        }

        public async Task<T?> FindEntity<T>(object keyValue) where T : class
            => await _context.Set<T>().FindAsync(keyValue);

        public async Task<T?> FindEntity<T>(Expression<Func<T, bool>> condition)
            where T : class, new()
            => await _context.Set<T>().FirstOrDefaultAsync(condition);

        public async Task<IEnumerable<T>> FindList<T>(bool asNoTracking = false)
            where T : class, new()
        {
            var query = _context.Set<T>().AsQueryable();
            if (asNoTracking) query = query.AsNoTracking();
            return await query.ToListAsync();
        }

        public async Task<IEnumerable<T>> FindList<T>(Expression<Func<T, bool>> condition)
            where T : class, new()
            => await _context.Set<T>().Where(condition).ToListAsync();

        public async Task<IEnumerable<T>> FindList<T>(string sql) where T : class
            => await _context.Set<T>().FromSqlRaw(sql).ToListAsync();

        public async Task<IEnumerable<T>> FindList<T>(string sql, DbParameter[] parameters)
            where T : class
            => await _context.Set<T>().FromSqlRaw(sql, parameters).ToListAsync();

        public async Task<(int total, IEnumerable<T> list)> FindList<T>(
            Expression<Func<T, bool>> condition, string sort, bool isAsc,
            int pageSize, int pageIndex) where T : class, new()
        {
            var query = _context.Set<T>().Where(condition);
            int total = await query.CountAsync();

            // Basic single-column sort via LINQ OrderBy string is not natively supported;
            // consumers should pass IQueryable directly for complex sorts.
            var list = await query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            return (total, list);
        }

        public async Task<DataTable> FindTable(string sql)
        {
            var conn = _context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open)
                await conn.OpenAsync();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = sql;
            using var reader = await cmd.ExecuteReaderAsync();

            var table = new DataTable();
            table.Load(reader);
            return table;
        }

        public async Task<DataTable> FindTable(string sql, DbParameter[] parameters)
        {
            var conn = _context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open)
                await conn.OpenAsync();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = sql;
            cmd.Parameters.AddRange(parameters);
            using var reader = await cmd.ExecuteReaderAsync();

            var table = new DataTable();
            table.Load(reader);
            return table;
        }

        public async Task<object?> FindObject(string sql)
        {
            var conn = _context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open)
                await conn.OpenAsync();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = sql;
            return await cmd.ExecuteScalarAsync();
        }

        public async Task<T?> FindObject<T>(string sql) where T : class
        {
            var result = await FindObject(sql);
            return result as T;
        }

        // ── IDisposable ───────────────────────────────────────────────────

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
    }

    // Helper extension for DataReader
    internal static class DataReaderExtensions
    {
        public static bool HasColumn(this DbDataReader reader, string columnName)
        {
            for (int i = 0; i < reader.FieldCount; i++)
                if (reader.GetName(i).Equals(columnName, StringComparison.OrdinalIgnoreCase))
                    return true;
            return false;
        }
    }
}

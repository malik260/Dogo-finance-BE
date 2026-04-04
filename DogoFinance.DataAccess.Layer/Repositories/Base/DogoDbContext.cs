using DogoFinance.DataAccess.Layer.Enums;
using DogoFinance.DataAccess.Layer.Global;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace DogoFinance.DataAccess.Layer.Repositories.Base
{
    /// <summary>
    /// Base EF Core DbContext that auto-discovers all entity types decorated with
    /// <see cref="TableAttribute"/> from loaded assemblies — mirroring the Fintrak pattern.
    /// </summary>
    public class DogoDbContext : DbContext
    {
        private readonly DatabaseProvider _dbType;
        private readonly string? _connectionString;
        private readonly int _timeoutSeconds;

        public DogoDbContext()
        {
            _dbType           = DbFactory.Type;
            _connectionString = DbFactory.Connect;
            _timeoutSeconds   = DbFactory.TimeoutSeconds;
        }

        public DogoDbContext(DatabaseProvider dbType, string connectionString, int timeoutSeconds = 30)
        {
            _dbType           = dbType;
            _connectionString = connectionString;
            _timeoutSeconds   = timeoutSeconds;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            switch (_dbType)
            {
                case DatabaseProvider.SqlServer:
                    options.UseSqlServer(_connectionString,
                        o => o.CommandTimeout(_timeoutSeconds));
                    break;

/*
                case DatabaseProvider.MySql:
                    options.UseMySql(_connectionString,
                        ServerVersion.AutoDetect(_connectionString),
                        o => o.CommandTimeout(_timeoutSeconds));
                    break;
*/

                // Oracle / others can be added here when needed
                default:
                    options.UseSqlServer(_connectionString,
                        o => o.CommandTimeout(_timeoutSeconds));
                    break;
            }

            base.OnConfiguring(options);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Auto-discover all entity types marked with [Table] from loaded DLLs
            var runPath = GlobalConstant.GetRunPath;
            var dlls = new DirectoryInfo(runPath).GetFiles("*.dll");

            foreach (var dll in dlls)
            {
                try
                {
                    if (dll.Name.StartsWith("Microsoft") || dll.Name.StartsWith("System")) continue;
                    var assembly = Assembly.Load(AssemblyName.GetAssemblyName(dll.FullName));
                    var entityTypes = assembly.GetTypes()
                        .Where(t => t.Namespace != null &&
                                    t.GetCustomAttribute<TableAttribute>() != null);

                    foreach (var type in entityTypes)
                    {
                        if (modelBuilder.Model.FindEntityType(type) == null)
                        {
                            var entity = modelBuilder.Model.AddEntityType(type);
                            // Auto-set RowVersion if found as byte[] and named RowVersion
                            var rowVersionProp = type.GetProperty("RowVersion");
                            if (rowVersionProp != null && rowVersionProp.PropertyType == typeof(byte[]))
                            {
                                modelBuilder.Entity(type).Property("RowVersion").IsRowVersion();
                            }
                        }
                    }
                }
                catch { /* silently skip incompatible assemblies */ }
            }

            // Standardise decimal precision globally
            var decimalProps = modelBuilder.Model.GetEntityTypes()
                .SelectMany(t => t.GetProperties())
                .Where(p => (Nullable.GetUnderlyingType(p.ClrType) ?? p.ClrType) == typeof(decimal));

            foreach (var prop in decimalProps)
            {
                prop.SetPrecision(18);
                prop.SetScale(2);
            }

            base.OnModelCreating(modelBuilder);
        }
    }
}

using DogoFinance.DataAccess.Layer.Enums;
using DogoFinance.DataAccess.Layer.Global;

namespace DogoFinance.DataAccess.Layer.Repositories.Base
{
    /// <summary>
    /// Reads DB provider / connection string / timeout from GlobalContext (set per request).
    /// </summary>
    public static class DbFactory
    {
        public static DatabaseProvider Type
        {
            get
            {
                var provider = GlobalContext.Provider?.ToLower() ?? "sqlserver";
                return provider switch
                {
                    "mssql"     => DatabaseProvider.SqlServer,
                    "sqlserver" => DatabaseProvider.SqlServer,
                    "mysql"     => DatabaseProvider.MySql,
                    "oracle"    => DatabaseProvider.Oracle,
                    _           => DatabaseProvider.SqlServer  // safe default
                };
            }
        }

        public static string? Connect => GlobalContext.ConnectionString;

        public static int TimeoutSeconds => GlobalContext.CommandTimeoutSeconds > 0
            ? GlobalContext.CommandTimeoutSeconds
            : 30;
    }
}

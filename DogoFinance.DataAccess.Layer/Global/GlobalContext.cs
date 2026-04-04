using DogoFinance.DataAccess.Layer.Models.Base;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DogoFinance.DataAccess.Layer.Global
{
    /// <summary>
    /// Per-request, thread-safe global context powered by AsyncLocal.
    /// Holds DB connection settings and current user context injected per HTTP request.
    /// </summary>
    public static class GlobalContext
    {
        // ── Shared singletons ──────────────────────────────────────────────
        public static IServiceCollection? Services { get; set; }
        public static IServiceProvider? ServiceProvider { get; set; }
        public static IWebHostEnvironment? HostingEnvironment { get; set; }

        // ── Per-request state (AsyncLocal) ─────────────────────────────────
        private static readonly AsyncLocal<GlobalContextState> _state = new();
        private static GlobalContextState State => _state.Value ??= new GlobalContextState();

        public static string? ConnectionString
        {
            get => State.ConnectionString ?? SystemConfig?.DBConnectionString;
            set => State.ConnectionString = value;
        }

        public static string? Provider
        {
            get => State.Provider ?? SystemConfig?.DBProvider;
            set => State.Provider = value;
        }

        public static int CommandTimeoutSeconds
        {
            get => State.CommandTimeoutSeconds;
            set => State.CommandTimeoutSeconds = value;
        }

        public static OperatorInfo? OperatorInfo
        {
            get => State.OperatorInfo;
            set => State.OperatorInfo = value;
        }

        // ── Configuration & SystemConfig ───────────────────────────────────
        private static IConfiguration? _configuration;

        public static IConfiguration? Configuration
        {
            get
            {
                if (_configuration is null)
                    SetConfigFiles("appsettings.json");
                return _configuration;
            }
            set => _configuration = value;
        }

        private static SystemConfig? _systemConfig;

        public static SystemConfig? SystemConfig
        {
            get
            {
                if (_systemConfig is null)
                {
                    _systemConfig = Configuration?.GetSection("SystemConfig").Get<SystemConfig>() ?? new SystemConfig();
                }
                return _systemConfig;
            }
        }

        public static void SetConfigFiles(params string[] fileNames)
        {
            var dir = GlobalConstant.GetRunPath;
            foreach (var f in fileNames)
            {
                if (!File.Exists(Path.Combine(dir, f))) dir = GlobalConstant.GetRunPath2;
                if (!File.Exists(Path.Combine(dir, f))) dir = GlobalConstant.GetRunPath3;
                if (!File.Exists(Path.Combine(dir, f))) dir = GlobalConstant.GetRunPath4;
                if (!File.Exists(Path.Combine(dir, f)))
                    throw new FileNotFoundException($"Cannot find configuration file: {f}");
            }

            var builder = new ConfigurationBuilder().SetBasePath(dir);
            foreach (var f in fileNames)
                builder.AddJsonFile(f, optional: false, reloadOnChange: true);

            _configuration = builder.Build();
            _systemConfig = null; // reset so it picks up change
        }
    }

    internal sealed class GlobalContextState
    {
        public string? ConnectionString { get; set; }
        public string? Provider { get; set; }
        public int CommandTimeoutSeconds { get; set; } = 30;
        public OperatorInfo? OperatorInfo { get; set; }
    }
}

namespace DogoFinance.DataAccess.Layer.Models.Base
{
    /// <summary>
    /// System-wide configuration options, loaded from appsettings.json.
    /// </summary>
    public class SystemConfig
    {
        public bool LoginMultiple { get; set; }
        public string? LoginProvider { get; set; }
        public int SnowFlakeWorkerId { get; set; }
        public string? JWTSecret { get; set; }
        public string? ApiSite { get; set; }
        public string? AllowCorsSite { get; set; }
        public string? VirtualDirectory { get; set; }
        public string? DBProvider { get; set; }
        public string? DBConnectionString { get; set; }
        public int DBCommandTimeout { get; set; }
        public string? FrontendBaseUrl { get; set; }
    }
}

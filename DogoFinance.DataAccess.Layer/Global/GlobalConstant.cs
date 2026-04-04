namespace DogoFinance.DataAccess.Layer.Global
{
    public static class GlobalConstant
    {
        public static PlatformID SystemType => Environment.OSVersion.Platform;

        /// <summary>Path of the assembly directory (used to find config files).</summary>
        public static string GetRunPath =>
            Path.GetDirectoryName(typeof(GlobalConstant).Assembly.Location)!;

        public static string GetRunPath2 => AppDomain.CurrentDomain.BaseDirectory;
        public static string GetRunPath3 => Environment.CurrentDirectory;
        public static string GetRunPath4 => Directory.GetCurrentDirectory();

        public static DateTime DefaultTime => DateTime.Parse("1970-01-01 00:00:00");

        public static string NewGuid => Guid.NewGuid().ToString();
    }
}

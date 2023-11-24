using Serilog;

namespace Factory
{
    public class LoggerService
    {
        public static void InitLogService(string LogFolder = "")
        {
            if (string.IsNullOrEmpty(LogFolder))
                LogFolder = AppContext.BaseDirectory;


            var loggerConfig = new LoggerConfiguration();
            if (Global.GlobalConfig.Instance.LogLevel.Equals("debug", StringComparison.OrdinalIgnoreCase))
            {
                loggerConfig.MinimumLevel.Debug();
            }
            else if (Global.GlobalConfig.Instance.LogLevel.Equals("information", StringComparison.OrdinalIgnoreCase))
            {
                loggerConfig.MinimumLevel.Information();
            }
            else if (Global.GlobalConfig.Instance.LogLevel.Equals("warning", StringComparison.OrdinalIgnoreCase))
            {
                loggerConfig.MinimumLevel.Warning();
            }
            else
            {
                loggerConfig.MinimumLevel.Error();
            }
            loggerConfig.WriteTo.File(string.Format("{0}/logs/log_.txt", LogFolder), rollingInterval: RollingInterval.Day);
            loggerConfig.WriteTo.Console();

            Log.Logger = loggerConfig.CreateLogger();
        }
    }
}

using Serilog;

namespace Factory
{
    public class LoggerService
    {
        public static void InitLogService(string LogFolder = "")
        {
            if (string.IsNullOrEmpty(LogFolder))
                LogFolder = AppContext.BaseDirectory;

            Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.File(string.Format("{0}/logs/log_.txt", LogFolder), rollingInterval: RollingInterval.Day)
            .WriteTo.Console()
            .CreateLogger();
        }
    }
}

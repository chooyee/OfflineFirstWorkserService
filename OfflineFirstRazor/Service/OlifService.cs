using Factory.DB.Model;
using Factory;
using System.Diagnostics;

namespace Service
{
	public partial class OlifService: BackgroundService
	{
		public OlifService(ILoggerFactory loggerFactory)
		{
			Logger = loggerFactory.CreateLogger<OlifService>();
			//Init crytography keys
            CipherService.InitKey();
            Factory.DB.Init.InitDB.Init();
        }

		public ILogger Logger { get; }

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			Logger.LogInformation("OlifService is starting.");

			stoppingToken.Register(() => Logger.LogInformation("OlifService is stopping."));

			while (!stoppingToken.IsCancellationRequested)
			{
				Logger.LogInformation("OlifService is doing background work.");

				await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
			}

			Logger.LogInformation("OlifService has stopped.");
		}

        
    }
}

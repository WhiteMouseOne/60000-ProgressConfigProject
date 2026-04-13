using Progress.IService.Business;

namespace Progress.WebApi
{
    public class AlertBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<AlertBackgroundService> _log;

        public AlertBackgroundService(IServiceScopeFactory scopeFactory, ILogger<AlertBackgroundService> log)
        {
            _scopeFactory = scopeFactory;
            _log = log;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var timer = new PeriodicTimer(TimeSpan.FromMinutes(15));
            try
            {
                while (await timer.WaitForNextTickAsync(stoppingToken))
                {
                    try
                    {
                        await using var scope = _scopeFactory.CreateAsyncScope();
                        var scanner = scope.ServiceProvider.GetRequiredService<IAlertScanService>();
                        await scanner.RunOnceAsync(stoppingToken);
                    }
                    catch (Exception ex)
                    {
                        _log.LogError(ex, "Alert scan failed");
                    }
                }
            }
            catch (OperationCanceledException) { /* shutdown */ }
        }
    }
}

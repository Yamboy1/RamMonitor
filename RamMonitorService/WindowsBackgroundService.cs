namespace RamMonitor.WindowsService;
public sealed class WindowsBackgroundService : BackgroundService
{
    private readonly RamMonitorService _monitorService;
    private readonly ILogger<WindowsBackgroundService> _logger;

    public WindowsBackgroundService(
        RamMonitorService monitorService,
        ILogger<WindowsBackgroundService> logger) =>
        (_monitorService, _logger) = (monitorService, logger);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _monitorService.UpdateLeds();
                await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
            }
            _logger.LogInformation("Ram Monitor Stopped");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{Message}", ex.Message);

            // Terminates this process and returns an exit code to the operating system.
            // This is required to avoid the 'BackgroundServiceExceptionBehavior', which
            // performs one of two scenarios:
            // 1. When set to "Ignore": will do nothing at all, errors cause zombie services.
            // 2. When set to "StopHost": will cleanly stop the host, and log errors.
            //
            // In order for the Windows Service Management system to leverage configured
            // recovery options, we need to terminate the process with a non-zero exit code.
            Environment.Exit(1);
        }
    }
}
using RamMonitor.WindowsService;

using IHost host = Host.CreateDefaultBuilder(args)
    .UseWindowsService(options =>
    {
        options.ServiceName = "Ram Monitor Service";
    })
    .ConfigureServices(services =>
    {
        services.AddSingleton<RamMonitorService>();
        services.AddHostedService<WindowsBackgroundService>();
    })
    .Build();

await host.RunAsync();
using DownloadService;
using Coravel;
using Coravel.Scheduling.Schedule;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<DownloadAndParseFileService>();
    })
    .Build();
await host.RunAsync();

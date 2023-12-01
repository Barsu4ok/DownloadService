using DownloadService;
using Coravel;
using Coravel.Scheduling.Schedule;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddScheduler();
        services.AddTransient<DownloadAndParseFileService>();
    })
    .Build();

host.Services.UseScheduler(scheduler =>
{
    var downloadSchedule = scheduler.Schedule<DownloadAndParseFileService>();
    downloadSchedule.Weekly();
});


await host.RunAsync();

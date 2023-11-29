using DownloadService;
using Coravel;
using Coravel.Scheduling.Schedule;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        //services.AddHostedService<Worker>();
        services.AddScheduler();
        //services.AddTransient<ProcessOrder>();
        services.AddTransient<DownloadAndParseFileService>();
    })
    .Build();

host.Services.UseScheduler(scheduler =>
{
    var downloadSchedule = scheduler.Schedule<DownloadAndParseFileService>();
    downloadSchedule.EveryMinute();
});


await host.RunAsync();

using DownloadService;
using Coravel;
using Coravel.Scheduling.Schedule;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        //services.AddHostedService<Worker>();
        services.AddScheduler();
        services.AddTransient<ProcessOrder>();
    })
    .Build();

host.Services.UseScheduler(scheduler =>
{
    var jobSchedule = scheduler.Schedule<ProcessOrder>();
    jobSchedule.EverySeconds(5);
});


await host.RunAsync();

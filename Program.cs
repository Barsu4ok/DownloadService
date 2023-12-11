using DownloadService;
using Microsoft.Extensions.Options;
using DownloadService.Services.Interfaces;
using DownloadService.Services;
using DownloadService.Config;
using DownloadService.DataSources;
using DownloadService.DataAccess;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext,services) =>
    {
        IConfiguration configuration = hostContext.Configuration;
        services.Configure<WebConfig>(configuration.GetSection("WebConfig"));
        services.Configure<FileConfig>(configuration.GetSection("FileConfig"));
        services.Configure<TimerConfig>(configuration.GetSection("TimerConfig"));
        services.Configure<MySqlConnectionConfig>(configuration.GetSection("MySqlConfig"));
        services.AddHostedService<DownloadAndParseFileService>();
        services.AddSingleton<IOptionsMonitor<WebConfig>, OptionsMonitor<WebConfig>>();
        services.AddSingleton<IOptionsMonitor<FileConfig>, OptionsMonitor<FileConfig>>();
        services.AddSingleton<IOptionsMonitor<TimerConfig>, OptionsMonitor<TimerConfig>>();
        services.AddSingleton<IParseService, CellTowerParseService>();
        services.AddSingleton<Parser>();
        services.AddSingleton<IDataSource,WebDataSource>();
        //services.AddSingleton<IDataTarget, FileDataTarget>();
        services.AddSingleton<IDataTarget, DbDataTarget>();
        //services.AddSingleton<IDataSource, FileDataSource>();

    })
    .Build();
await host.RunAsync();

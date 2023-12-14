using DownloadService;
using Microsoft.Extensions.Options;
using DownloadService.Config;
using DownloadService.DataSources;
using DownloadService.DataAccess;
using DownloadService.Parser;
using DownloadService.Interfaces;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext,services) =>
    {
        IConfiguration configuration = hostContext.Configuration;
        services.Configure<WebConfig>(configuration.GetSection("WebConfig"));
        services.Configure<FileConfig>(configuration.GetSection("FileConfig"));
        services.Configure<TimerConfig>(configuration.GetSection("TimerConfig"));
        services.Configure<MySqlConnectionConfig>(configuration.GetSection("MySqlConfig"));
        services.AddHostedService<DownloadAndParseFileService>();
        services.AddSingleton<IParseService, CellTowerParseService>();
        //services.AddSingleton<IDataSource,WebDataSource>();
        //services.AddSingleton<IDataTarget, DbDataTarget>();
        services.AddSingleton<IDataSource, FileDataSource>();
        services.AddSingleton<IDataTarget, FileDataTarget>();

    })
    .Build();
await host.RunAsync();

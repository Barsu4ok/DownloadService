using DownloadService;
using DownloadService.Config;
using DownloadService.DataSources;
using DownloadService.DataAccess;
using DownloadService.Parser;
using DownloadService.Interfaces;
using DownloadService.Validators;
using FluentValidation;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext,services) =>
    {
        var configuration = hostContext.Configuration;
        services.Configure<WebConfig>(configuration.GetSection("WebConfig"));
        services.Configure<FileConfig>(configuration.GetSection("FileConfig"));
        services.Configure<TimerConfig>(configuration.GetSection("TimerConfig"));
        services.Configure<MySqlConnectionConfig>(configuration.GetSection("MySqlConfig"));
        services.AddSingleton<IValidator<FileConfig>, FileConfigValidator>();
        services.AddSingleton<IValidator<MySqlConnectionConfig>, MySqlConnectionValidator>();
        services.AddSingleton<IValidator<TimerConfig>, TimerConfigValidator>();
        services.AddSingleton<IValidator<WebConfig>, WebConfigValidator>();
        services.AddHostedService<DownloadAndParseFileService>();
        services.AddSingleton<IParseService, CellTowerParseService>();
        services.AddSingleton<IDataSource,WebDataSource>();
        services.AddSingleton<IDataTarget, MySqlDataTarget>();
        //services.AddSingleton<IDataSource, FileDataSource>();
        //services.AddSingleton<IDataTarget, FileDataTarget>();

    })
    .Build();
await host.RunAsync();

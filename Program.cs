using DownloadService;
using Microsoft.Extensions.Options;
using DownloadService.Services.Interfaces;
using DownloadService.Services;
using DownloadService.Config;
using DownloadService.DataSources;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext,services) =>
    {
        IConfiguration configuration = hostContext.Configuration;
        services.Configure<DownloadConfig>(configuration.GetSection(nameof(DownloadConfig)));
        services.AddHostedService<DownloadAndParseFileService>();
        services.AddSingleton<IParseService, CellTowerParseService>();
        services.AddSingleton<Parser>();
        services.AddSingleton<IDataSource>(provider =>
        {
            var config = provider.GetRequiredService<IOptions<DownloadConfig>>().Value;
            return new WebDataSource(config.uri);
        });

    })
    .Build();
await host.RunAsync();

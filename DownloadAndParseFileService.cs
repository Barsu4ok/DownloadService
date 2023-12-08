using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coravel;
using Coravel.Invocable;
using DownloadService.Services.Interfaces;
using DownloadService.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using DownloadService.Config;
using Microsoft.Extensions.Logging;
using DownloadService.DataSources;

namespace DownloadService
{
    class DownloadAndParseFileService : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<DownloadAndParseFileService> _logger;
        private readonly IDataSource _dataSource;
        private Parser _parser;
        public DownloadAndParseFileService(ILogger<DownloadAndParseFileService> logger,
            Parser parser, IDataSource dataSource, IServiceScopeFactory serviceScopeFactory)
        {
            _parser = parser;
            _logger = logger;
            _dataSource = dataSource;
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var currentConfig = scope.ServiceProvider.GetRequiredService<IOptionsSnapshot<DownloadConfig>>();
                //using PeriodicTimer timer = new PeriodicTimer(TimeSpan.Parse(currentConfig.Value.timeInterval));
                using PeriodicTimer timer = new PeriodicTimer(TimeSpan.Parse("00:00:00:5"));
                while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
                {
                    try
                    {
                        Stream dataSource = _dataSource.getDataSource();
                        _parser.parseFile(dataSource);
                        _logger.LogInformation("Success download and parse file");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogInformation($"Failed download and parse file! Description ERROR: {ex.Message}");
                    }
                }
            }
        }
    }
}

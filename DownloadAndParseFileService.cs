using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        private readonly IOptionsMonitor<TimerConfig> _timerConfig;
        private readonly ILogger<DownloadAndParseFileService> _logger;
        private readonly IDataSource _dataSource;
        private readonly IDataTarget _dataTarget;
        private Parser _parser;
        public DownloadAndParseFileService(ILogger<DownloadAndParseFileService> logger,
            Parser parser, IDataSource dataSource, IOptionsMonitor<TimerConfig> timerConfig,
            IDataTarget dataTarget)
        {
            _parser = parser;
            _logger = logger;
            _dataSource = dataSource;
            _timerConfig = timerConfig;
            _dataTarget = dataTarget;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //using PeriodicTimer timer = new PeriodicTimer(TimeSpan.Parse("00:00:00:10"));
            using PeriodicTimer timer = new PeriodicTimer(TimeSpan.Parse(_timerConfig.CurrentValue.timeInterval));
            while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
            {
                try
                {
                    Stream dataSource = await _dataSource.getDataSource();
                    _dataTarget.writeAllData(_parser.parseFile(dataSource));
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

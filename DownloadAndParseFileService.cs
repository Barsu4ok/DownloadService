using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using DownloadService.Config;
using Microsoft.Extensions.Logging;
using DownloadService.DataSources;
using DownloadService.DataAccess;
using DownloadService.Interfaces;
using DownloadService.Validators;
using MySqlX.XDevAPI.Common;
using FluentValidation;

namespace DownloadService
{
    class DownloadAndParseFileService : BackgroundService
    {
        private readonly IOptionsMonitor<TimerConfig> _timerConfig;
        private readonly ILogger<DownloadAndParseFileService> _logger;
        private readonly IValidator<TimerConfig> _timerConfigValidator;
        private readonly IDataSource _dataSource;
        private readonly IDataTarget _dataTarget;
        private readonly IParseService _parseService;
        public DownloadAndParseFileService(ILogger<DownloadAndParseFileService> logger,
            IParseService parseService, IDataSource dataSource, IOptionsMonitor<TimerConfig> timerConfig,
            IDataTarget dataTarget, IValidator<TimerConfig> timerConfigValidator)
        {
            _parseService = parseService;
            _logger = logger;
            _dataSource = dataSource;
            _timerConfig = timerConfig;
            _dataTarget = dataTarget;
            _timerConfigValidator = timerConfigValidator;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                var resultValidation = _timerConfigValidator.Validate(_timerConfig.CurrentValue);
                if (resultValidation.IsValid)
                {
                    using var timer = new PeriodicTimer(TimeSpan.FromMilliseconds(10000));
                    //using PeriodicTimer timer = new PeriodicTimer(TimeSpan.FromMilliseconds(_timerConfig.CurrentValue.timeInterval));
                    while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
                    {
                        await using var dataSource = await _dataSource.getDataSource();
                        _dataTarget.writeData(_parseService.parse(dataSource));
                        _logger.LogInformation("Success download and parse file");

                    }
                }
                else _logger.LogInformation("Incorrect time interval value");
            }
            catch(Exception e)
            {
                _logger.LogInformation(e.Message);
            }
            
        }
    }
}

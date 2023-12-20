using Microsoft.Extensions.Options;
using DownloadService.Config;
using DownloadService.Interfaces;
using FluentValidation;

namespace DownloadService
{
    internal class DownloadAndParseFileService : BackgroundService
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
                var resultValidation = await _timerConfigValidator.ValidateAsync(_timerConfig.CurrentValue, stoppingToken);
                if (resultValidation.IsValid)
                {
                    using var timer = new PeriodicTimer(TimeSpan.FromMilliseconds(10000));
                    //using PeriodicTimer timer = new PeriodicTimer(TimeSpan.FromMilliseconds(_timerConfig.CurrentValue.TimeInterval));
                    while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
                    {
                        _logger.LogInformation("Starting the parsing process ");
                        await using var dataSource = await _dataSource.GetDataSource();
                        _dataTarget.WriteData(_parseService.Parse(dataSource));
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

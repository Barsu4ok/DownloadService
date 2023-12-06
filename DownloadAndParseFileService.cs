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

namespace DownloadService
{
    class DownloadAndParseFileService : BackgroundService
    {
        private readonly DownloadConfig _configuration;
        private readonly ILogger<DownloadAndParseFileService> logger;
        private readonly TimeSpan period = TimeSpan.FromSeconds(2);
        private Parser? parser;
        public DownloadAndParseFileService(ILogger<DownloadAndParseFileService> logger,
            Parser? parser,IOptions<DownloadConfig> configuration)
        {
            this.parser = parser;
            this.logger = logger;
            _configuration = configuration.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using PeriodicTimer timer = new PeriodicTimer(period);
            while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
            {
                try
                {
                    parser.parseFile(_configuration.uri, _configuration.outputPath);
                    logger.LogInformation("Success download and parse file");   
                }
                catch(Exception ex)
                {
                    logger.LogInformation($"Failed download and parse file! Description ERROR: {ex.Message}");
                }
            }
    
        }
    }
}

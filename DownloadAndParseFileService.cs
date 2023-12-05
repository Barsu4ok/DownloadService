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

namespace DownloadService
{
    class DownloadAndParseFileService : BackgroundService
    {
        private readonly ILogger<DownloadAndParseFileService> logger;
        private readonly TimeSpan period = TimeSpan.FromDays(7);
        private Parser? parser;
        private readonly string outputPath = "D:\\Learning\\DownloadService\\DownloadFiles\\result.txt";
        private readonly string uri = "https://drive.google.com/uc?export=download&id=1ZQBgouAZ5pfHkleQLNRKquTxrQqDDiN7";
        public DownloadAndParseFileService(ILogger<DownloadAndParseFileService> logger)
        {
            var services = new ServiceCollection()
                .AddTransient<IParseService, CellTowerParseService>()
                .AddTransient<Parser>();
            using var serviceProvider = services.BuildServiceProvider();
            parser = serviceProvider.GetService<Parser>();
            this.logger = logger;

        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using PeriodicTimer timer = new PeriodicTimer(period);
            while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
            {
                try
                {
                 if(parser != null)
                 {
                  parser.parseFile(uri, outputPath);
                  logger.LogInformation("Success download and parse file");
                 }   
                }
                catch(Exception ex)
                {
                    logger.LogInformation($"Failed download and parse file! Description ERROR: {ex.Message}");
                }
            }
    
        }
    }
}

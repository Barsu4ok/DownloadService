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
    class DownloadAndParseFileService : IInvocable
    {
        private Parser parser;
        private string outputPath = "D:\\Learning\\DownloadService\\DownloadFiles\\result.txt";
        private string uri = "https://drive.google.com/uc?export=download&id=1ZQBgouAZ5pfHkleQLNRKquTxrQqDDiN7";
        private readonly ILogger<ProcessOrder> logger;
        public DownloadAndParseFileService(ILogger<ProcessOrder> logger)
        {
            var services = new ServiceCollection()
                .AddTransient<IParseService, CellTowerParseService>()
                .AddTransient<IDownloadService, DownloadFileService>()
                .AddTransient<Parser>();
            using var serviceProvider = services.BuildServiceProvider();
            parser = serviceProvider.GetService<Parser>();
            this.logger = logger;

        }

        public Task Invoke()
        {
            logger.LogInformation("Starting the process of downloading and parsing the file");
            parser.parseFile(uri,outputPath);
            logger.LogInformation("Finishing the process of downloading and parsing the file");
            return Task.FromResult(true);
        }
    }
}

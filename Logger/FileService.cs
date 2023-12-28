using DownloadService.Config;
using DownloadService.Interfaces;
using Microsoft.Extensions.Options;

namespace DownloadService.Logger
{
    public sealed class FileService : IFileService
    {
        private readonly IOptionsMonitor<LoggerConfig> _loggerConfig;

        public FileService(IOptionsMonitor<LoggerConfig> loggerConfig)
        {
            _loggerConfig = loggerConfig;
        }
        public async Task WriteLog(string message)
        {
            using var sw = new StreamWriter(_loggerConfig.CurrentValue.PathFromLogFile, true,
                System.Text.Encoding.Default);
            await sw.WriteLineAsync(message);
        }
    }
}

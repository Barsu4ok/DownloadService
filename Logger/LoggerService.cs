using DownloadService.Interfaces;

namespace DownloadService.Logger
{
    public sealed class LoggerService : ILoggerService
    {
        private readonly IFileService _fileService;

        public LoggerService(IFileService fileService)
        {
            _fileService = fileService;
        }

        public void Log(LogLevel level, string message)
        {
            _fileService.WriteLog($"{DateTime.Now}|{level}|{message}");
        }
    }
}

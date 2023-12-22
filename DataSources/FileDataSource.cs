using DownloadService.Config;
using DownloadService.Interfaces;
using FluentValidation;
using Microsoft.Extensions.Options;
using System.IO.Compression;
namespace DownloadService.DataSources
{
    public class FileDataSource : IDataSource
    {
        private readonly IOptionsMonitor<FileConfig> _fileConfig;
        private readonly IValidator<FileConfig> _fileConfigValidator;
        private readonly ILoggerService _logger;

        public FileDataSource(ILoggerService logger,IOptionsMonitor<FileConfig> fileConfig, IValidator<FileConfig> fileConfigValidator)
        {
            _logger = logger;
            _fileConfig = fileConfig;
            _fileConfigValidator = fileConfigValidator;
        }

        public async Task<Stream> GetDataSource()
        {
            var resultValidation = await _fileConfigValidator.ValidateAsync(_fileConfig.CurrentValue);
            if (!resultValidation.IsValid)
            {
                _logger.Log(LogLevel.Error,$"Validation failed: {resultValidation.Errors}");
            }

            await using var fileStream = File.OpenRead(_fileConfig.CurrentValue.InputFilePath ?? throw new InvalidOperationException());
            await using var zipStream = new GZipStream(fileStream, CompressionMode.Decompress);
            var memoryStream = new MemoryStream();
            await zipStream.CopyToAsync(memoryStream);
            memoryStream.Seek(0, SeekOrigin.Begin);
            _logger.Log(LogLevel.Information, "Successful retrieval of data from the archive");
            return memoryStream;
        }
    }
}

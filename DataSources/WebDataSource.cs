using DownloadService.Config;
using DownloadService.Interfaces;
using FluentValidation;
using Microsoft.Extensions.Options;
using System.IO.Compression;

namespace DownloadService.DataSources
{
    public class WebDataSource : IDataSource
    {
        private readonly IOptionsMonitor<WebConfig> _webConfig;
        private readonly IValidator<WebConfig> _webConfigValidator;
        private readonly ILoggerService _logger;

        public WebDataSource(ILoggerService logger, IOptionsMonitor<WebConfig> webConfig, IValidator<WebConfig> webConfigValidator)
        {
            _logger = logger;
            _webConfig = webConfig;
            _webConfigValidator = webConfigValidator;
        }

        public async Task<Stream> GetDataSource()
        {

            var resultValidation = await _webConfigValidator.ValidateAsync(_webConfig.CurrentValue);
            using var httpClient = new HttpClient();
            if (!resultValidation.IsValid) 
            {
                _logger.Log(LogLevel.Error,$"Validation failed: {resultValidation.Errors}");
            }
            var response = await httpClient.GetAsync(_webConfig.CurrentValue.Uri);
            if (!response.IsSuccessStatusCode)
            {
                _logger.Log(LogLevel.Error,$"HTTP request failed with status code: {response.StatusCode}");
            }

            await using var gzipStream = await response.Content.ReadAsStreamAsync();
            await using var decompressionStream = new GZipStream(gzipStream, CompressionMode.Decompress);
            var memoryStream = new MemoryStream();
            await decompressionStream.CopyToAsync(memoryStream);
            memoryStream.Seek(0, SeekOrigin.Begin);
            _logger.Log(LogLevel.Information, "Successfully downloading the archive and retrieving its contents");
            return memoryStream;
        }
    }
}

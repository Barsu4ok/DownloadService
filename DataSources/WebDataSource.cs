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

        public WebDataSource(IOptionsMonitor<WebConfig> webConfig, IValidator<WebConfig> webConfigValidator)
        {
            _webConfig = webConfig;
            _webConfigValidator = webConfigValidator;
        }

        public async Task<Stream> GetDataSource()
        {

            var resultValidation = await _webConfigValidator.ValidateAsync(_webConfig.CurrentValue);
            using var httpClient = new HttpClient();
            if (!resultValidation.IsValid) 
            {
                throw new ValidationException($"Validation failed: {resultValidation.Errors}");
            }
            var response = await httpClient.GetAsync(_webConfig.CurrentValue.Uri);
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"HTTP request failed with status code: {response.StatusCode}");
            }

            await using var gzipStream = await response.Content.ReadAsStreamAsync();
            await using var decompressionStream = new GZipStream(gzipStream, CompressionMode.Decompress);
            var memoryStream = new MemoryStream();
            await decompressionStream.CopyToAsync(memoryStream);
            memoryStream.Seek(0, SeekOrigin.Begin);
            return memoryStream;
        }
    }
}

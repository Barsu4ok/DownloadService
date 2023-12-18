using DownloadService.Config;
using DownloadService.Interfaces;
using DownloadService.Validators;
using FluentValidation;
using Microsoft.Extensions.Options;
using MySqlX.XDevAPI;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.IO.Pipelines;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;

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

        public async Task<Stream> getDataSource()
        {

            var resultValidation = _webConfigValidator.Validate(_webConfig.CurrentValue);
            if (resultValidation.IsValid)
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    HttpResponseMessage response = await httpClient.GetAsync(_webConfig.CurrentValue.uri);
                    if (response.IsSuccessStatusCode)
                    {
                        using (Stream gzipStream = await response.Content.ReadAsStreamAsync())
                        {
                            using (GZipStream decompressionStream = new GZipStream(gzipStream, CompressionMode.Decompress))
                            {
                                MemoryStream memoryStream = new MemoryStream();
                                await decompressionStream.CopyToAsync(memoryStream);
                                memoryStream.Seek(0, SeekOrigin.Begin);
                                return memoryStream;
                            }
                        }
                    }
                    else throw new Exception(response.StatusCode + "");
                }
            }
            else throw new Exception("Invalid uri address");
        }
    }
}

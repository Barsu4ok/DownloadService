using DownloadService.Config;
using DownloadService.Services.Interfaces;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;

namespace DownloadService.DataSources
{
    public class WebDataSource : IDataSource
    {
        private readonly IOptionsMonitor<WebConfig> _webConfig;

        public WebDataSource(IOptionsMonitor<WebConfig> webConfig)
        {
            _webConfig = webConfig;
        }

        public async Task<Stream> getDataSource()
        {
            HttpClient httpClient = new HttpClient();
            return await httpClient.GetStreamAsync(_webConfig.CurrentValue.uri) ?? throw new Exception("Request failed");
        }
    }
}

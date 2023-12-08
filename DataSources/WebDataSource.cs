using DownloadService.Config;
using DownloadService.Services.Interfaces;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DownloadService.DataSources
{
    public class WebDataSource : IDataSource
    {
        private readonly string _uri;

        public WebDataSource(string uri)
        {
           _uri = uri;
        }

        public Stream getDataSource()
        {
            using HttpClient httpClient = new HttpClient();
            using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, _uri);
            HttpResponseMessage response = httpClient.Send(request);
            return response.Content.ReadAsStream() ?? throw new Exception("Request failed");
        }
    }
}

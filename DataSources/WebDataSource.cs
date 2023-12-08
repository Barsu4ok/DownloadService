using DownloadService.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DownloadService.DataSources
{
    public class WebDataSource : IDataSource
    {
        public Stream getDataSource(String path)
        {
            using HttpClient httpClient = new HttpClient();
            using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, path);
            HttpResponseMessage response = httpClient.Send(request);
            return response.Content.ReadAsStream() ?? throw new Exception("Request failed");
        }
    }
}

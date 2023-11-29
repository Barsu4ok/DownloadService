using DownloadService.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DownloadService.Services
{
    internal class DownloadFileService : IDownloadService
    {
        public HttpResponseMessage download(string uri)
        {
            using HttpClient httpClient = new HttpClient();
            using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, uri);
            HttpResponseMessage response = httpClient.Send(request);
            return response;
        }
    }
}

using DownloadService.Config;
using DownloadService.Interfaces;
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

        public WebDataSource(IOptionsMonitor<WebConfig> webConfig)
        {
            _webConfig = webConfig;
        }

        public async Task<Stream> getDataSource()
        {
            using (HttpClient httpClient = new HttpClient())
            {
                HttpResponseMessage response = await httpClient.GetAsync(_webConfig.CurrentValue.uri);
                if (response.IsSuccessStatusCode)
                {
                    using (Stream zipStream = await response.Content.ReadAsStreamAsync())
                    {
                        using(ZipArchive archive = new ZipArchive(zipStream,ZipArchiveMode.Read))
                        {
                            foreach(ZipArchiveEntry entry in archive.Entries)
                            {
                                if(entry.FullName == _webConfig.CurrentValue.fileName)
                                {
                                    MemoryStream memoryStream = new MemoryStream();
                                    using (Stream entryStream = entry.Open())
                                    {
                                        await entryStream.CopyToAsync(memoryStream);
                                        memoryStream.Seek(0, SeekOrigin.Begin);
                                        return memoryStream;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            throw new Exception("File not found in the archive");
        }
    }
}

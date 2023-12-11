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
    public class FileDataSource : IDataSource
    {
        private readonly IOptionsMonitor<FileConfig> _fileConfig;

        public FileDataSource(IOptionsMonitor<FileConfig> fileConfig)
        {
            _fileConfig = fileConfig;
        }

        public async Task<Stream> getDataSource()
        {
           if(File.Exists(_fileConfig.CurrentValue.inputFilePath))
           {
                return await Task.FromResult(File.OpenRead(_fileConfig.CurrentValue.inputFilePath));
           }
           else
           {
                throw new FileNotFoundException("File not found", _fileConfig.CurrentValue.inputFilePath);
            }
        }
    }
}

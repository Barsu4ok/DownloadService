using DownloadService.Config;
using DownloadService.Interfaces;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO.Compression;
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
            using(FileStream stream = File.Open(_fileConfig.CurrentValue.inputFilePath,FileMode.Open))
            {
                using(var archive = new ZipArchive(stream))
                {
                    foreach(ZipArchiveEntry entry in archive.Entries)
                    {
                        if(entry.FullName == _fileConfig.CurrentValue.fileName)
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
            throw new Exception("File not found in the archive");
        }
    }
}

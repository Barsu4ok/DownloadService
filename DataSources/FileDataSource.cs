using DownloadService.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DownloadService.DataSources
{
    public class FileDataSource : IDataSource
    {
        private readonly string _filePath;

        public FileDataSource(string filePath)
        {
            _filePath = filePath;
        }

        public Stream getDataSource()
        {
           if(File.Exists(_filePath))
           {
                return File.OpenRead(_filePath);
           }
           else
           {
                throw new Exception("Read Failed");
            }
        }
    }
}

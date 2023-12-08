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
        public Stream getDataSource(String path)
        {
           if(File.Exists(path))
           {
                return File.OpenRead(path);
           }
           else
           {
                throw new Exception("Read Failed");
            }
        }
    }
}

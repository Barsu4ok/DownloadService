using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DownloadService.Services.Interfaces
{
    public interface IDataSource
    {
        public Task<Stream> getDataSource();
    }
}

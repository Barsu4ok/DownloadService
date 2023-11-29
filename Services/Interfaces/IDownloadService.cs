using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DownloadService.Services.Interfaces
{
    interface IDownloadService
    {
        public HttpResponseMessage download(string uri);
    }
}

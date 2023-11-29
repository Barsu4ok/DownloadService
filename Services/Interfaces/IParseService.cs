using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DownloadService.Services.Interfaces
{
    interface IParseService
    {
        public void parse(HttpResponseMessage response, string outputPath);
    }
}

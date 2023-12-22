using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DownloadService.Interfaces
{
    public interface ILoggerService
    {
        void Log(LogLevel level, string message);
    }
}

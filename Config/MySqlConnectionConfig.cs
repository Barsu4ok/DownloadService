using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DownloadService.Config
{
    public class MySqlConnectionConfig
    {
        public string server { get; set; }
        public string database { get; set; }
        public string table { get; set; }
        public string user { get; set; }
        public string password { get; set; }
        public string charset { get; set; }
    }
}

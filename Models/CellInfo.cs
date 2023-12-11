using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DownloadService.Models
{
    public class CellInfo
    {
        public string type { get; set; }
        public string countryCode { get; set; }
        public string networkCode { get; set; }
        public string lac { get; set; }
        public string cellId { get; set; }
        public string lon { get; set; }
        public string lan { get; set; }

    }
}

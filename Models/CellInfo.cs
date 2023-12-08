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
        public float countryCode { get; set; }
        public float networkCode { get; set; }
        public float lac { get; set; }
        public float cellId { get; set; }
        public double lon { get; set; }
        public double lan { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DownloadService.Models
{
    public class CellInfo
    {
        public string Radio { get; set; }
        public int MCC { get; set; }
        public int MNC { get; set; }
        public int LAC { get; set; }
        public int CID { get; set; }
        public double LON { get; set; }
        public double LAN { get; set; }

    }
}

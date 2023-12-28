
namespace DownloadService.Models
{
    public class CellInfo
    {
        public string? Radio { get; set; }
        public int Mcc { get; set; }
        public int Mnc { get; set; }
        public int Lac { get; set; }
        public int Cid { get; set; }
        public double Lng { get; set; }
        public double Lat { get; set; }

    }
}

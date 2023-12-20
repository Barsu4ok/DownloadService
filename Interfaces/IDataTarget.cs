using DownloadService.Models;

namespace DownloadService.Interfaces
{
    public interface IDataTarget
    {
        public void WriteData(IEnumerable<CellInfo> cellInfoList);
    }
}

using DownloadService.Models;

namespace DownloadService.Interfaces
{
    internal interface IParseService
    {
        public IEnumerable<CellInfo> Parse(Stream data);
    }
}

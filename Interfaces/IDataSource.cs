
namespace DownloadService.Interfaces
{
    public interface IDataSource
    {
        public Task<Stream> GetDataSource();
    }
}

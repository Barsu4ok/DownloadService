using DownloadService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DownloadService.Interfaces
{
    public interface IDataTarget
    {
        public void writeAllData(IEnumerable<CellInfo> cellInfoList);
    }
}

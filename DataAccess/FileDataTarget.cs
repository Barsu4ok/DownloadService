using DownloadService.Config;
using DownloadService.Models;
using DownloadService.Services.Interfaces;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DownloadService.DataAccess
{
    public class FileDataTarget : IDataTarget
    {
        private readonly IOptionsMonitor<FileConfig> _fileConfig;

        public FileDataTarget(IOptionsMonitor<FileConfig> fileConfig)
        {
            _fileConfig = fileConfig;
        }
        public void writeAllData(IEnumerable<CellInfo> cellInfoList)
        {
            using (StreamWriter writer = new StreamWriter(_fileConfig.CurrentValue.outputFilePath, false))
            {
                foreach (var cellInfo in cellInfoList)
                {
                    writer.WriteLine(cellInfo.type + "," + cellInfo.countryCode + "," + cellInfo.networkCode +
                        "," + cellInfo.lac + "," + cellInfo.cellId + "," + cellInfo.lon + "," + cellInfo.lan);
                }
            }
        }
    }
}

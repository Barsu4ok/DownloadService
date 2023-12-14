using DownloadService.Config;
using DownloadService.Interfaces;
using DownloadService.Models;
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
                var sb = new StringBuilder();
                foreach (var cellInfo in cellInfoList)
                {
                    sb.Append(cellInfo.Radio);
                    sb.Append(',');
                    sb.Append(cellInfo.MCC);
                    sb.Append(',');
                    sb.Append(cellInfo.MNC);
                    sb.Append(',');
                    sb.Append(cellInfo.LAC);
                    sb.Append(',');
                    sb.Append(cellInfo.CID);
                    sb.Append(',');
                    sb.Append(cellInfo.LON);
                    sb.Append(',');
                    sb.Append(cellInfo.LAN);
                    writer.WriteLine(sb.ToString());
                    sb.Clear();
                }
            }
        }
    }
}

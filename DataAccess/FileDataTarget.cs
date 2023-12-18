using DownloadService.Config;
using DownloadService.Interfaces;
using DownloadService.Models;
using DownloadService.Validators;
using FluentValidation;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DownloadService.DataAccess
{
    public class FileDataTarget : IDataTarget
    {
        private readonly IOptionsMonitor<FileConfig> _fileConfig;
        private readonly IValidator<FileConfig> _fileConfigValidator;

        public FileDataTarget(IOptionsMonitor<FileConfig> fileConfig, IValidator<FileConfig> fileConfigValidator)
        {
            _fileConfig = fileConfig;
            _fileConfigValidator = fileConfigValidator;
        }
        public void writeData(IEnumerable<CellInfo> cellInfoList)
        {
            var delimiter = CultureInfo.InvariantCulture;
            var resultValidation = _fileConfigValidator.Validate(_fileConfig.CurrentValue);
            if (resultValidation.IsValid)
            {
                using (StreamWriter writer = new StreamWriter(_fileConfig.CurrentValue.outputFilePath, false))
                {
                    foreach (var cellInfo in cellInfoList)
                    {
                        writer.Write(cellInfo.Radio);
                        writer.Write(',');
                        writer.Write(cellInfo.MCC);
                        writer.Write(',');
                        writer.Write(cellInfo.MNC);
                        writer.Write(',');
                        writer.Write(cellInfo.LAC);
                        writer.Write(',');
                        writer.Write(cellInfo.CID);
                        writer.Write(',');
                        writer.Write(cellInfo.LON.ToString(delimiter));
                        writer.Write(',');
                        writer.Write(cellInfo.LAN.ToString(delimiter));
                        writer.WriteLine("");
                    }
                }
            }
            else throw new Exception("Output path is not valid");
        }
    }
}

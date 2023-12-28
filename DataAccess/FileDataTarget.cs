using DownloadService.Config;
using DownloadService.Interfaces;
using DownloadService.Models;
using FluentValidation;
using Microsoft.Extensions.Options;
using System.Globalization;


namespace DownloadService.DataAccess
{
    public class FileDataTarget : IDataTarget
    {
        private readonly IOptionsMonitor<FileConfig> _fileConfig;
        private readonly IValidator<FileConfig> _fileConfigValidator;
        private readonly ILoggerService _logger;

        public FileDataTarget(ILoggerService logger,IOptionsMonitor<FileConfig> fileConfig, IValidator<FileConfig> fileConfigValidator)
        {
            _logger = logger;
            _fileConfig = fileConfig;
            _fileConfigValidator = fileConfigValidator;
        }
        public void WriteData(IEnumerable<CellInfo> cellInfoList)
        {
            var resultValidation = _fileConfigValidator.Validate(_fileConfig.CurrentValue);
            if (!resultValidation.IsValid)
            {
                _logger.Log(LogLevel.Error,$"Validation failed: {resultValidation.Errors}");
            }

            using var writer = new StreamWriter(_fileConfig.CurrentValue.OutputFilePath ?? throw new InvalidOperationException(), false);
            foreach (var cellInfo in cellInfoList)
            {
                writer.Write(cellInfo.Radio);
                writer.Write(',');
                writer.Write(cellInfo.Mcc);
                writer.Write(',');
                writer.Write(cellInfo.Mnc);
                writer.Write(',');
                writer.Write(cellInfo.Lac);
                writer.Write(',');
                writer.Write(cellInfo.Cid);
                writer.Write(',');
                writer.Write(cellInfo.Radio.ToString(CultureInfo.InvariantCulture));
                writer.Write(',');
                writer.WriteLine(cellInfo.Lat.ToString(CultureInfo.InvariantCulture));
            }
            _logger.Log(LogLevel.Information, "Successful writing of data to a file");
        }
    }
}

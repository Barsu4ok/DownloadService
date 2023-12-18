using DownloadService.Config;
using DownloadService.Interfaces;
using DownloadService.Validators;
using FluentValidation;
using Microsoft.Extensions.Options;
using SharpCompress.Archives.GZip;
using SharpCompress.Common;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DownloadService.DataSources
{
    public class FileDataSource : IDataSource
    {
        private readonly IOptionsMonitor<FileConfig> _fileConfig;
        private readonly IValidator<FileConfig> _fileConfigValidator;

        public FileDataSource(IOptionsMonitor<FileConfig> fileConfig, IValidator<FileConfig> fileConfigValidator)
        {
            _fileConfig = fileConfig;
            _fileConfigValidator = fileConfigValidator;
        }

        public async Task<Stream> getDataSource()
        {
            var resultValidation = _fileConfigValidator.Validate(_fileConfig.CurrentValue);
            if (resultValidation.IsValid)
            {
                using (FileStream fileStream = File.OpenRead(_fileConfig.CurrentValue.inputFilePath))
                {
                    using (GZipStream zipStream = new GZipStream(fileStream, CompressionMode.Decompress))
                    {
                        MemoryStream memoryStream = new MemoryStream();
                        zipStream.CopyTo(memoryStream);
                        memoryStream.Seek(0, SeekOrigin.Begin);
                        return memoryStream;
                    }
                }
            }
            else throw new Exception("Input path is not valid");
        }
    }
}

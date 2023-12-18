using DownloadService.Config;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DownloadService.Validators
{
    public class FileConfigValidator : AbstractValidator<FileConfig>
    {
        public FileConfigValidator()
        {
            RuleFor(inputPath => inputPath.inputFilePath).NotEmpty().Must(BeValidInputFileExtension);
            RuleFor(outputPath => outputPath.outputFilePath).NotEmpty().Must(BeValidOutputFileExtension);
        }

        private bool BeValidInputFileExtension(string filePath)
        {
            string extension = Path.GetExtension(filePath);
            return !string.IsNullOrEmpty(extension) && extension.Equals(".gz", StringComparison.OrdinalIgnoreCase);
        }
        private bool BeValidOutputFileExtension(string filePath)
        {
            string extension = Path.GetExtension(filePath);
            return !string.IsNullOrEmpty(extension) && extension.Equals(".txt", StringComparison.OrdinalIgnoreCase);
        }
    }
}

using DownloadService.Config;
using FluentValidation;


namespace DownloadService.Validators
{
    public class FileConfigValidator : AbstractValidator<FileConfig>
    {
        public FileConfigValidator()
        {
            RuleFor(config => config.InputFilePath).NotEmpty().Must(BeValidInputFileExtension);
            RuleFor(config => config.OutputFilePath).NotEmpty().Must(BeValidOutputFileExtension);
        }

        private static bool BeValidInputFileExtension(string? filePath)
        {
            var extension = Path.GetExtension(filePath);
            return !string.IsNullOrEmpty(extension) && extension.Equals(".gz", StringComparison.OrdinalIgnoreCase);
        }
        private static bool BeValidOutputFileExtension(string? filePath)
        {
            var extension = Path.GetExtension(filePath);
            return !string.IsNullOrEmpty(extension) && extension.Equals(".txt", StringComparison.OrdinalIgnoreCase);
        }
    }
}

using DownloadService.Config;
using FluentValidation;

namespace DownloadService.Validators
{
    public class MySqlConnectionValidator : AbstractValidator<MySqlConnectionConfig>
    {
        public MySqlConnectionValidator()
        {
            RuleFor(mySqlConnection => mySqlConnection.ConnectionString)
                .NotEmpty()
                .Must(BeValidConnectionString);
        }
        private static bool BeValidConnectionString(string? connectionString)
        {
            var requiredParams = new List<string> { "server", "uid", "pwd", "database" };
            return requiredParams.TrueForAll(param => connectionString != null && connectionString.Contains($"{param}="));
        }
    }
}

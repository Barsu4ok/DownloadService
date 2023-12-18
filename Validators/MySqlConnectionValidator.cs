using DownloadService.Config;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DownloadService.Validators
{
    public class MySqlConnectionValidator : AbstractValidator<MySqlConnectionConfig>
    {
        public MySqlConnectionValidator()
        {
            RuleFor(mySqlConnection => mySqlConnection.connectionString)
                .NotEmpty()
                .Must(BeValidConnectionString);
        }
        private bool BeValidConnectionString(string connectionString)
        {
            var requiredParams = new[] { "server", "uid", "pwd", "database" };
            foreach (var param in requiredParams)
            {
                if (!connectionString.Contains($"{param}="))
                {
                    return false;
                }   
            }
            return true;
        }
    }
}

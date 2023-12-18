using DownloadService.Config;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DownloadService.Validators
{
    public class TimerConfigValidator : AbstractValidator<TimerConfig>
    {
        public TimerConfigValidator()
        {
            RuleFor(timer => timer.timeInterval).NotNull().GreaterThan(0);
        }
    }
}

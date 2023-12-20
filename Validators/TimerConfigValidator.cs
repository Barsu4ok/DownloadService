using DownloadService.Config;
using FluentValidation;

namespace DownloadService.Validators
{
    public class TimerConfigValidator : AbstractValidator<TimerConfig>
    {
        public TimerConfigValidator()
        {
            RuleFor(timer => timer.TimeInterval).NotNull();
        }
    }
}

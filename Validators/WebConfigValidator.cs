using DownloadService.Config;
using FluentValidation;

namespace DownloadService.Validators
{
    public class WebConfigValidator : AbstractValidator<WebConfig>
    {
        public WebConfigValidator()
        {
            RuleFor(x => x.Uri).NotEmpty().Must(BeValidUrl);
        }
        private static bool BeValidUrl(string? url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out var result) &&
                   (result.Scheme == Uri.UriSchemeHttp || result.Scheme == Uri.UriSchemeHttps);
        }

    }
}

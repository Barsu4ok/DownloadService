using DownloadService.Config;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DownloadService.Validators
{
    public class WebConfigValidator : AbstractValidator<WebConfig>
    {
        public WebConfigValidator()
        {
            RuleFor(x => x.uri).NotEmpty().Must(BeValidUrl);
        }
        private bool BeValidUrl(string url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out Uri result) &&
                   (result.Scheme == Uri.UriSchemeHttp || result.Scheme == Uri.UriSchemeHttps);
        }

    }
}

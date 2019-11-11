using System.Text.Encodings.Web;
using System.Threading.Tasks;
using lonefire.Services;
using Microsoft.Extensions.Localization;

namespace lonefire.Extensions
{
    public static class EmailSenderExtensions
    {
        public static Task SendEmailConfirmationAsync(this IEmailSender emailSender, string email, string link, IStringLocalizer _localizer)
        {
            return emailSender.SendEmailAsync(email, _localizer["Confirm your email"],
               $" {_localizer["Please confirm your email"]}: <a href='{HtmlEncoder.Default.Encode(link)}'>link</a>");
        }
    }
}

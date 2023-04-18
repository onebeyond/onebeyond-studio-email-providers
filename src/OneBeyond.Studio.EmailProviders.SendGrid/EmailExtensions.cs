using System.Net.Mail;
using SendGrid.Helpers.Mail;

namespace OneBeyond.Studio.EmailProviders.SendGrid;
internal static class EmailExtensions
{
    internal static EmailAddress ToEmailAddress(this MailAddress address)
        => new EmailAddress(address.Address, address.DisplayName);
}

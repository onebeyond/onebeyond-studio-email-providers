using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;

namespace OneBeyond.Studio.EmailProviders.Domain;

public interface IEmailSender
{
    /// <summary>
    /// Send e-mail
    /// </summary>
    /// <param name="mailMessage">E-mail to be sent</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Correlation Id of the e-mail in the external system</returns>
    Task<string?> SendEmailAsync(
        MailMessage mailMessage,
        CancellationToken cancellationToken = default);
}

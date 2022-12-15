using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;

namespace OneBeyond.Studio.EmailProviders.Domain;

public interface IEmailSender
{
    /// <summary>
    /// Email sender
    /// </summary>
    /// <param name="mailMessage"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task SendEmailAsync(
        MailMessage mailMessage,
        CancellationToken cancellationToken = default);
}

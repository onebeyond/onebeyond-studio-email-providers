using System.Collections.Generic;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
using EnsureThat;

namespace OneBeyond.Studio.EmailProviders.Domain;

public static class EmailSenderExtensions
{
    public static Task SendEmailAsync(
        this IEmailSender @this,
        string toEmailAddress,
        string subject,
        string body,
        string? ccEmailAddress = null,
        IEnumerable<Attachment>? attachments = null,
        CancellationToken cancellationToken = default)
    {
        EnsureArg.IsNotNull(@this);
        EnsureArg.IsNotNullOrWhiteSpace(toEmailAddress);
        EnsureArg.IsNotNullOrWhiteSpace(subject);
        EnsureArg.IsNotNullOrWhiteSpace(body);

        MailMessage mailMessage = new MailMessage
        {
            Subject = subject,
            Body = body,
            IsBodyHtml = true
        };

        mailMessage.To.Add(toEmailAddress);

        if (!string.IsNullOrEmpty(ccEmailAddress))
        {
            mailMessage.CC.Add(ccEmailAddress);
        }

        if (attachments != null)
        {
            foreach (var attachment in attachments)
            {
                mailMessage.Attachments.Add(attachment);
            }
        }

        return @this.SendEmailAsync(mailMessage, cancellationToken);
    }
}

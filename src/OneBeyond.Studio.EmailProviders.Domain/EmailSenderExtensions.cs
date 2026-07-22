using System.Net.Mail;
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

    public static string GetOriginalRecipientsForBody(this MailMessage mailMessage)
    {
        var originalRecipients = "";
        var toAsCsv = string.Join(',', mailMessage.To.Select(x => x.Address));

        originalRecipients += Environment.NewLine;
        originalRecipients += "Original Recipients:";
        originalRecipients += Environment.NewLine;
        originalRecipients += $"To: {toAsCsv}";

        if (mailMessage.CC.Any())
        {
            var ccAsCsv = string.Join(',', mailMessage.CC.Select(x => x.Address));
            originalRecipients += Environment.NewLine;
            originalRecipients += $"CC: {ccAsCsv}";
        }

        if (mailMessage.Bcc.Any())
        {
            var bccAsCsv = string.Join(',', mailMessage.Bcc.Select(x => x.Address));
            originalRecipients += Environment.NewLine;
            originalRecipients += $"BCC: {bccAsCsv}";
        }

        return originalRecipients;
    }
}

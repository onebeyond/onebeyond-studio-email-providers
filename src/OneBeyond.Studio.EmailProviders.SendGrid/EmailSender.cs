using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
using EnsureThat;
using OneBeyond.Studio.EmailProviders.Domain;
using OneBeyond.Studio.EmailProviders.Domain.Exceptions;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace OneBeyond.Studio.EmailProviders.SendGrid;

internal sealed class EmailSender : IEmailSender
{
    private readonly EmailAddress _defaultSender;
    private readonly string _sendGridApiKey;
    private readonly string? _enforcedToEmailAddress;

    public EmailSender(
        string sendGridApiKey,
        string fromEmail,
        string? fromEmailName,
        string? enforcedToEmailAddress)
    {

        EnsureArg.IsNotNullOrWhiteSpace(sendGridApiKey, nameof(sendGridApiKey));
        EnsureArg.IsNotNullOrWhiteSpace(fromEmail, nameof(fromEmail));

        _sendGridApiKey = sendGridApiKey;
        _defaultSender = new EmailAddress(fromEmail, fromEmailName);
        _enforcedToEmailAddress = enforcedToEmailAddress;
    }

    public async Task SendEmailAsync(MailMessage mailMessage, CancellationToken cancellationToken = default)
    {
        EnsureArg.IsNotNull(mailMessage, nameof(mailMessage));

        if (mailMessage.ReplyToList.Count > 1)
        {
            throw new EmailSenderException("SendGrid does not support more than one replyTo address.");
        }

        if (mailMessage.AlternateViews.Count > 0)
        {
            throw new EmailSenderException("AlternativeViews are not supported by SendGrid.");
        }

        //map mail message to send grid message
        var sendGridMessage = new SendGridMessage()
        {
            From = GetMailSender(mailMessage.From, _defaultSender)
        };

        foreach (var to in GetMailRecipients(mailMessage.To, _enforcedToEmailAddress))
        {
            sendGridMessage.AddTo(to);
        }

        foreach (var cc in mailMessage.CC)
        {
            if (!mailMessage.To.Contains(cc)) //SendGrid does not allow CC to contain To addresses 
            {
                sendGridMessage.AddCc(cc.ToEmailAddress());
            }
        }

        foreach (var bcc in mailMessage.Bcc)
        {
            if (!mailMessage.To.Contains(bcc)) //SendGrid does not allow CC to contain To addresses 

            {
                sendGridMessage.AddBcc(bcc.ToEmailAddress());
            }
        }

        foreach (var replyTo in mailMessage.ReplyToList)
        {
            sendGridMessage.ReplyTo = replyTo.ToEmailAddress();
        }

        sendGridMessage.AddHeader("Priority", mailMessage.Priority.ToString());

        sendGridMessage.Subject = mailMessage.Subject;

        if (mailMessage.IsBodyHtml)
        {
            sendGridMessage.HtmlContent = mailMessage.Body;
        }
        else
        {
            sendGridMessage.PlainTextContent = mailMessage.Body;
        }

        foreach (string key in mailMessage.Headers)
        {
            sendGridMessage.AddHeader(key, mailMessage.Headers[key]);
        }

        foreach (var attachment in mailMessage.Attachments)
        {
            await sendGridMessage.AddAttachmentAsync(
                attachment.Name,
                attachment.ContentStream,
                attachment.ContentType.MediaType,
                disposition: string.IsNullOrEmpty(attachment.ContentId) ? "attachment" : "inline", //inline is used to display images within an e-mail's body)
                content_id: attachment.ContentId,
                cancellationToken: cancellationToken);
        }

        await SendEmailMessageAsync(sendGridMessage, cancellationToken);
    }

    private static EmailAddress GetMailSender(MailAddress? sender, EmailAddress defaultSender)
        => sender is { }
            ? sender.ToEmailAddress()
            : defaultSender;

    private static List<EmailAddress> GetMailRecipients(MailAddressCollection recipients, string? enforcedToEmailAddress)
        => string.IsNullOrEmpty(enforcedToEmailAddress)
            ? recipients.Select(recipient => recipient.ToEmailAddress()).ToList()
            : new List<EmailAddress> { new EmailAddress(enforcedToEmailAddress!) };

    private Task SendEmailMessageAsync(SendGridMessage sendGridMessage, CancellationToken cancellationToken)
    {
        var client = new SendGridClient(_sendGridApiKey);
        return client.SendEmailAsync(sendGridMessage, cancellationToken);
    }
}

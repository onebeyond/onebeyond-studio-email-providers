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
    private readonly string _fromEmail;
    private readonly string _fromEmailName;
    private readonly string _sendGridApiKey;
    private readonly string? _enforcedToEmailAddress;

    /// <summary>Create an object to Handle Sending e-mail using Sendgrid service</summary>
    /// <param name="sendGridApiKey"></param>
    /// <param name="fromEmail"></param>
    /// <param name="fromEmailName"></param>
    public EmailSender(
        string sendGridApiKey,
        string fromEmail,
        string fromEmailName,
        string? enforcedToEmailAddress)
    {

        EnsureArg.IsNotNullOrWhiteSpace(sendGridApiKey, nameof(sendGridApiKey));
        EnsureArg.IsNotNullOrWhiteSpace(fromEmail, nameof(fromEmail));
        EnsureArg.IsNotNullOrWhiteSpace(fromEmailName, nameof(fromEmailName));

        _sendGridApiKey = sendGridApiKey;
        _fromEmail = fromEmail;
        _fromEmailName = fromEmailName;
        _enforcedToEmailAddress = enforcedToEmailAddress;
    }

    public async Task SendEmailAsync(MailMessage mailMessage, CancellationToken cancellationToken = default)
    {
        EnsureArg.IsNotNull(mailMessage);
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
            From = new EmailAddress(_fromEmail, _fromEmailName)
        };

        if (mailMessage.From != null)
        {
            sendGridMessage.From = new EmailAddress(
                mailMessage.From.Address, mailMessage.From.DisplayName);
        }

        if (!string.IsNullOrEmpty(_enforcedToEmailAddress))
        {
            mailMessage.To.Clear();
            sendGridMessage.AddTo(new EmailAddress(_enforcedToEmailAddress));
        }

        foreach (var to in mailMessage.To)
        {
            sendGridMessage.AddTo(new EmailAddress(
                to.Address, to.DisplayName));
        }

        foreach (var cc in mailMessage.CC)
        {
            //SendGrid does not allow CC to contain To addresses 
            if (!mailMessage.To.Contains(cc))
            {
                sendGridMessage.AddCc(new EmailAddress(
                    cc.Address, cc.DisplayName));
            }
        }

        foreach (var bcc in mailMessage.Bcc)
        {
            //SendGrid does not allow CC to contain To addresses 
            if (!mailMessage.To.Contains(bcc))
            {
                sendGridMessage.AddBcc(new EmailAddress(
                    bcc.Address, bcc.DisplayName));
            }
        }

        foreach (var replyTo in mailMessage.ReplyToList)
        {
            sendGridMessage.ReplyTo = new EmailAddress(replyTo.Address, replyTo.DisplayName);
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

    private Task SendEmailMessageAsync(SendGridMessage sendGridMessage, CancellationToken cancellationToken)
    {
        var client = new SendGridClient(_sendGridApiKey);
        return client.SendEmailAsync(sendGridMessage, cancellationToken);
    }
}

using System.Net.Mail;
using Azure.Communication.Email;
using EnsureThat;
using Microsoft.Extensions.Logging;
using OneBeyond.Studio.EmailProviders.Domain;

namespace OneBeyond.Studio.EmailProviders.AzureCommService;

internal sealed class EmailSender : IEmailSender
{
    private readonly ILogger _logger;

    private readonly string _defaultFromAddress;
    private readonly string? _enforcedToEmailAddress;
    private readonly EmailClient _emailClient;

    /// <summary>Create an object to Handle Sending e-mail using Sendgrid service</summary>
    /// <param name="connectionString">Azure Communication service connection string</param>
    public EmailSender(
        ILoggerFactory loggerFactory,
        string connectionString,
        string defaultFromAddress,
        string? enforcedToEmailAddress)
    {
        EnsureArg.IsNotNull(loggerFactory, nameof(loggerFactory));
        EnsureArg.IsNotNullOrWhiteSpace(connectionString, nameof(connectionString));
        EnsureArg.IsNotNullOrWhiteSpace(defaultFromAddress, nameof(defaultFromAddress));

        _logger = loggerFactory.CreateLogger<EmailSender>();
        _defaultFromAddress = defaultFromAddress;
        _enforcedToEmailAddress = enforcedToEmailAddress;
        _emailClient = new EmailClient(connectionString);
    }

    public async Task SendEmailAsync(MailMessage mailMessage, CancellationToken cancellationToken = default)
    {
        EnsureArg.IsNotNull(mailMessage, nameof(mailMessage));

        var fromAddress = mailMessage.From?.Address ?? _defaultFromAddress;

        var toAddressesList = string.IsNullOrEmpty(_enforcedToEmailAddress)
            ? mailMessage.To.Select(x => new EmailAddress(x.Address, x.DisplayName)).ToList()
            : new List<EmailAddress> { new EmailAddress(_enforcedToEmailAddress) };

        var ccAddressesList = mailMessage.CC.Select(x => new EmailAddress(x.Address, x.DisplayName)).ToList();

        var bccAddressesList = mailMessage.Bcc.Select(x => new EmailAddress(x.Address, x.DisplayName)).ToList();

        var emailRecipients = new EmailRecipients(toAddressesList, ccAddressesList, bccAddressesList);

        var emailContent = new EmailContent(mailMessage.Subject);

        if (mailMessage.IsBodyHtml)
        {
            emailContent.Html = mailMessage.Body;
        }
        else
        {
            emailContent.PlainText = mailMessage.Body;
        }

        var emailMessage = new EmailMessage(fromAddress, emailRecipients, emailContent);

        foreach (string key in mailMessage.Headers)
        {
            emailMessage.Headers.Add(key, mailMessage.Headers[key]);
        }

        emailMessage.Headers.Add("Priority", mailMessage.Priority.ToString());

        foreach (var attachment in mailMessage.Attachments)
        {
            // Read the attachment file into a byte array
            byte[] attachmentBytes;
            using (var ms = new MemoryStream())
            {
                await attachment.ContentStream.CopyToAsync(ms);
                attachmentBytes = ms.ToArray();
            }

            emailMessage.Attachments.Add(
                new EmailAttachment(
                    attachment.Name,
                    attachment.ContentType.MediaType,
                    new BinaryData(attachmentBytes)));
        }

        await _emailClient.SendAsync(
            Azure.WaitUntil.Completed, 
            emailMessage,
            cancellationToken);
    }
}

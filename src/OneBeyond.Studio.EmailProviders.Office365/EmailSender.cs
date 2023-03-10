using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Threading;
using EnsureThat;
using Microsoft.Exchange.WebServices.Data;
using OneBeyond.Studio.EmailProviders.Domain;
using OneBeyond.Studio.EmailProviders.Domain.Exceptions;

namespace OneBeyond.Studio.EmailProviders.Office365;

internal sealed class EmailSender : IEmailSender
{
    private readonly ExchangeVersion _exchangeVersion;
    private readonly string _username;
    private readonly string _password;
    private readonly string _fromEmail;
    private readonly string _fromEmailName;
    private readonly string? _enforcedToEmailAddress;
    private readonly string _deliveryMethod;
    private readonly bool _saveCopy;
    private readonly string? _saveCopyFolderId;
    private readonly int _port;
    private readonly string _host;
    private readonly bool _enableSsl;

    /// <summary>Create an object to Handle Sending e-mail using Office365 service</summary>
    public EmailSender(ExchangeVersion exchangeVersion,
                            string username,
                            string password,
                            string fromEmail,
                            string fromEmailName,
                            string? enforcedToEmailAddress,
                            string deliveryMethod,
                            bool saveCopy,
                            string? saveCopyFolderId,
                            int port,
                            string host,
                            bool enableSsl)
    {


        EnsureArg.IsNotNullOrWhiteSpace(username, nameof(username));
        EnsureArg.IsNotNullOrWhiteSpace(password, nameof(password));
        EnsureArg.IsNotNullOrWhiteSpace(fromEmail, nameof(fromEmail));
        EnsureArg.IsNotNullOrWhiteSpace(fromEmailName, nameof(fromEmailName));

        EnsureArg.IsNotNullOrWhiteSpace(deliveryMethod, nameof(deliveryMethod));
        if (saveCopy)
        {
            EnsureArg.IsNotNullOrWhiteSpace(saveCopyFolderId, nameof(saveCopyFolderId));
        }

        EnsureArg.IsNotNullOrWhiteSpace(host, nameof(host));
        EnsureArg.IsInRange(port, 1, 65535, nameof(port));

        _exchangeVersion = exchangeVersion;
        _username = username;
        _password = password;
        _fromEmail = fromEmail;
        _fromEmailName = fromEmailName;
        _enforcedToEmailAddress = enforcedToEmailAddress;
        _deliveryMethod = deliveryMethod;
        _saveCopy = saveCopy;
        _saveCopyFolderId = saveCopyFolderId;
        _port = port;
        _host = host;
        _enableSsl = enableSsl;
    }

    public System.Threading.Tasks.Task SendEmailAsync(MailMessage mailMessage, CancellationToken cancellationToken = default)
    {
        EnsureArg.IsNotNull(mailMessage);

        if (!string.IsNullOrEmpty(_enforcedToEmailAddress))
        {
            mailMessage.To.Clear();
            mailMessage.To.Add(new MailAddress(_enforcedToEmailAddress));
        }

        if (_deliveryMethod == DeliveryMethod.Smtp)
        {
            return SendMailMessageAsync(mailMessage);
        }

        return _deliveryMethod == DeliveryMethod.EWS
            ? SendEWSMailMessageAsync(mailMessage)
            : throw new EmailSenderException($"Delivery Method '{_deliveryMethod}' is not supported.");
    }

    private System.Threading.Tasks.Task SendEWSMailMessageAsync(MailMessage mailMessage)
    {
        if (mailMessage.AlternateViews.Count > 0)
        {
            throw new EmailSenderException("AlternativeViews are not supported by Exchange (EWS).");
        }

        //map Net MailMessage to EWS EmailMessage
        var emailMessage = CreateExchangeEmail();
        emailMessage.From = new EmailAddress(_fromEmailName, _fromEmail);

        if (mailMessage.From != null)
        {
            emailMessage.From = new EmailAddress(
                mailMessage.From.Address, mailMessage.From.DisplayName);
        }

        foreach (var to in mailMessage.To)
        {
            emailMessage.ToRecipients.Add(
                new EmailAddress(to.DisplayName, to.Address));
        }

        foreach (var cc in mailMessage.CC)
        {
            emailMessage.CcRecipients.Add(
                new EmailAddress(cc.DisplayName, cc.Address));
        }

        foreach (var bcc in mailMessage.Bcc)
        {
            emailMessage.BccRecipients.Add(
                new EmailAddress(bcc.DisplayName, bcc.Address));
        }

        foreach (var replyTo in mailMessage.ReplyToList)
        {
            emailMessage.ReplyTo.Add(
                new EmailAddress(replyTo.DisplayName, replyTo.Address));
        }

        emailMessage.Importance = mailMessage.Priority switch
        {
            MailPriority.High => Importance.High,
            MailPriority.Low => Importance.Low,
            MailPriority.Normal => Importance.Normal,
            _ => throw new EmailSenderException($"MailPriority '{mailMessage.Priority}' is not supported."),
        };

        emailMessage.Subject = mailMessage.Subject;

        emailMessage.Body = mailMessage.IsBodyHtml ?
            new MessageBody(BodyType.HTML, mailMessage.Body) : new MessageBody(BodyType.Text, mailMessage.Body);

        foreach (string key in mailMessage.Headers)
        {
            var property = new ExtendedPropertyDefinition(DefaultExtendedPropertySet.InternetHeaders, "x-custom", MapiPropertyType.String);
            emailMessage.SetExtendedProperty(property, mailMessage.Headers[key]);
        }

        foreach (var attachment in mailMessage.Attachments)
        {
            using (var br = new BinaryReader(attachment.ContentStream))
            {
                var b = br.ReadBytes((int)attachment.ContentStream.Length);
                emailMessage.Attachments.AddFileAttachment(attachment.Name, b);
            }
        }

        return SendMailMessageAsync(emailMessage);
    }

    private System.Threading.Tasks.Task SendMailMessageAsync(MailMessage mailMessage)
    {
        using (var smtpClient = new SmtpClient())
        {
            smtpClient.Credentials = new NetworkCredential(_username, _password);
            smtpClient.Port = _port;
            smtpClient.Host = _host;
            smtpClient.EnableSsl = _enableSsl;
            return smtpClient.SendMailAsync(mailMessage);
        }
    }

    private System.Threading.Tasks.Task SendMailMessageAsync(EmailMessage emailMessage)
    {
        return _saveCopy
            ? !string.IsNullOrWhiteSpace(_saveCopyFolderId)
                ? emailMessage.SendAndSaveCopy(new FolderId(_saveCopyFolderId))
                : emailMessage.SendAndSaveCopy()
            : emailMessage.Send();

    }

    private EmailMessage CreateExchangeEmail()
    {
        var exchangeService = new ExchangeService(_exchangeVersion);
        exchangeService.Credentials = new WebCredentials(_username, _password);
        exchangeService.AutodiscoverUrl(_username, RedirectionUrlValidationCallback);
        return new EmailMessage(exchangeService);
    }

    private static bool RedirectionUrlValidationCallback(string redirectionUrl)
    {
        return new Uri(redirectionUrl).Scheme == "https";
    }
}

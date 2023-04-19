using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
using Azure.Identity;
using EnsureThat;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Graph.Users.Item.SendMail;
using OneBeyond.Studio.EmailProviders.Domain;
using OneBeyond.Studio.EmailProviders.Domain.Exceptions;

namespace OneBeyond.Studio.EmailProviders.Graph;

internal sealed class EmailSender : IEmailSender
{
    private readonly ILogger _logger;
    private readonly Lazy<GraphServiceClient> _graphServiceClient;
    private readonly string _senderUserAzureId;

    public EmailSender(
        ILoggerFactory loggerFactory,
        string clientId,
        string tenantId,
        string clientSecret,
        string senderUserAzureId)
    {
        EnsureArg.IsNotNull(loggerFactory, nameof(loggerFactory));
        EnsureArg.IsNotNullOrWhiteSpace(clientId, nameof(clientId));
        EnsureArg.IsNotNullOrWhiteSpace(tenantId, nameof(tenantId));
        EnsureArg.IsNotNullOrWhiteSpace(clientSecret, nameof(clientSecret));
        EnsureArg.IsNotNullOrWhiteSpace(senderUserAzureId, nameof(senderUserAzureId));

        _logger = loggerFactory.CreateLogger<EmailSender>();

        _graphServiceClient = new Lazy<GraphServiceClient>(() =>
        {
            var options = new TokenCredentialOptions
            {
                AuthorityHost = AzureAuthorityHosts.AzurePublicCloud
            };

            var authProvider = new ClientSecretCredential(tenantId, clientId, clientSecret, options);
            return new GraphServiceClient(authProvider);
        });

        _senderUserAzureId = senderUserAzureId;
    }

    //Note! If you use this provider, please make sure the application registration you're using
    //has the API Permission (Type: Application) to Microsoft Graph : Mail.Send
    public async Task<string?> SendEmailAsync(
        MailMessage mailMessage,
        CancellationToken cancellationToken = default)
    {
        EnsureArg.IsNotNull(mailMessage, nameof(mailMessage));

        var correlationId = Guid.NewGuid().ToString();

        var message = new Message
        {
            Subject = mailMessage.Subject,

            Body = new ItemBody
            {
                ContentType = mailMessage.IsBodyHtml ? BodyType.Html : BodyType.Text,
                Content = mailMessage.Body
            },

            ToRecipients = GetRecipientsList(mailMessage.To),
            BccRecipients = GetRecipientsList(mailMessage.Bcc),
            CcRecipients = GetRecipientsList(mailMessage.CC),
            Attachments = GetAttachmentsList(mailMessage.Attachments),
            AdditionalData = new Dictionary<string, object>
                {
                    {"X-Correlation-Id", correlationId}
                }
        };

        try
        {
            await _graphServiceClient.Value
                .Users[_senderUserAzureId]
                .SendMail
                .PostAsync(
                    new SendMailPostRequestBody 
                    { 
                        Message = message, 
                        SaveToSentItems = false
                    }, 
                    cancellationToken: cancellationToken).ConfigureAwait(false);

            return correlationId;
        }
        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                "Unable to send message via Graph API: {exception.Message}",
                exception.Message);

            throw new EmailSenderException("Unable to send message via Graph API.", exception);
        }
    }

    private List<Microsoft.Graph.Models.Attachment>? GetAttachmentsList(AttachmentCollection attachments) 
    {
        if (!attachments.Any())
        {
            return null;
        }

        var attachmentsList = new List<Microsoft.Graph.Models.Attachment>();

        foreach (var attachment in attachments)
        {
            using (var br = new BinaryReader(attachment.ContentStream))
            {
                var b = br.ReadBytes((int)attachment.ContentStream.Length);

                attachmentsList.Add(
                    new FileAttachment
                    {
                        Name = attachment.Name,
                        ContentBytes = b,
                        IsInline = !string.IsNullOrEmpty(attachment.ContentId), //inline is used to display images within an e-mail's body
                        ContentId = attachment.ContentId,
                        ContentType = attachment.ContentType.MediaType
                    });
            }
        }

        return attachmentsList;
    }

    private static List<Recipient> GetRecipientsList(MailAddressCollection mailAddresses)
        => mailAddresses.Select(recipient =>
                    new Recipient
                    {
                        EmailAddress = new EmailAddress
                        {
                            Address = recipient.Address,
                            Name = recipient.DisplayName
                        }
                    }).ToList();
}

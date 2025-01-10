using Amazon.Runtime.Internal;
using Amazon.SimpleEmailV2;
using Amazon.SimpleEmailV2.Model;
using EnsureThat;
using OneBeyond.Studio.EmailProviders.Domain;
using OneBeyond.Studio.EmailProviders.Domain.Exceptions;
using System.Net.Mail;

namespace OneBeyond.Studio.EmailProviders.AwsSes;

/// <summary>
/// Email Sender for Amazon SES (Simple Email Service). This utilises SES V2 
/// API. Currently we only support SSO/IAM based authentication. This could be 
/// extended in future to support AWS Access Key/Secret authentication 
/// </summary>
internal sealed class EmailSender : IEmailSender
{
    private readonly AmazonSimpleEmailServiceV2Client _emailClient;
    private readonly string _defaultFromAddress;
    private readonly string? _enforcedToEmailAddress;

    public EmailSender(
        string defaultFromAddress, 
        string? enforcedToEmailAddress)
    {
        // Picks up profile from appsettings
        _emailClient = new AmazonSimpleEmailServiceV2Client();
        _defaultFromAddress = defaultFromAddress;
        _enforcedToEmailAddress = enforcedToEmailAddress;
    }

    public async Task<string?> SendEmailAsync(MailMessage mailMessage, CancellationToken cancellationToken = default)
    {
        EnsureArg.IsNotNull(mailMessage, nameof(mailMessage));

        SendEmailRequest ser = new SendEmailRequest();

        ser.FromEmailAddress = mailMessage.From?.Address ?? _defaultFromAddress;

        var toAddressesList = string.IsNullOrEmpty(_enforcedToEmailAddress)
            ? mailMessage.To.Select(x => x.Address).ToList()
            : _enforcedToEmailAddress.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList();


        ser.Destination = new Destination()
        {
            ToAddresses = toAddressesList
        };

        if (string.IsNullOrWhiteSpace(_enforcedToEmailAddress))
        {
            ser.Destination.CcAddresses = mailMessage.CC.Select(x => x.Address).ToList();
            ser.Destination.BccAddresses = mailMessage.Bcc.Select(x => x.Address).ToList();
        }

        var body = new Body();
        if (mailMessage.IsBodyHtml)
        {
            body.Html = new Content { Data = mailMessage.Body };
        }
        else
        { 
            body.Text = new Content { Data = mailMessage.Body };
        }

        ser.Content = new EmailContent
        {
            Simple = new Message
            {
                Subject = new Content { Data = mailMessage.Subject },
                Body = body
            }
        };

        try
        {
            var response = await _emailClient.SendEmailAsync(ser);
            // SES message id
            return response.MessageId;
        }
        catch (AccountSuspendedException ex)
        {
            throw new EmailSenderException("The account's ability to send email has been permanently restricted.", ex);
        }
        catch (MailFromDomainNotVerifiedException ex)
        {
            throw new EmailSenderException("The sending domain is not verified.", ex);
        }
        catch (MessageRejectedException ex)
        {
            throw new EmailSenderException("The message content is invalid.", ex);
        }
        catch (SendingPausedException ex)
        {
            throw new EmailSenderException("The account's ability to send email is currently paused.", ex);
        }
        catch (TooManyRequestsException ex)
        {
            throw new EmailSenderException("Too many requests were made. Please try again later.", ex);
        }
        catch (Exception ex)
        {
            throw new EmailSenderException($"An error occurred while sending the email", ex);
        }
    }
}

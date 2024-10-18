using System;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
using EnsureThat;
using MimeKit;
using OneBeyond.Studio.EmailProviders.Domain;

namespace OneBeyond.Studio.EmailProviders.Folder;

internal sealed class EmailSender : IEmailSender
{
    private readonly string _folder;
    private readonly string? _fromEmailAddress;
    private readonly string? _fromEmailName;
    private readonly string? _enforcedToEmailAddresses;

    public EmailSender(
        string folder,
        string? fromEmailAddress,
        string? fromEmailName,
        string? enforcedToEmailAddresses)
    {
        EnsureArg.IsNotNullOrWhiteSpace(folder, nameof(folder));
        _folder = folder;
        _fromEmailAddress = fromEmailAddress;
        _fromEmailName = fromEmailName;
        _enforcedToEmailAddresses = enforcedToEmailAddresses;
    }

    public async Task<string?> SendEmailAsync(MailMessage mailMessage, CancellationToken cancellationToken)
    {
        EnsureArg.IsNotNull(mailMessage, nameof(mailMessage));

        if (mailMessage.From is null && _fromEmailAddress is { })
        {
            mailMessage.From = new MailAddress(_fromEmailAddress, _fromEmailName ?? _fromEmailAddress);
        }

        if (!string.IsNullOrWhiteSpace(_enforcedToEmailAddresses))
        {
            mailMessage.To.Clear();
            mailMessage.CC.Clear();
            mailMessage.Bcc.Clear();
            // Folder sending allows comma-separated string
            mailMessage.To.Add(_enforcedToEmailAddresses);
        }

        var mimeMessage = (MimeMessage)mailMessage;

        var correlationId = Guid.NewGuid().ToString();

        await mimeMessage.WriteToAsync($"{_folder}/{correlationId}.eml", cancellationToken);

        return correlationId;
    }
}

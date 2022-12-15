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
    private readonly string? _enforcedToEmailAddress;

    public EmailSender(
        string folder,
        string? fromEmailAddress,
        string? fromEmailName,
        string? enforcedToEmailAddress)
    {
        EnsureArg.IsNotNullOrWhiteSpace(folder, nameof(folder));
        _folder = folder;
        _fromEmailAddress = fromEmailAddress;
        _fromEmailName = fromEmailName;
        _enforcedToEmailAddress = enforcedToEmailAddress;
    }

    public Task SendEmailAsync(MailMessage mailMessage, CancellationToken cancellationToken)
    {
        EnsureArg.IsNotNull(mailMessage, nameof(mailMessage));

        if (mailMessage.From is null
            && _fromEmailAddress is { })
        {
            mailMessage.From = new MailAddress(_fromEmailAddress, _fromEmailName ?? _fromEmailAddress);
        }

        if (!string.IsNullOrWhiteSpace(_enforcedToEmailAddress))
        {
            mailMessage.To.Clear();
            mailMessage.To.Add(new MailAddress(_enforcedToEmailAddress));
        }

        var mimeMessage = (MimeMessage)mailMessage;

        return mimeMessage.WriteToAsync($"{_folder}/{Guid.NewGuid()}.eml", cancellationToken);
    }
}

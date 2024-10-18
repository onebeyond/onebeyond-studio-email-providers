using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
using EnsureThat;
using Microsoft.Extensions.Logging;
using MimeKit;
using OneBeyond.Studio.EmailProviders.Domain;
using OneBeyond.Studio.EmailProviders.Domain.Exceptions;

namespace OneBeyond.Studio.EmailProviders.Smtp;

internal sealed class EmailSender : IEmailSender, IDisposable
{
    private const int SmtpClientsMaxCount = 20;

    private readonly ILogger _logger;
    private readonly string _host;
    private readonly int? _port;
    private readonly MailKit.Security.SecureSocketOptions _secureConnection;
    private readonly string? _username;
    private readonly string? _password;
    private readonly string? _fromEmailAddress;
    private readonly string? _fromEmailName;
    private readonly string? _enforcedToEmailAddresses;
    private readonly ConcurrentQueue<(int Id, MailKit.Net.Smtp.SmtpClient Value)> _smtpClients;
    private int _lastSmtpClientId;
    private readonly bool _enableProtocolLogging;

    public EmailSender(
        ILoggerFactory loggerFactory,
        string host,
        int? port,
        MailKit.Security.SecureSocketOptions secureConnection,
        string? username,
        string? password,
        string? fromEmailAddress,
        string? fromEmailName,
        string? enforcedToEmailAddresses,
        bool enableProtocolLogging)
    {
        EnsureArg.IsNotNull(loggerFactory, nameof(loggerFactory));
        EnsureArg.IsNotNullOrWhiteSpace(host, nameof(host));
        EnsureArg.IsTrue(
            username is null && password is null
            || !string.IsNullOrWhiteSpace(username) && password is not null,
            nameof(username));
        EnsureArg.IsTrue(
            fromEmailAddress is null && fromEmailName is null
            || !string.IsNullOrWhiteSpace(fromEmailAddress),
            nameof(fromEmailAddress));

        _logger = loggerFactory.CreateLogger<EmailSender>();
        _host = host;
        _port = port;
        _secureConnection = secureConnection;
        _username = username;
        _password = password;
        _fromEmailAddress = fromEmailAddress;
        _fromEmailName = fromEmailName;
        _enforcedToEmailAddresses = enforcedToEmailAddresses;
        _smtpClients = new ConcurrentQueue<(int Id, MailKit.Net.Smtp.SmtpClient Value)>();
        _lastSmtpClientId = 0;
        _enableProtocolLogging = enableProtocolLogging;
    }

    public void Dispose()
    {
        while (_smtpClients.TryDequeue(out var smtpClient))
        {
            smtpClient.Value.Disconnect(true);
            smtpClient.Value.Dispose();
        }
    }

    public async Task<string?> SendEmailAsync(MailMessage mailMessage, CancellationToken cancellationToken)
    {
        EnsureArg.IsNotNull(mailMessage);

        if (mailMessage.From is null
            && _fromEmailAddress is not null)
        {
            mailMessage.From = new MailAddress(_fromEmailAddress, _fromEmailName ?? _fromEmailAddress);
        }

        if (!string.IsNullOrWhiteSpace(_enforcedToEmailAddresses))
        {
            mailMessage.To.Clear();
            mailMessage.CC.Clear();
            mailMessage.Bcc.Clear();

            // SMTP handles comma-separated string.
            mailMessage.To.Add(_enforcedToEmailAddresses);
        }

        var mimeMessage = (MimeMessage)mailMessage;

        var smtpClient = await AcquireSmtpClientAsync(cancellationToken).ConfigureAwait(false);

        try
        {
            _logger.LogInformation(
                "Sending message '{SmtpMessageSubject}' via SmtpClient {SmtpClientId} to {SmtpMessageRecipients}",
                mimeMessage.Subject,
                smtpClient.Id,
                string.Join(", ", mimeMessage.To.Mailboxes.Select((mailboxAddress) => mailboxAddress.Address)));
            await smtpClient.Value.SendAsync(mimeMessage, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                "Unable to send message via SmptClient {SmtpClientId}. Disconnecting the client before releasing it",
                smtpClient.Id);
            await smtpClient.Value.DisconnectAsync(true, cancellationToken).ConfigureAwait(false);

            throw new EmailSenderException("Unable to send message via SmptClient { SmtpClientId }.Disconnecting the client before releasing it.", exception);
        }
        finally
        {
            await ReleaseSmtpClientAsync(smtpClient, cancellationToken).ConfigureAwait(false);
        }

        return null; //We do not support correlation Id for SMTP senders
    }

    private async Task<(int Id, MailKit.Net.Smtp.SmtpClient Value)> AcquireSmtpClientAsync(CancellationToken cancellationToken)
    {
        if (!_smtpClients.TryDequeue(out var smtpClient))
        {
            var smtpClientId = Interlocked.Increment(ref _lastSmtpClientId);
            smtpClient = (
                smtpClientId,
                _enableProtocolLogging
                    ? new MailKit.Net.Smtp.SmtpClient(new ProtocolLogger(_logger))
                    : new MailKit.Net.Smtp.SmtpClient());
        }

        try
        {
            if (!smtpClient.Value.IsConnected)
            {
                await smtpClient.Value.ConnectAsync(_host, _port ?? 0, _secureConnection, cancellationToken)
                    .ConfigureAwait(false);
            }

            if (!string.IsNullOrWhiteSpace(_username) && !smtpClient.Value.IsAuthenticated)
            {
                await smtpClient.Value.AuthenticateAsync(_username, _password, cancellationToken)
                    .ConfigureAwait(false);
            }

            _logger.LogInformation(
                "Succefully acquired SmtpClient {SmtpClientId}",
                smtpClient.Id);

            return smtpClient;
        }
        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                "Unable to acquire SmtpClient");
            await ReleaseSmtpClientAsync(smtpClient, cancellationToken).ConfigureAwait(false);

            throw new EmailSenderException("Unable to acquire SmtpClient", exception);
        }
    }

    private async Task ReleaseSmtpClientAsync(
        (int Id, MailKit.Net.Smtp.SmtpClient Value) smtpClient,
        CancellationToken cancellationToken)
    {
        // Only SmtpClients created before threshold are returned into the queue for later re-use,
        // all the others get decommissioned. Another option that SmtpClient gets returned regardless its id
        // provided the queue count is less than the threshold.
        if (smtpClient.Id > SmtpClientsMaxCount)
        {
            await smtpClient.Value.DisconnectAsync(true, cancellationToken).ConfigureAwait(false);
            smtpClient.Value.Dispose();
        }
        else
        {
            _smtpClients.Enqueue(smtpClient);
        }
    }
}

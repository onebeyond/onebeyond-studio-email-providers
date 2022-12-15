using System;
using System.Text;
using EnsureThat;
using MailKit;
using Microsoft.Extensions.Logging;

namespace OneBeyond.Studio.EmailProviders.Smtp;

internal sealed class ProtocolLogger : IProtocolLogger
{
    private readonly ILogger _logger;

    public ProtocolLogger(ILogger logger)
    {
        EnsureArg.IsNotNull(logger, nameof(logger));

        _logger = logger;
    }

    public IAuthenticationSecretDetector? AuthenticationSecretDetector { get; set; }

    public void LogConnect(Uri uri)
        => _logger.LogInformation(
            "SMTP: Connected to {SmtpServerUri}",
            uri?.OriginalString ?? "N/A");

    public void LogClient(byte[] buffer, int offset, int count)
        => Log("C", buffer, offset, count);

    public void LogServer(byte[] buffer, int offset, int count)
        => Log("S", buffer, offset, count);

    private void Log(string prefix, byte[] buffer, int offset, int count)
        => _logger.LogInformation(
            $"SMTP {prefix}: {{SmtpMessage}}",
            Encoding.UTF8.GetString(buffer, offset, count));

    public void Dispose() { }
}

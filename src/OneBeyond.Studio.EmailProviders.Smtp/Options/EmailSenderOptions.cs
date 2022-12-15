namespace OneBeyond.Studio.EmailProviders.Smtp.Options;

/// <summary>
/// Options for email sending
/// </summary>
public sealed record EmailSenderOptions : Domain.Options.EmailSenderOptions
{
    /// <summary>
    /// Specifies SMTP username to be used
    /// </summary>
    public string? Username { get; init; }

    /// <summary>
    /// Specifies SMTP password to be used
    /// </summary>
    public string? Password { get; init; }

    /// <summary>
    /// Specifies SMTP server host like "smtp.gmail.com"
    /// </summary>
    public string? Host { get; init; }

    /// <summary>
    /// Specifies SMTP server port
    /// </summary>
    public int? Port { get; init; }

    /// <summary>
    /// "None" - No SSL or TLS encryption should be used.
    /// "Auto" - Allow the IMailService to decide which SSL or TLS options to use(default).
    ///          If the server does not support SSL or TLS, then the connection will continue without any encryption.
    /// "SslOnConnect" - The connection should use SSL or TLS encryption immediately.
    /// "StartTls" - Elevates the connection to use TLS encryption immediately after reading the greeting and capabilities of the server.
    ///              If the server does not support the STARTTLS extension, then the connection will fail and a NotSupportedException will be thrown.
    /// "StartTlsWhenAvailable" - Elevates the connection to use TLS encryption immediately after reading the greeting and capabilities of the server,
    ///                           but only if the server supports the STARTTLS extension.
    /// </summary>
    public string? SecureConnection { get; init; }

    /// <summary>
    /// Determines whether to write SMTP logs
    /// </summary>
    public bool EnableProtocolLogging { get; init; }
}

namespace OneBeyond.Studio.EmailProviders.Office365.Options;

/// <summary>
/// Options for email sending
/// </summary>
public sealed record EmailSenderOptions : Domain.Options.EmailSenderOptions
{
    /// <summary>
    /// Specifies SMTP username to be used for Exchange Web Service delievry method.
    /// </summary>
    public string? Username { get; init; }

    /// <summary>
    /// Specifies SMTP password to be used for Exchange Web Service delievry method.
    /// </summary>
    public string? Password { get; init; }

    /// <summary>
    /// Specifies the delivery method either EWS or Smtp.
    /// </summary>
    public string? DeliveryMethod { get; init; }

    public EWS? EWS { get; init; }

    public Smtp? Smtp { get; init; }
}

public sealed record EWS
{
    /// <summary>
    /// Sends the e-mail message and saves a copy of it to the specified folder.
    /// </summary>
    public bool SaveCopy { get; init; }

    /// <summary>
    /// The folderId to save the sent message too or empty to save the message to the Sent items folder.
    /// </summary>
    public string? SaveCopyFolderId { get; init; }
}

public sealed record Smtp
{
    /// <summary>
    /// Specifies the SMTP client Port to use.
    /// </summary>
    public int Port { get; init; }

    /// <summary>
    /// Specifies the SMTP client Host to use.
    /// </summary>
    public string? Host { get; init; }

    /// <summary>
    /// Specifies if SSL should be used.
    /// </summary>
    public bool EnableSsl { get; init; }
}

namespace OneBeyond.Studio.EmailProviders.Exchange.Options;

/// <summary>
/// Options for email sending
/// </summary>
public sealed record EmailSenderOptions : Domain.Options.EmailSenderOptions
{
    /// <summary>
    /// Specifies Exchange Web Service API sender email id to be used for Exchange Web Service delievry method.
    /// </summary>
    public string? Username { get; init; }

    /// <summary>
    /// Specifies Exchange Web Service API password to be used for Exchange Web Service delievry method.
    /// </summary>
    public string? Password { get; init; }

    /// <summary>
    /// Specifies Office 365 Web Service Url to be used for Exchange Web Service delievry method.
    /// </summary>
    public string? WebServiceUrl { get; init; }

    /// <summary>
    /// Sends the e-mail message and saves a copy of it to the specified folder.
    /// </summary>
    public bool SaveCopy { get; init; }

    /// <summary>
    /// The folderId to save the sent message too or empty to save the message to the Sent items folder.
    /// </summary>
    public string? SaveCopyFolderId { get; init; }
}

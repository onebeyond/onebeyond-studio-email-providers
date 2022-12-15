namespace OneBeyond.Studio.EmailProviders.SendGrid.Options;

/// <summary>
/// Options for email sending
/// </summary>
public sealed record EmailSenderOptions : Domain.Options.EmailSenderOptions
{
    /// <summary>
    /// Specifies SendGrid API auth key to be used for SendGrid delievry method.
    /// </summary>
    public string? Key { get; init; }
}

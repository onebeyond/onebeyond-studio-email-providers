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
    /// <summary>
    /// In Sendbox mode Sendgrid does not actually send e-mails (in case if you need to test mass e-mails sending campaign)
    /// </summary>
    public bool UseSandBoxMode { get; init; }
}

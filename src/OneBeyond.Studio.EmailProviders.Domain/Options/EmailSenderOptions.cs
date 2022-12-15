namespace OneBeyond.Studio.EmailProviders.Domain.Options;

public abstract record EmailSenderOptions
{
    /// <summary>
    /// Specifies email address to be used for the email From field.
    /// </summary>
    public string? FromEmailAddress { get; init; }

    /// <summary>
    /// Specifies sender name to be used for the email From field.
    /// </summary>
    public string? FromEmailName { get; init; }

    /// <summary>
    /// Specifies if the <see cref="EnforcedToEmailAddress"/> should be used instead of any user email
    /// </summary>
    public bool UseEnforcedToEmailAddress { get; init; }

    /// <summary>
    /// Specificies the email address to send all the emails to, overriding user email.
    /// <remarks>This setting is used only if <see cref="UseEnforcedToEmailAddress"/> is true</remarks>
    /// </summary>
    public string? EnforcedToEmailAddress { get; init; }
}

namespace OneBeyond.Studio.EmailProviders.AzureCommService.Options;

/// <summary>
/// Options for email sending
/// </summary>
public sealed record EmailSenderOptions : Domain.Options.EmailSenderOptions
{
    /// <summary>
    /// Azure communication service connection string
    /// </summary>
    public string CommunicationServiceConnectionString { get; init; } = default!;
    /// <summary>
    /// When DoNotWaitTillOperationCompleted is true, we just pass the e-mail to communication services and do not wait till the e-mail is acutally sent.
    /// When DoNotWaitTillOperationCompleted is false, we do wait till the e-mail is acutally sent (takes longer).
    /// </summary>
    public bool DoNotWaitTillOperationCompleted { get; init; }
}

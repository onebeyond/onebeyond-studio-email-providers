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
}

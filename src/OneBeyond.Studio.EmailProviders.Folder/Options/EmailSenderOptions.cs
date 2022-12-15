namespace OneBeyond.Studio.EmailProviders.Folder.Options;

public sealed record EmailSenderOptions : Domain.Options.EmailSenderOptions
{
    public string? Folder { get; init; }
}

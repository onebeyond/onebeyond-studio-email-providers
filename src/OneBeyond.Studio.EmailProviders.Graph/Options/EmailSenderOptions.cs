namespace OneBeyond.Studio.EmailProviders.Graph.Options;

/// <summary>
/// NOTE: In order to use this sending method, you need to register your app in Azure Active Directory. Once done,
/// you need to also setup the proper permission for email sending by going to:
/// Azure Active Directory -> App Registrations -> [Your App] -> API permissions
/// There, add a permission: Microsoft Graph -> Application Permissions -> Mail.Send
/// </summary>
public sealed record EmailSenderOptions : Domain.Options.EmailSenderOptions
{
    /// <summary>
    /// Application Id that can be found in Azure Portal
    /// once the app has been registered in Azure Active Directory
    /// </summary>
    public string? ClientId { get; init; }

    /// <summary>
    /// Tenant Id that can be found in Azure Portal
    /// once the app has been registered in Azure Active Directory
    /// /// </summary>
    public string? TenantId { get; init; }

    /// <summary>
    /// Client Secret that must be created in the Azure Portal
    /// once the app has been registered, going to:
    /// Azure Active Directory -> App Registrations -> [Your App] -> Certificates & Secrets -> Client secrets
    /// </summary>
    public string? Secret { get; init; }

    /// <summary>
    /// Azure Id of a user registered in Azure Active Directory
    /// that will be used as email sender
    /// </summary>
    public string? SenderUserAzureId { get; init; }
}

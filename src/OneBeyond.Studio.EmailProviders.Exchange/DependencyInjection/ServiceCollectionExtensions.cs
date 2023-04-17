using EnsureThat;
using Microsoft.Exchange.WebServices.Data;
using Microsoft.Extensions.DependencyInjection;
using OneBeyond.Studio.EmailProviders.Domain;
using OneBeyond.Studio.EmailProviders.Exchange.Options;

namespace OneBeyond.Studio.EmailProviders.Exchange.DependencyInjection;

/// <summary>
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Configures email sending capabilities on DI container.
    /// </summary>
    /// <param name="this"></param>
    /// <param name="emailSenderOptions">Email sending configuration options</param>
    /// <returns></returns>
    public static IServiceCollection AddEmailSender(this IServiceCollection @this, EmailSenderOptions emailSenderOptions, ExchangeVersion exchangeVersion)
    {
        EnsureArg.IsNotNull(@this, nameof(@this));
        EnsureArg.IsNotNull(emailSenderOptions, nameof(emailSenderOptions));

        @this.AddSingleton<IEmailSender>(
            (_) =>
            {
                return new EmailSender(
                        exchangeVersion,
                        emailSenderOptions.Username!,
                        emailSenderOptions.Password!,
                        emailSenderOptions.WebServiceUrl!,
                        emailSenderOptions.FromEmailAddress!,
                        emailSenderOptions.FromEmailName!,
                        emailSenderOptions.UseEnforcedToEmailAddress ? emailSenderOptions.EnforcedToEmailAddress : null,
                        emailSenderOptions.SaveCopy,
                        emailSenderOptions.SaveCopyFolderId);
            });
        return @this;
    }

}

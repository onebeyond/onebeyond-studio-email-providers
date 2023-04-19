using EnsureThat;
using Microsoft.Exchange.WebServices.Data;
using Microsoft.Extensions.DependencyInjection;
using OneBeyond.Studio.EmailProviders.Domain;
using OneBeyond.Studio.EmailProviders.Office365.Options;

namespace OneBeyond.Studio.EmailProviders.Office365.DependencyInjection;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Configures email sending capabilities on DI container.
    /// </summary>
    /// <param name="this"></param>
    /// <param name="emailSenderOptions">Email sending configuration options</param>
    /// <returns></returns>
    public static IServiceCollection AddEmailSender(
        this IServiceCollection @this,
        EmailSenderOptions emailSenderOptions,
        ExchangeVersion exchangeVersion)
    {
        EnsureArg.IsNotNull(emailSenderOptions, nameof(emailSenderOptions));
        EnsureArg.IsNotNull(@this, nameof(@this));

        @this.AddSingleton<IEmailSender>(
            (_) =>
            {
                return new EmailSender(
                        exchangeVersion,
                        emailSenderOptions.Username!,
                        emailSenderOptions.Password!,
                        emailSenderOptions.FromEmailAddress!,
                        emailSenderOptions.FromEmailName!,
                        emailSenderOptions.UseEnforcedToEmailAddress ? emailSenderOptions.EnforcedToEmailAddress : null,
                        emailSenderOptions.DeliveryMethod!,
                        emailSenderOptions.EWS!.SaveCopy,
                        emailSenderOptions.EWS.SaveCopyFolderId,
                        emailSenderOptions.Smtp!.Port,
                        emailSenderOptions.Smtp.Host!,
                        emailSenderOptions.Smtp.EnableSsl
                        );
            });
        return @this;
    }
}

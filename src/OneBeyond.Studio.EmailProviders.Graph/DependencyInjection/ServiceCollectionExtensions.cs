using EnsureThat;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OneBeyond.Studio.EmailProviders.Domain;
using OneBeyond.Studio.EmailProviders.Graph.Options;

namespace OneBeyond.Studio.EmailProviders.Graph.DependencyInjection;

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
    public static IServiceCollection AddEmailSender(this IServiceCollection @this, EmailSenderOptions emailSenderOptions)
    {
        EnsureArg.IsNotNull(@this, nameof(@this));
        EnsureArg.IsNotNull(emailSenderOptions, nameof(emailSenderOptions));

        @this.AddSingleton(
            (serviceProvider) =>
            {
                return new EmailSender(
                    serviceProvider.GetRequiredService<ILoggerFactory>(),
                    emailSenderOptions.ClientId!,
                    emailSenderOptions.TenantId!,
                    emailSenderOptions.Secret!,
                    emailSenderOptions.SenderUserAzureId!
                    ) as IEmailSender;
            });
        return @this;
    }
}

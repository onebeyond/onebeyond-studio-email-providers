
using EnsureThat;
using Microsoft.Extensions.DependencyInjection;
using OneBeyond.Studio.EmailProviders.Domain;
using OneBeyond.Studio.EmailProviders.AwsSes.Options;

namespace OneBeyond.Studio.EmailProviders.AwsSes.DependencyInjection;

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

        @this.AddSingleton<IEmailSender>(
            (serviceProvider) =>
            {
                return new EmailSender(
                    emailSenderOptions.FromEmailAddress,
                    emailSenderOptions.UseEnforcedToEmailAddress
                        ? emailSenderOptions.EnforcedToEmailAddress
                        : default
                );
            });
        return @this;
    }
}

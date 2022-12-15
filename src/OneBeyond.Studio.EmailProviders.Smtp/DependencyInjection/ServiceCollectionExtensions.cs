using System;
using EnsureThat;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OneBeyond.Studio.EmailProviders.Domain;
using OneBeyond.Studio.EmailProviders.Smtp.Options;

namespace OneBeyond.Studio.EmailProviders.Smtp.DependencyInjection;

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
                var host = emailSenderOptions.Host
                    ?? throw new ArgumentNullException(nameof(emailSenderOptions.Host));

                var secureConnection = string.IsNullOrWhiteSpace(emailSenderOptions.SecureConnection)
                    ? MailKit.Security.SecureSocketOptions.Auto
                    : (MailKit.Security.SecureSocketOptions)Enum.Parse(
                        typeof(MailKit.Security.SecureSocketOptions),
                        emailSenderOptions.SecureConnection,
                        true);

                return new EmailSender(
                    serviceProvider.GetRequiredService<ILoggerFactory>(),
                    host,
                    emailSenderOptions.Port,
                    secureConnection,
                    emailSenderOptions.Username,
                    emailSenderOptions.Password,
                    emailSenderOptions.FromEmailAddress,
                    emailSenderOptions.FromEmailName,
                    emailSenderOptions.UseEnforcedToEmailAddress
                        ? emailSenderOptions.EnforcedToEmailAddress
                        : default,
                    emailSenderOptions.EnableProtocolLogging);
            });
        return @this;
    }

}

using System;
using EnsureThat;
using Microsoft.Extensions.DependencyInjection;
using OneBeyond.Studio.EmailProviders.Domain;
using OneBeyond.Studio.EmailProviders.SendGrid.Options;

namespace OneBeyond.Studio.EmailProviders.SendGrid.DependencyInjection;

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
            (sp) =>
            {
                return new EmailSender(
                        emailSenderOptions.Key!,
                        emailSenderOptions.FromEmailAddress!,
                        emailSenderOptions.FromEmailName!,
                        emailSenderOptions.UseEnforcedToEmailAddress ? emailSenderOptions.EnforcedToEmailAddress : null) as IEmailSender;
            });
        return @this;
    }

}

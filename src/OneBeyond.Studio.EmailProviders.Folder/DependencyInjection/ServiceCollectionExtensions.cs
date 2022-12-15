using System;
using EnsureThat;
using Microsoft.Extensions.DependencyInjection;
using OneBeyond.Studio.EmailProviders.Domain;
using OneBeyond.Studio.EmailProviders.Folder.Options;

namespace OneBeyond.Studio.EmailProviders.Folder.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEmailSender(this IServiceCollection @this, EmailSenderOptions emailSenderOptions)
    {
        EnsureArg.IsNotNull(@this, nameof(@this));
        EnsureArg.IsNotNull(emailSenderOptions, nameof(emailSenderOptions));

        @this.AddSingleton<IEmailSender>(
            (_) =>
            {
                var folder = emailSenderOptions.Folder
                    ?? throw new ArgumentNullException(nameof(emailSenderOptions.Folder));

                return new EmailSender(
                    folder,
                    emailSenderOptions.FromEmailAddress,
                    emailSenderOptions.FromEmailName,
                    emailSenderOptions.UseEnforcedToEmailAddress
                        ? emailSenderOptions.EnforcedToEmailAddress
                        : default);
            });

        return @this;
    }
}

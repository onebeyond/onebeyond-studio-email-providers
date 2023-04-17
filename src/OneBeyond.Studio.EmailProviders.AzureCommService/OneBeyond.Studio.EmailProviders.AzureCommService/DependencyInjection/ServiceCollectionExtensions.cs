using System;
using EnsureThat;
using Microsoft.Extensions.DependencyInjection;
using OneBeyond.Studio.EmailProviders.Domain;
using OneBeyond.Studio.EmailProviders.AzureCommService.Options;
using Microsoft.Extensions.Logging;

namespace OneBeyond.Studio.EmailProviders.AzureCommService.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEmailSender(this IServiceCollection @this, EmailSenderOptions emailSenderOptions)
    {
        EnsureArg.IsNotNull(@this, nameof(@this));
        EnsureArg.IsNotNull(emailSenderOptions, nameof(emailSenderOptions));

        @this.AddSingleton(
            (serviceProvider) =>
            {
                return new EmailSender(
                        serviceProvider.GetRequiredService<ILoggerFactory>(),
                        emailSenderOptions.CommunicationServiceConnectionString,
                        emailSenderOptions.FromEmailAddress,
                        emailSenderOptions.UseEnforcedToEmailAddress ? emailSenderOptions.EnforcedToEmailAddress : null) as IEmailSender;
            });
        return @this;
    }

}

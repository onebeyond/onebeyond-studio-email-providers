using EnsureThat;
using Microsoft.Extensions.DependencyInjection;
using OneBeyond.Studio.EmailProviders.AzureCommService.Options;
using OneBeyond.Studio.EmailProviders.Domain;

namespace OneBeyond.Studio.EmailProviders.AzureCommService.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEmailSender(this IServiceCollection @this, EmailSenderOptions emailSenderOptions)
    {
        EnsureArg.IsNotNull(@this, nameof(@this));
        EnsureArg.IsNotNull(emailSenderOptions, nameof(emailSenderOptions));

        @this.AddSingleton(
            (_) =>
            {
                return new EmailSender(
                        emailSenderOptions.CommunicationServiceConnectionString,
                        emailSenderOptions.FromEmailAddress!,
                        emailSenderOptions.UseEnforcedToEmailAddress ? emailSenderOptions.EnforcedToEmailAddress : null,
                        emailSenderOptions.DoNotWaitTillOperationCompleted) as IEmailSender;
            });
        return @this;
    }

}

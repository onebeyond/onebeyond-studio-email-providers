using Autofac.Extensions.DependencyInjection;
using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OneBeyond.Studio.EmailProviders.Domain;
using OneBeyond.Studio.EmailProviders.SendGrid.Options;
using OneBeyond.Studio.EmailProviders.SendGrid.DependencyInjection;
using System.Net.Mail;

const string _defaultFromEmail = "<ADD-VERIFIED-EMAIL>";
const string _defaultToEmail = "<ADD-EMAIL>";

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", true)
    .Build();

var serviceCollection = new ServiceCollection();

serviceCollection.AddLogging();

var section = configuration.GetSection("EmailSender:SendGrid")
    ?? throw new Exception($"Unable to find section EmailSender:SendGrid");

var options = section.Get<EmailSenderOptions>();

serviceCollection.AddEmailSender(options!);

var containerBuilder = new ContainerBuilder();

containerBuilder.Populate(serviceCollection);

var serviceProvider = new AutofacServiceProvider(containerBuilder.Build());

using (var serviceScope = serviceProvider.CreateScope())
{
    var emailSender = serviceScope.ServiceProvider.GetRequiredService<IEmailSender>();

    var correlationId1 = await SendPlainTextEmailAsync(emailSender);
    var correlationId2 = await SendHtmlEmailAsync(emailSender);
    var correlationId3 = await SendForcedToEmailAsync(emailSender);
    var correlationId4 = await SendAttachmentsEmailAsync(emailSender);
    await SendMultipleEmailsAsync(emailSender);
}

static async Task SendMultipleEmailsAsync(IEmailSender emailSender, CancellationToken ct = default)
{
    for (var i = 0; i < 100; i++)
    {
        var mail = new MailMessage(_defaultFromEmail, _defaultToEmail)
        {
            Subject = $"Test {i}",
            Body = $"Test message {i}"
        };

        await emailSender.SendEmailAsync(mail, ct);
    }
}

static Task<string?> SendPlainTextEmailAsync(IEmailSender emailSender, CancellationToken ct = default)
{
    var mail = new MailMessage(_defaultFromEmail, _defaultToEmail)
    {
        Subject = "Test",
        Body = "Test message",
        IsBodyHtml = true
    };

    return emailSender.SendEmailAsync(mail, ct);
}

static Task<string?> SendHtmlEmailAsync(IEmailSender emailSender, CancellationToken ct = default)
{
    var mail = new MailMessage(_defaultFromEmail, _defaultToEmail)
    {
        Subject = "Test",
        Body = "<h1>Test message</h1>",
        IsBodyHtml = true
    };

    return emailSender.SendEmailAsync(mail, ct);
}

static Task<string?> SendForcedToEmailAsync(IEmailSender emailSender, CancellationToken ct = default)
{
    var mail = new MailMessage(_defaultFromEmail, "unknown.address@nowhere.com")
    {
        Subject = "Test",
        Body = "Test message",
        IsBodyHtml = true
    };

    return emailSender.SendEmailAsync(mail, ct);
}

static Task<string?> SendAttachmentsEmailAsync(IEmailSender emailSender, CancellationToken ct = default)
{
    var mail = new MailMessage(_defaultFromEmail, _defaultToEmail)
    {
        Subject = "Test with attachment",
        Body = "Test message with Attachment",
        IsBodyHtml = false
    };

    mail.Attachments.Add(new Attachment("Attachments/attachment.png", "image/png"));

    return emailSender.SendEmailAsync(mail, ct);
}

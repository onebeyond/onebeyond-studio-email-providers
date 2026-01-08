<p>
  <a href="https://one-beyond.com">
    <img src="Logo.png" width="300" alt="One Beyond" />
  </a>
</p>

[![Nuget version](https://img.shields.io/nuget/v/OneBeyond.Studio.EmailProviders.Domain?style=plastic)](https://www.nuget.org/packages/OneBeyond.Studio.EmailProviders.Domain)
[![Nuget downloads](https://img.shields.io/nuget/dt/OneBeyond.Studio.EmailProviders.Domain?style=plastic)](https://www.nuget.org/packages/OneBeyond.Studio.EmailProviders.Domain)
[![License](https://img.shields.io/github/license/OneBeyond/onebeyond-studio-email-providers?style=plastic)](LICENSE)
[![Maintainability](https://api.codeclimate.com/v1/badges/60e9e03f64f5f9368561/maintainability)](https://codeclimate.com/github/onebeyond/onebeyond-studio-email-providers/maintainability)
[![Test Coverage](https://api.codeclimate.com/v1/badges/60e9e03f64f5f9368561/test_coverage)](https://codeclimate.com/github/onebeyond/onebeyond-studio-email-providers/test_coverage)

# Introduction
On Beyond Studio Email Providers is a set of .NET libraries that helps you to abstract send e-mail logic in your application.
At this moment, we support the following types of e-mail providers:
- [Folder](https://www.nuget.org/packages/OneBeyond.Studio.EmailProviders.Folder)
- [SendGrid](https://www.nuget.org/packages/OneBeyond.Studio.EmailProviders.SendGrid)
- [SMTP](https://www.nuget.org/packages/OneBeyond.Studio.EmailProviders.Smtp)
- [Graph](https://www.nuget.org/packages/OneBeyond.Studio.EmailProviders.Graph)
- [Office365](https://www.nuget.org/packages/OneBeyond.Studio.EmailProviders.Office365)
- [Exchange](https://www.nuget.org/packages/OneBeyond.Studio.EmailProviders.Exchange)

### Supported .NET version:

10.0

### Installation

The library that contains IEmailSender abstraction:

`dotnet new install OneBeyond.Studio.EmailProviders.Domain`

Libraries that contain particular implementation of IEmailSender (depending on your needs):

`dotnet new install OneBeyond.Studio.EmailProviders.Folder`

`dotnet new install OneBeyond.Studio.EmailProviders.SendGrid`

`dotnet new install OneBeyond.Studio.EmailProviders.Smtp`

`dotnet new install OneBeyond.Studio.EmailProviders.Graph`

`dotnet new install OneBeyond.Studio.EmailProviders.Office365`

`dotnet new install OneBeyond.Studio.EmailProviders.Exchange`

### Documentation

For more detailed documentation, please refer to our [Wiki](https://github.com/onebeyond/onebeyond-studio-email-providers/wiki)

### Contributing

If you want to contribute, we are currently accepting PRs and/or proposals/discussions in the issue tracker.

<p align="center">
  <a href="https://one-beyond.com">
    <img src="Logo.png" width="700" alt="One Beyond" />
  </a>
</p>

# One Beyond Studio Email Providers Dependencies

```mermaid
 graph BT;
 B1[EmailProviders.AzureCommService] --> A1[EmailProviders.Domain];
 C1[EmailProviders.Exchange] --> A1[EmailProviders.Domain];
 D1[EmailProviders.Folder] --> A1[EmailProviders.Domain];
 E1[EmailProviders.Graph] --> A1[EmailProviders.Domain];
 F1[EmailProviders.Office365] --> A1[EmailProviders.Domain];
 G1[EmailProviders.SendGrid] --> A1[EmailProviders.Domain];
 H1[EmailProviders.Smtp] --> A1[EmailProviders.Domain];
```

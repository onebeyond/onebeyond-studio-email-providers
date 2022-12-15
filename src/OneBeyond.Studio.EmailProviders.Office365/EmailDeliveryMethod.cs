using System.Net.Mail;

namespace OneBeyond.Studio.EmailProviders.Office365;

/// <summary>
/// Email delievry methods to be used for the <see cref="EmailSenderOptions.DeliveryMethod"/> field.
/// </summary>
public static class DeliveryMethod
{
    /// <summary>
    /// </summary>
    public const string EWS = "EWS";

    /// <summary>
    /// </summary>
    public const string Smtp = "Smtp";

}

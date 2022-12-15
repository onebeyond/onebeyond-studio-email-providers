using System;
using System.Runtime.Serialization;

namespace OneBeyond.Studio.EmailProviders.Domain.Exceptions;

/// <summary>
/// </summary>
[Serializable]
public sealed class EmailSenderException : Exception
{
    /// <summary>
    /// </summary>
    public EmailSenderException()
        : base()
    {
    }

    /// <summary>
    /// </summary>
    /// <param name="message"></param>
    public EmailSenderException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// </summary>
    /// <param name="message"></param>
    /// <param name="innerException"></param>
    public EmailSenderException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    /// <summary>
    /// </summary>
    /// <param name="info"></param>
    /// <param name="context"></param>
    private EmailSenderException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}

using CommandLineParsing.Output;
using System;

namespace CommandLineParsing
{
    public class MessageException : Exception
    {
        public MessageException(ConsoleString message) : base(message.Content)
        {
            Message = message ?? throw new ArgumentNullException(nameof(message));
        }
        public MessageException(ConsoleString message, Exception inner) : base(message.Content, inner)
        {
            Message = message ?? throw new ArgumentNullException(nameof(message));
        }

        protected MessageException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context)
        {
        }

        public new ConsoleString Message { get; }
    }

    public static class MessageExtension
    {
        public static T GetValueOrThrow<T>(this Message<T> message)
        {
            if (message.IsError)
                throw new MessageException(message.Content);
            else
                return message.Value;
        }

        public static void ThrowIfError<T>(this Message<T> message)
        {
            if (message.IsError)
                throw new MessageException(message.Content);
        }
        public static void ThrowIfError(this Message message)
        {
            if (message.IsError)
                throw new MessageException(message.Content);
        }
    }
}

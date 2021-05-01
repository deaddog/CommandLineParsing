using System;

namespace ConsoleTools
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

        public new ConsoleString Message { get; }
    }
}

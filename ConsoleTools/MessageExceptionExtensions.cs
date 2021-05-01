namespace ConsoleTools
{
    public static class MessageExceptionExtensions
    {
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

        public static T GetValueOrThrow<T>(this Message<T> message)
        {
            if (message.IsError)
                throw new MessageException(message.Content);
            else
                return message.Value;
        }
    }
}

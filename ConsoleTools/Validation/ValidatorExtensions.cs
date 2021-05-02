namespace ConsoleTools.Validation
{
    public static class ValidatorExtensions
    {
        public static Message<T> Validate<T>(this IValidator<T> validator, Message<T> message)
        {
            if (message.IsError)
                return message;
            else
            {
                var msg = validator.Validate(message.Value);

                if (msg.IsError)
                    return new Message<T>(msg.Content);
                else
                    return message;
            }
        }
    }
}

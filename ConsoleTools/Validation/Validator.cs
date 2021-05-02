using System;

namespace ConsoleTools.Validation
{
    public class Validator<T> : IValidator<T>
    {
        public static IValidator<T> NoRules => AndValidator<T>.Empty;

        public static IValidator<T> Create(Func<T, Message> validator) => new Validator<T>(validator);

        private readonly Func<T, Message> _validator;

        public Validator(Func<T, Message> validator)
        {
            _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        }

        public Message Validate(T item) => _validator(item);
    }
}

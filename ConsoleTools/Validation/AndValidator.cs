using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace ConsoleTools.Validation
{
    public class AndValidator<T> : IValidator<T>
    {
        public static IValidator<T> Empty => new AndValidator<T>(ImmutableList<IValidator<T>>.Empty);

        public static IValidator<T> Create(IEnumerable<IValidator<T>> validators) => new AndValidator<T>(validators.ToImmutableList());
        public static IValidator<T> Create(params IValidator<T>[] validators) => new AndValidator<T>(validators.ToImmutableList());

        private readonly IImmutableList<IValidator<T>> _validators;

        public AndValidator(IImmutableList<IValidator<T>> validators)
        {
            _validators = validators ?? throw new ArgumentNullException(nameof(validators));

            if (validators.Any(v => v is null))
                throw new ArgumentNullException(nameof(validators), "A validator method cannot be null.");

            for (int i = 0; i < _validators.Count; i++)
            {
                while (_validators[i] is AndValidator<T> comp)
                {
                    _validators = _validators
                        .RemoveAt(i)
                        .InsertRange(i, comp._validators);
                }
            }
        }

        public Message Validate(T item)
        {
            return _validators
                .Select(v => v.Validate(item))
                .FirstOrDefault(m => m.IsError) ?? Message.NoError;
        }
    }
}

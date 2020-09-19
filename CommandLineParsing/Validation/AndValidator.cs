using System;
using System.Collections.Immutable;
using System.Linq;

namespace CommandLineParsing.Validation
{
    /// <summary>
    /// Combines multiple <see cref="IValidator"/>s with an AND operation (short circuits).
    /// </summary>
    public class AndValidator : IValidator
    {
        private readonly IImmutableList<IValidator> _validators;

        /// <summary>
        /// Initializes a new instance of the <see cref="AndValidator"/> class.
        /// </summary>
        /// <param name="validators">The validators that should be executed. If empty, <see cref="Message.NoError"/> will be the validation result.</param>
        public AndValidator(IImmutableList<IValidator> validators)
        {
            _validators = validators ?? throw new ArgumentNullException(nameof(validators));

            if (validators.Any(v => v is null))
                throw new ArgumentNullException(nameof(validators), "A validator method cannot be null.");

            for (int i = 0; i < _validators.Count; i++)
                while (_validators[i] is AndValidator and)
                {
                    _validators = _validators.RemoveAt(i);
                    _validators = _validators.InsertRange(i, and._validators);
                }
        }

        /// <summary>
        /// Executes each validator in sequence.
        /// </summary>
        /// <returns>A <see cref="Message"/> indicating the result of the first validation that fails, or <see cref="Message.NoError"/>.</returns>
        public Message Validate()
        {
            return _validators
                .Select(v => v.Validate())
                .FirstOrDefault(m => m.IsError) ?? Message.NoError;
        }
    }

    /// <summary>
    /// Combines multiple <see cref="IValidator{T}"/>s with an AND operation (short circuits).
    /// </summary>
    /// <typeparam name="T">The type of elements the validator can validate.</typeparam>
    public class AndValidator<T> : IValidator<T>
    {
        private readonly IImmutableList<IValidator<T>> _validators;

        /// <summary>
        /// Initializes a new instance of the <see cref="AndValidator{T}"/> class.
        /// </summary>
        /// <param name="validators">The validators that should be executed. If empty, <see cref="Message.NoError"/> will be the validation result.</param>
        public AndValidator(IImmutableList<IValidator<T>> validators)
        {
            _validators = validators ?? throw new ArgumentNullException(nameof(validators));

            if (validators.Any(v => v is null))
                throw new ArgumentNullException(nameof(validators), "A validator method cannot be null.");

            for (int i = 0; i < _validators.Count; i++)
                while (_validators[i] is AndValidator<T> and)
                {
                    _validators = _validators.RemoveAt(i);
                    _validators = _validators.InsertRange(i, and._validators);
                }
        }

        /// <summary>
        /// Executes each validator in sequence.
        /// </summary>
        /// <param name="item">The item being validated against the each of the validators rules.</param>
        /// <returns>A <see cref="Message"/> indicating the result of the first validation that fails, or <see cref="Message.NoError"/>.</returns>
        public Message Validate(T item)
        {
            return _validators
                .Select(v => v.Validate(item))
                .FirstOrDefault(m => m.IsError) ?? Message.NoError;
        }
    }
}

using System;

namespace CommandLineParsing.Validation
{
    /// <summary>
    /// Wraps a <see cref="Func{Message}"/> as a <see cref="IValidator"/>.
    /// </summary>
    public class FuncValidator : IValidator
    {
        private readonly Func<Message> _validator;

        /// <summary>
        /// Initializes a new instance of the <see cref="FuncValidator"/> class.
        /// </summary>
        /// <param name="validator">The validation method that should represent this validator.</param>
        public FuncValidator(Func<Message> validator)
        {
            _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        }

        /// <summary>
        /// Executes the validators inner function.
        /// </summary>
        /// <returns>A <see cref="Message"/> indicating the result of validation.</returns>
        public Message Validate() => _validator();
    }

    /// <summary>
    /// Wraps a <see cref="Func{T, Message}"/> as a <see cref="IValidator{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of elements the validator can validate.</typeparam>
    public class FuncValidator<T> : IValidator<T>
    {
        private readonly Func<T, Message> _validator;

        /// <summary>
        /// Initializes a new instance of the <see cref="FuncValidator{T}"/> class.
        /// </summary>
        /// <param name="validator">The validation method that should represent this validator.</param>
        public FuncValidator(Func<T, Message> validator)
        {
            _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        }

        /// <summary>
        /// Executes the validators inner function.
        /// </summary>
        /// <param name="item">The item being validated against the validators rules.</param>
        /// <returns>A <see cref="Message"/> indicating the result of validation.</returns>
        public Message Validate(T item) => _validator(item);
    }
}

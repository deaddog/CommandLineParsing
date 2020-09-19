using System;

namespace CommandLineParsing.Validation
{
    /// <summary>
    /// Provides a context-free validator composer.
    /// Use fluent extension methods for compose validation rules.
    /// Initialize using <see cref="NoRules"/>.
    /// </summary>
    public class ValidatorComposer : IValidatorComposer<ValidatorComposer>
    {
        /// <summary>
        /// Creates a <see cref="ValidatorComposer"/> based on <see cref="CommandLineParsing.Validation.Validator.NoRules"/>, which always returns <see cref="Message.NoError"/>.
        /// </summary>
        public static ValidatorComposer NoRules => new ValidatorComposer(CommandLineParsing.Validation.Validator.NoRules);

        private ValidatorComposer(IValidator validator)
        {
            Validator = validator ?? throw new ArgumentNullException(nameof(validator));
        }

        /// <summary>
        /// Get the validator defined by the composer.
        /// </summary>
        public IValidator Validator { get; }

        ValidatorComposer IValidatorComposer<ValidatorComposer>.WithValidator(IValidator validator)
        {
            return new ValidatorComposer(validator);
        }
    }

    /// <summary>
    /// Provides a context-free validator for extension.
    /// Use fluent extension methods for compose validation rules.
    /// Initialize using <see cref="NoRules"/>.
    /// <typeparam name="T">The type of elements the final validator will be able to validate.</typeparam>
    public class ValidatorComposer<T> : IValidatorComposer<T, ValidatorComposer<T>>
    {
        /// <summary>
        /// Creates a <see cref="ValidatorComposer"/> based on <see cref="CommandLineParsing.Validation.Validator{T}.NoRules"/>, which always returns <see cref="Message.NoError"/>.
        /// </summary>
        public static ValidatorComposer<T> NoRules => new ValidatorComposer<T>(CommandLineParsing.Validation.Validator<T>.NoRules);

        private ValidatorComposer(IValidator<T> validator)
        {
            Validator = validator ?? throw new ArgumentNullException(nameof(validator));
        }

        /// <summary>
        /// Get the validator defined by the composer.
        /// </summary>
        public IValidator<T> Validator { get; }

        ValidatorComposer<T> IValidatorComposer<T,ValidatorComposer<T>>.WithValidator(IValidator<T> validator)
        {
            return new ValidatorComposer<T>(validator);
        }
    }
}

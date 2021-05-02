using System;

namespace ConsoleTools.Validation
{
    public class ValidatorComposer<T> : IValidatorComposer<T, ValidatorComposer<T>>
    {
        public ValidatorComposer(IValidator<T> validator)
        {
            Validator = validator ?? throw new ArgumentNullException(nameof(validator));
        }

        public IValidator<T> Validator { get; }

        public ValidatorComposer<T> WithValidator(IValidator<T> validator)
        {
            return new ValidatorComposer<T>(validator);
        }
    }
}

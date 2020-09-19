namespace CommandLineParsing.Validation
{
    /// <summary>
    /// Defines a method for validating conditions in a given context.
    /// </summary>
    public interface IValidator
    {
        /// <summary>
        /// Executes the validators validation rules.
        /// </summary>
        /// <returns>A <see cref="Message"/> indicating the result of validation.</returns>
        Message Validate();
    }

    /// <summary>
    /// Defines a method for validating conditions in a given context against values of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of elements the validator can validate.</typeparam>
    public interface IValidator<T>
    {
        /// <summary>
        /// Executes the validators validation rules.
        /// </summary>
        /// <param name="item">The item being validated against the validators rules.</param>
        /// <returns>A <see cref="Message"/> indicating the result of validation.</returns>
        Message Validate(T item);
    }
}

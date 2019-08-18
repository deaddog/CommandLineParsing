namespace CommandLineParsing.Validation
{
    /// <summary>
    /// Provides a base interface for fluently extending a <see cref="IValidator"/> in a custom composition context.
    /// </summary>
    /// <typeparam name="TReturn">
    /// The type of items the composer can generate.
    /// If <typeparamref name="TReturn"/> is itself <see cref="IValidatorComposer{TReturn}"/> fluent configuration will be supported.
    /// </typeparam>
    public interface IValidatorComposer<out TReturn>
    {
        /// <summary>
        /// Get the validator defined by the composer.
        /// </summary>
        IValidator Validator { get; }
        /// <summary>
        /// Creates a <typeparamref name="TReturn"/> item with the specified validator.
        /// </summary>
        /// <param name="validator">The validator the returned <typeparamref name="TReturn"/> item should expose.</param>
        /// <returns>A <typeparamref name="TReturn"/> that employs the validation rules defined by <paramref name="validator"/>.</returns>
        TReturn WithValidator(IValidator validator);
    }

    /// <summary>
    /// Provides a base interface for fluently extending a <see cref="IValidator{T}"/> in a custom composition context.
    /// </summary>
    /// <typeparam name="T">The type of elements the final validator will be able to validate.</typeparam>
    /// <typeparam name="TReturn">
    /// The type of items the composer can generate.
    /// If <typeparamref name="TReturn"/> is itself <see cref="IValidatorComposer{T, TReturn}"/> fluent configuration will be supported.</typeparam>
    public interface IValidatorComposer<T, out TReturn>
    {
        /// <summary>
        /// Get the validator defined by the composer.
        /// </summary>
        IValidator<T> Validator { get; }
        /// <summary>
        /// Creates a <typeparamref name="TReturn"/> item with the specified validator.
        /// </summary>
        /// <param name="validator">The validator the returned <typeparamref name="TReturn"/> item should expose.</param>
        /// <returns>A <typeparamref name="TReturn"/> that employs the validation rules defined by <paramref name="validator"/>.</returns>
        TReturn WithValidator(IValidator<T> validator);
    }
}

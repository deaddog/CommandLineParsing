namespace CommandLineParsing.Output.Formatting
{
    /// <summary>
    /// Represents a builder that fluently constructs a formatter.
    /// </summary>
    /// <typeparam name="T">The type of elements the resulting formatter will support.</typeparam>
    public interface IFormatterComposer<T>
    {
        /// <summary>
        /// Gets the <see cref="IFormatter{T}"/> as described by this <see cref="IFormatterComposer{T}"/>.
        /// </summary>
        /// <returns>A <see cref="IFormatter{T}"/> using the rules defined by this <see cref="IFormatterComposer{T}"/>.</returns>
        IFormatter<T> GetFormatter();

        /// <summary>
        /// Adds a variable to the <see cref="IFormatterComposer{T}"/>.
        /// </summary>
        /// <param name="variable">The variable which the resulting formatter should support.</param>
        /// <returns>A <see cref="IVariableComposer{T}"/> for further configuration.</returns>
        IVariableComposer<T> With(Variable<T> variable);
        /// <summary>
        /// Adds a condition to the <see cref="IFormatterComposer{T}"/>.
        /// </summary>
        /// <param name="condition">The condition which the resulting formatter should support.</param>
        /// <returns>A <see cref="IConditionComposer{T}"/> for further configuration.</returns>
        IConditionComposer<T> With(Condition<T> condition);
        /// <summary>
        /// Adds a function to the <see cref="IFormatterComposer{T}"/>.
        /// </summary>
        /// <param name="function">The function which the resulting formatter should support.</param>
        /// <returns>A <see cref="IFunctionComposer{T}"/> for further configuration.</returns>
        IFunctionComposer<T> With(Function<T> function);
    }
}

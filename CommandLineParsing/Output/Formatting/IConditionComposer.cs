namespace CommandLineParsing.Output.Formatting
{
    /// <summary>
    /// Represents a builder that fluently configures a conditions behaviour in a format.
    /// </summary>
    /// <typeparam name="T">The type of elements the resulting formatter will support.</typeparam>
    public interface IConditionComposer<T> : IFormatterComposer<T>
    {
        /// <summary>
        /// The condition this <see cref="IConditionComposer{T}"/> can configure.
        /// </summary>
        Condition<T> Condition { get; }
    }
}

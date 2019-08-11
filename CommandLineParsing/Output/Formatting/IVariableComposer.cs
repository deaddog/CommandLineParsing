namespace CommandLineParsing.Output.Formatting
{
    /// <summary>
    /// Represents a builder that fluently configures a variables behaviour in a format.
    /// </summary>
    /// <typeparam name="T">The type of elements the resulting formatter will support.</typeparam>
    public interface IVariableComposer<T> : IFormatterComposer<T>
    {
        /// <summary>
        /// The variable this <see cref="IVariableComposer{T}"/> can configure.
        /// </summary>
        Variable<T> Variable { get; }
    }
}

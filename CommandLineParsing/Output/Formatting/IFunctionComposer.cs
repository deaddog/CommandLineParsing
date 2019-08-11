namespace CommandLineParsing.Output.Formatting
{
    /// <summary>
    /// Represents a builder that fluently configures a functions behaviour in a format.
    /// </summary>
    /// <typeparam name="T">The type of elements the resulting formatter will support.</typeparam>
    public interface IFunctionComposer<T> : IFormatterComposer<T>
    {
        /// <summary>
        /// The function this <see cref="IFunctionComposer{T}"/> can configure.
        /// </summary>
        Function<T> Function { get; }
    }
}

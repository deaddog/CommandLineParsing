namespace CommandLineParsing.Output.Formatting
{
    /// <summary>
    /// Defines processing of a format-structure into console output.
    /// </summary>
    /// <typeparam name="T">The type of elements that this formatter applies to.</typeparam>
    public interface IFormatter<in T>
    {
        /// <summary>
        /// Applies <paramref name="format"/> to <paramref name="item"/>.
        /// </summary>
        /// <param name="format">A tree-structure of format-elements.</param>
        /// <param name="item">The data element to which the format is applied.</param>
        /// <returns>A <see cref="ConsoleString"/> with all text and color details, as provided by <paramref name="format"/>.</returns>
        ConsoleString Format(Formatting.Structure.FormatElement format, T item);
    }
}

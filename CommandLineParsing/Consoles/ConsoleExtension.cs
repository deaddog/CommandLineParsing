namespace CommandLineParsing.Consoles
{
    /// <summary>
    /// Provides console methods generic to any implementation of <see cref="IConsole"/>.
    /// </summary>
    public static class ConsoleExtension
    {
        /// <summary>
        /// Writes the text representation of the specified object to <paramref name="console"/>.
        /// </summary>
        /// <param name="console">The console used for writing.</param>
        /// <param name="value">The value to write, or null.</param>
        public static void Write<T>(this IConsole console, T value) => console.Write(value?.ToString() ?? "");

        /// <summary>
        /// Writes the current line terminator to <paramref name="console"/>.
        /// </summary>
        /// <param name="console">The console used for writing.</param>
        public static void WriteLine(this IConsole console) => console.WriteLine("");
        /// <summary>
        /// Writes the text representation of the specified object, followed by the current line terminator, to <paramref name="console"/>.
        /// </summary>
        /// <param name="console">The console used for writing.</param>
        /// <param name="value">The value to write, or null.</param>
        public static void WriteLine<T>(this IConsole console, T value) => console.WriteLine(value?.ToString() ?? "");
    }
}

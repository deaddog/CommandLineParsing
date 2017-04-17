namespace CommandLineParsing.Input
{
    /// <summary>
    /// Represent the event handler that handles <see cref="ConsoleReader.TextChanged"/> events.
    /// </summary>
    /// <param name="reader">The console reader associated with the event.</param>
    /// <param name="oldText">The old <see cref="ConsoleReader.Text"/> from before the event was executed.</param>
    public delegate void ConsoleReaderTextChanged(ConsoleReader reader, string oldText);
}

using System.Collections.Generic;

namespace CommandLineParsing
{
    /// <summary>
    /// Represents a collection of text-lines that are printable in the console.
    /// </summary>
    public class ConsoleCache
    {
        private ConsoleLine[] lines;

        private ConsoleCache(ConsoleLine[] lines)
        {
            this.lines = lines;
        }
    }
}

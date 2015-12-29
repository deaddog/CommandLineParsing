using System;
using System.Collections.Generic;

namespace CommandLineParsing
{
    /// <summary>
    /// Represents a collection of text-lines that are printable in the console.
    /// </summary>
    public class ConsoleCache
    {
        internal class Builder
        {
            private List<ConsoleLine> lines = new List<ConsoleLine>();
            private ConsoleLine current;

            public Builder()
            {
                this.lines = new List<ConsoleLine>();
                this.current = new ConsoleLine();
            }

            private void addCurrent()
            {
                lines.Add(current);
                current = new ConsoleLine();
            }

            public void WriteString(string value)
            {
                throw new NotImplementedException();
            }

            public ConsoleCache ConstructCache()
            {
                if (!current.Empty)
                    addCurrent();

                var arr = lines.ToArray();
                lines.Clear();

                return new ConsoleCache(arr);
            }
        }

        private ConsoleLine[] lines;

        private ConsoleCache(ConsoleLine[] lines)
        {
            this.lines = lines;
        }
    }
}

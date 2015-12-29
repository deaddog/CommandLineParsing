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
            private static readonly char[] control = new char[] { '\n', '\t', '\r', '\b', '\a' };

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
                if (value == null || value.Length == 0)
                    return;

                int index = 0;
                while (index < value.Length)
                {
                    switch (value[index])
                    {
                        case '\n':
                            throw new NotImplementedException();

                        case '\t':
                            throw new NotImplementedException();

                        case '\r':
                            throw new NotImplementedException();

                        case '\b':
                            throw new NotImplementedException();

                        case '\a':
                            throw new NotImplementedException();

                        default:
                            throw new NotImplementedException();
                    }
                }
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

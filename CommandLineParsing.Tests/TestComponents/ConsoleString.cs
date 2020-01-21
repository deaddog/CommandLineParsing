using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace CommandLineParsing.Tests.TestComponents
{
    public class ConsoleString
    {
        public ConsoleString(ConsolePoint position, IEnumerable<ConsoleSegment> segments)
        {
            Position = position;
            Segments = segments.ToImmutableList();
        }

        public ConsolePoint Position { get; }
        public IImmutableList<ConsoleSegment> Segments { get; }

        public string Text => string.Join("", Segments.Select(x => x.Text));
    }
}

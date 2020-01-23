using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace CommandLineParsing.Tests.TestComponents
{
    public class ConsoleString : IEquatable<CommandLineParsing.Output.ConsoleString>
    {
        private readonly CommandLineParsing.Output.ConsoleString _outputString;

        public ConsoleString(ConsolePoint position, IEnumerable<ConsoleSegment> segments)
        {
            Position = position;
            Segments = segments.ToImmutableList();

            _outputString = new CommandLineParsing.Output.ConsoleString(Segments.Select(Map));
        }

        public ConsolePoint Position { get; }
        public IImmutableList<ConsoleSegment> Segments { get; }

        public string Text => string.Join("", Segments.Select(x => x.Text));

        public bool Equals(CommandLineParsing.Output.ConsoleString other)
        {
            return _outputString.Equals(other);
        }

        private CommandLineParsing.Output.ConsoleStringSegment Map(ConsoleSegment segment)
        {
            var foreground = segment.Foreground == ConsoleColor.Gray ? string.Empty : segment.Foreground.ToString();
            var background = segment.Background == ConsoleColor.Black ? string.Empty : segment.Background.ToString();

            var color = CommandLineParsing.Output.Color.Parse($"{foreground}|{background}");

            return new CommandLineParsing.Output.ConsoleStringSegment
            (
                content: segment.Text,
                color: color
            );
        }
    }
}

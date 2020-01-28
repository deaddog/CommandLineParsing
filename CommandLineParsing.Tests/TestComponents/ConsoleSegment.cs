using System;

namespace CommandLineParsing.Tests.TestComponents
{
    public class ConsoleSegment
    {
        public ConsoleSegment(string text, ConsoleColor foreground, ConsoleColor background)
        {
            Text = text;
            Foreground = foreground;
            Background = background;
        }

        public string Text { get; }
        public ConsoleColor Foreground { get; }
        public ConsoleColor Background { get; }
    }
}

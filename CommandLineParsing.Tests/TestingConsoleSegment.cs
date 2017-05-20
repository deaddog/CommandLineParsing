using System;

namespace CommandLineParsing.Tests
{
    public class TestingConsoleSegment
    {
        public string Text { get; }
        public ConsoleColor Foreground { get; }
        public ConsoleColor Background { get; }

        public TestingConsoleSegment(string text, ConsoleColor foreground, ConsoleColor background)
        {
            Text = text;
            Foreground = foreground;
            Background = background;
        }
    }
}

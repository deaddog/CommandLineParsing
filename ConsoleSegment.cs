using System;

namespace CommandLineParsing
{
    internal class ConsoleSegment
    {
        public readonly ConsoleColor? ForegroundColor;
        public readonly ConsoleColor? BackgroundColor;
        public readonly string Text;

        public ConsoleSegment(string text)
        {
            this.Text = text;
            this.ForegroundColor = null;
            this.BackgroundColor = null;
        }
        public ConsoleSegment(string text, ConsoleColor foreground, ConsoleColor background)
        {
            this.Text = text;
            this.ForegroundColor = foreground;
            this.BackgroundColor = background;
        }
        private ConsoleSegment(string text, ConsoleColor? foreground, ConsoleColor? background)
        {
            this.Text = text;
            this.ForegroundColor = foreground;
            this.BackgroundColor = background;
        }

        public override string ToString() => Text;
    }
}

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

        public static Tuple<ConsoleSegment, ConsoleSegment> Split(ConsoleSegment segment, int lengthOfFirst)
        {
            var first = new ConsoleSegment(segment.Text.Substring(0, lengthOfFirst), segment.ForegroundColor, segment.BackgroundColor);
            var second = new ConsoleSegment(segment.Text.Substring(lengthOfFirst), segment.ForegroundColor, segment.BackgroundColor);

            return Tuple.Create(first, second);
        }

        public override string ToString() => Text;
    }
}

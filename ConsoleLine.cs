using System;
using System.Collections.Generic;
using System.Linq;

namespace CommandLineParsing
{
    internal class ConsoleLine
    {
        private LinkedList<ConsoleSegment> segments;

        public ConsoleLine()
        {
            this.segments = new LinkedList<ConsoleSegment>();
        }

        public int Length => segments.Sum(x => x.Text.Length);
        public bool Empty => segments.Count == 0;

        public void AddSegment(ConsoleSegment segment)
        {
            segments.AddLast(segment);
        }

        public void WriteToConsole()
        {
            ConsoleColor c = Console.ForegroundColor;
            foreach (var s in segments)
            {
                if (s.ForegroundColor.HasValue) Console.ForegroundColor = s.ForegroundColor.Value;
                Console.Write(s.Text);
            }
            if (Length < Console.WindowWidth)
                Console.WriteLine();
            Console.ForegroundColor = c;
        }

        public override string ToString() => string.Join("", segments);
    }
}

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

        public void InsertSegment(ConsoleSegment segment, int cursor)
        {
            if (segments.Count == 0)
            {
                segments.AddLast(segment);
                return;
            }

            int cursorTemp = cursor;

            LinkedListNode<ConsoleSegment> node = segments.First;

            while (true)
            {
                if (node.Value.Text.Length == cursor)
                {
                    segments.AddLast(segment);
                    return;
                }
                else if (node.Value.Text.Length > cursor)
                {
                    if (cursor > 0)
                    {
                        var split = ConsoleSegment.Split(node.Value, cursor);

                        segments.AddBefore(node, split.Item1);
                        var next = segments.AddAfter(node, split.Item2);
                        segments.Remove(node);

                        node = next;
                        cursor = 0;
                    }

                    if (segment.Text.Length < node.Value.Text.Length)
                    {
                        var split = ConsoleSegment.Split(node.Value, segment.Text.Length);
                        node.Value = segment;
                        segments.AddAfter(node, split.Item2);
                    }
                    else
                        node.Value = segment;

                    return;
                }
                else
                {
                    cursor -= node.Value.Text.Length;
                    node = node.Next;
                }
            }
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

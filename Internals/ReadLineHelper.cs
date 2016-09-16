using System;
using System.Text;

namespace CommandLineParsing.Internals
{
    internal class ReadLineHelper
    {
        private readonly int position;
        private readonly StringBuilder sb;

        public ReadLineHelper()
        {
            position = Console.CursorLeft;
            sb = new StringBuilder();
        }

        public int Length => sb.Length;

        public int Index
        {
            get { return Console.CursorLeft - position; }
            set
            {
                if (value > Index)
                {
                    if (value <= sb.Length)
                        Console.CursorLeft = value + position;
                }
                else if (value < Index)
                {
                    if (value >= 0)
                        Console.CursorLeft = value + position;
                }
            }
        }
    }
}

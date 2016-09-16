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

        public string Value => sb.ToString();
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

        public void Insert(string text)
        {
            if (Console.CursorLeft == position + sb.Length)
            {
                Console.Write(text);
                sb.Append(text);
            }
            else
            {
                int temp = Console.CursorLeft;

                sb.Insert(Index, text);
                Console.Write(sb.ToString().Substring(Index));

                Console.CursorLeft = temp + text.Length;
            }
        }
        public void Insert(char info)
        {
            if (Index == Length)
            {
                Console.Write(info);
                sb.Append(info);
            }
            else
            {
                int temp = Console.CursorLeft;

                sb.Insert(Console.CursorLeft - position, info);
                Console.Write(sb.ToString().Substring(Console.CursorLeft - position));

                Console.CursorLeft = temp + 1;
            }
        }
    }
}

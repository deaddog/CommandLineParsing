using System;
using System.Linq;
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

        public void Delete(int length)
        {
            if (length < 0)
            {
                if (Index == 0)
                    return;
                if (Index < -length)
                    length = -Index;

                sb.Remove(Index + length, -length);

                var replace = new string(' ', -length);
                if (Index != Length - length)
                    replace = sb.ToString().Substring(Index + length) + replace;

                int temp = Console.CursorLeft;
                Console.CursorLeft += length;
                Console.Write(replace);
                Console.CursorLeft = temp + length;
            }
            else if (length > 0)
            {
                if (Index == Length)
                    return;
                if (Index + length > Length)
                    length = Length - Index;

                int temp = Console.CursorLeft;
                sb.Remove(Index, length);
                Console.Write(sb.ToString().Substring(Index) + new string(' ', length));
                Console.CursorLeft = temp;
            }
        }

        public int IndexOfPrevious(params char[] chars)
        {
            int index = Index;
            if (index == 0)
                return 0;

            int i = Value.Substring(0, index - 1).LastIndexOf(' ') + 1;
            if (i == index - 1)
            {
                while (i > 0 && chars.Contains(Value[i - 1]))
                    i--;
            }

            return i;
        }
    }
}

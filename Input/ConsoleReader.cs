using System;
using System.Linq;
using System.Text;

namespace CommandLineParsing.Input
{
    internal class ConsoleReader
    {
        public static bool IsInputCharacter(ConsoleKeyInfo info)
        {
            return IsInputCharacter(info.KeyChar);
        }
        public static bool IsInputCharacter(char character)
        {
            return
                char.IsLetterOrDigit(character) ||
                char.IsPunctuation(character) ||
                char.IsSymbol(character) ||
                char.IsSeparator(character);
        }

        private readonly int position;
        private readonly StringBuilder sb;

        public ConsoleReader()
        {
            position = Console.CursorLeft;
            sb = new StringBuilder();
        }

        public string Value => sb.ToString();
        public int Length => sb.Length;

        /// <summary>
        /// Occurs when <see cref="Value"/> changes value.
        /// </summary>
        public event ConsoleReaderTextChanged TextChanged;

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
            var old = Value;

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

            TextChanged?.Invoke(this, old);
        }
        public void Insert(char info)
        {
            var old = Value;

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

            TextChanged?.Invoke(this, old);
        }

        public void Delete(int length)
        {
            var old = Value;

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

            TextChanged?.Invoke(this, old);
        }

        public void ApplyCleanup(ReadLineCleanup cleanup, string prompt = null)
        {
            var promptLength = prompt == null ?
                (int?)null :
                ColorConsole.ClearColors(prompt).Length;

            ApplyCleanup(cleanup, promptLength);
        }
        public void ApplyCleanup(ReadLineCleanup cleanup, int? promptLength = null)
        {
            switch (cleanup)
            {
                case ReadLineCleanup.None:
                    Console.Write(Environment.NewLine);
                    break;

                case ReadLineCleanup.RemovePrompt:
                case ReadLineCleanup.RemoveAll:
                    {
                        var value = Value;

                        Index = 0;
                        Delete(value.Length);

                        if (promptLength.HasValue)
                        {
                            Console.CursorLeft -= promptLength.Value;
                            Console.Write(new string(' ', promptLength.Value));
                            Console.CursorLeft -= promptLength.Value;
                        }

                        if (cleanup == ReadLineCleanup.RemovePrompt)
                            Console.WriteLine(value);
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(cleanup));
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
        public int IndexOfNext(params char[] chars)
        {
            int index = Index;
            if (index == Length)
                return index;

            int i = Value.Substring(index + 1).IndexOf(' ') + index + 1;
            if (i == index)
                i = Length;
            else if (i == index + 1)
            {
                while (i < Length && chars.Contains(Value[i]))
                    i++;
            }

            return i;
        }
    }
}

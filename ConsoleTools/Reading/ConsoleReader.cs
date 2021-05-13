using ConsoleTools.Coloring;
using System;
using System.Linq;
using System.Text;

namespace ConsoleTools.Reading
{
    public class ConsoleReader
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

        private readonly StringBuilder _stringBuilder;
        private Color _color;

        public ConsoleReader(IConsole console)
        {
            Console = console ?? throw new ArgumentNullException(nameof(console));

            Origin = Console.CursorPosition;
            _stringBuilder = new StringBuilder();
            _color = Color.NoColor
                .WithForeground(Console.ForegroundColor.ToString())
                .WithBackground(Console.BackgroundColor.ToString());
        }

        public IConsole Console { get; }

        public string Text
        {
            get { return _stringBuilder.ToString(); }
            set
            {
                int diff = Length - value.Length;
                if (diff > 0)
                {
                    Index = value.Length;
                    Write(value);
                    Console.Write(ConsoleString.FromContent(new string(' ', diff)));
                }

                Index = 0;
                _stringBuilder.Clear();
                Insert(value);
            }
        }
        public int Length => _stringBuilder.Length;

        public Color Color
        {
            get { return _color; }
            set
            {
                if (_color != value)
                {
                    _color = value;

                    var current = Index;
                    Index = 0;
                    Write(_stringBuilder.ToString());
                    Index = current;
                }
            }
        }

        public event ConsoleReaderTextChanged? TextChanged;

        public int Index
        {
            get
            {
                var diff = Console.CursorPosition - Origin;
                return diff.Horizontal + diff.Vertical * Console.BufferSize.Horizontal;
            }
            set
            {
                var position = Origin + (value, 0);

                while (position.Horizontal >= Console.BufferSize.Horizontal)
                    position += (-Console.BufferSize.Horizontal, 1);

                Console.CursorPosition = position;
            }
        }

        public Vector Origin { get; }

        public void Insert(string text)
        {
            var old = Text;

            if (Index == _stringBuilder.Length)
            {
                Write(text);
                _stringBuilder.Append(text);
            }
            else
            {
                int temp = Index;

                _stringBuilder.Insert(Index, text);
                Write(_stringBuilder.ToString().Substring(Index));

                Index = temp + text.Length;
            }

            TextChanged?.Invoke(this, old);
        }
        public void Insert(char info)
        {
            var old = Text;

            if (Index == Length)
            {
                Write(info.ToString());
                _stringBuilder.Append(info);
            }
            else
            {
                int temp = Index;

                _stringBuilder.Insert(Index, info);
                Write(_stringBuilder.ToString().Substring(Index));

                Index = temp + 1;
            }

            TextChanged?.Invoke(this, old);
        }

        public void Delete(int length)
        {
            var old = Text;

            if (length < 0)
            {
                if (Index == 0)
                    return;
                if (Index < -length)
                    length = -Index;

                _stringBuilder.Remove(Index + length, -length);

                var replace = new string(' ', -length);
                if (Index != Length - length)
                    replace = _stringBuilder.ToString().Substring(Index + length) + replace;

                int temp = Index;
                Index += length;
                Write(replace);
                Index = temp + length;
            }
            else if (length > 0)
            {
                if (Index == Length)
                    return;
                if (Index + length > Length)
                    length = Length - Index;

                int temp = Index;
                _stringBuilder.Remove(Index, length);
                Write(_stringBuilder.ToString().Substring(Index) + new string(' ', length));
                Index = temp;
            }

            TextChanged?.Invoke(this, old);
        }

        private int IndexOfPrevious(params char[] chars)
        {
            int index = Index;
            if (index == 0)
                return 0;

            int i = Text.Substring(0, index - 1).LastIndexOf(' ') + 1;
            if (i == index - 1)
            {
                while (i > 0 && chars.Contains(Text[i - 1]))
                    i--;
            }

            return i;
        }
        private int IndexOfNext(params char[] chars)
        {
            int index = Index;
            if (index == Length)
                return index;

            int i = Text.Substring(index + 1).IndexOf(' ') + index + 1;
            if (i == index)
                i = Length;
            else if (i == index + 1)
            {
                while (i < Length && chars.Contains(Text[i]))
                    i++;
            }

            return i;
        }

        private void Write(string content)
        {
            Console.Write(ConsoleString.FromContent(content, Color));
        }

        public void HandleKey(ConsoleKeyInfo key)
        {
            switch (key.Key)
            {
                case ConsoleKey.Backspace:
                    if (key.Modifiers == ConsoleModifiers.Control)
                        Delete(IndexOfPrevious(' ') - Index);
                    else
                        Delete(-1);
                    break;
                case ConsoleKey.Delete:
                    if (key.Modifiers == ConsoleModifiers.Control)
                        Delete(IndexOfNext(' ') - Index);
                    else
                        Delete(1);
                    break;

                case ConsoleKey.LeftArrow:
                    if (key.Modifiers == ConsoleModifiers.Control)
                        Index = IndexOfPrevious(' ');
                    else
                        Index--;
                    break;
                case ConsoleKey.RightArrow:
                    if (key.Modifiers == ConsoleModifiers.Control)
                        Index = IndexOfNext(' ');
                    else
                        Index++;
                    break;
                case ConsoleKey.Home:
                    Index = 0;
                    break;
                case ConsoleKey.End:
                    Index = Length;
                    break;

                default:
                    if (IsInputCharacter(key))
                        Insert(key.KeyChar);
                    break;
            }
        }
    }
}

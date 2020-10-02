using CommandLineParsing.Output;
using System;
using System.Linq;
using System.Text;

namespace CommandLineParsing.Input.Reading
{
    /// <summary>
    /// Provides methods for reading input from the console.
    /// </summary>
    public class ConsoleReader : IConsoleInput
    {
        /// <summary>
        /// Determines whether a <see cref="ConsoleKeyInfo"/> is a printable character, that can be used as raw text input.
        /// </summary>
        /// <param name="info">The key to check.</param>
        /// <returns>
        ///   <c>true</c> if the key is a valid input character; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsInputCharacter(ConsoleKeyInfo info)
        {
            return IsInputCharacter(info.KeyChar);
        }
        /// <summary>
        /// Determines whether a <see cref="char"/> is a printable character, that can be used as raw text input.
        /// </summary>
        /// <param name="character">The character to check.</param>
        /// <returns>
        ///   <c>true</c> if the character is a valid input character; otherwise, <c>false</c>.
        /// </returns>
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

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleReader"/> class.
        /// </summary>
        /// <param name="console">The console used to read input.</param>
        public ConsoleReader(IConsole console)
        {
            Console = console ?? throw new ArgumentNullException(nameof(console));

            Origin = Console.GetCursorPosition();
            _stringBuilder = new StringBuilder();
            _color = Color.NoColor
                .WithForeground(Console.ForegroundColor.ToString())
                .WithBackground(Console.BackgroundColor.ToString());
        }

        public IConsole Console { get; }

        /// <summary>
        /// Gets or sets the current text displayed by this <see cref="ConsoleReader"/>.
        /// </summary>
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
        /// <summary>
        /// Gets the length of the text displayed by this <see cref="ConsoleReader"/>.
        /// </summary>
        public int Length => _stringBuilder.Length;

        /// <summary>
        /// Gets or sets the color used for the users text.
        /// </summary>
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

        /// <summary>
        /// Occurs when <see cref="Text"/> changes value.
        /// </summary>
        public event ConsoleReaderTextChanged TextChanged;

        /// <summary>
        /// Gets or sets the cursors index in the input string.
        /// Index 0 (zero) places the cursor in front of the first character.
        /// </summary>
        public int Index
        {
            get { return Console.CursorLeft - Origin.Left + (Console.CursorTop - Origin.Top) * Console.BufferWidth; }
            set
            {
                var position = new ConsolePoint(Origin.Left + value, Origin.Top);

                while (position.Left >= Console.BufferWidth)
                    position += new ConsoleSize(-Console.BufferWidth, 1);

                Console.SetCursorPosition(position);
            }
        }

        /// <summary>
        /// Gets the location where the readline is displayed. If a prompt was passed to the constructer, this points to the start of that prompt.
        /// </summary>
        public ConsolePoint Origin { get; }

        /// <summary>
        /// Inserts the specified text at the cursors current position (<see cref="Index"/>).
        /// </summary>
        /// <param name="text">The text to insert.</param>
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
        /// <summary>
        /// Inserts the specified character at the cursors current position (<see cref="Index"/>).
        /// </summary>
        /// <param name="info">The character to insert.</param>
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

        /// <summary>
        /// Deletes the specified number of characters from the input text.
        /// </summary>
        /// <param name="length">
        /// The number of characters to delete from the cursors current position (<see cref="Index"/>).
        /// A positive value will remove characters to the right of the cursor.
        /// A negative value will remove characters to the left of the cursor.
        /// </param>
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

        /// <summary>
        /// Handles the specified key by updating the <see cref="Text"/> property.
        /// </summary>
        /// <param name="key">The key to process.</param>
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

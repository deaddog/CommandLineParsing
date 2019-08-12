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

        private readonly IConsole _console;
        private readonly ConsoleString prompt;
        private readonly ConsolePoint origin;
        private readonly int position;
        private readonly StringBuilder sb;
        private Color _color;

        private bool isDisposed = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleReader"/> class.
        /// </summary>
        /// <param name="console">The console used to read input.</param>
        /// <param name="prompt">A prompt message to display to the user before input. <c>null</c> indicates that no prompt message should be displayed.</param>
        public ConsoleReader(IConsole console, ConsoleString prompt = null)
        {
            _console = console ?? throw new ArgumentNullException(nameof(console));
            origin = _console.GetCursorPosition();

            this.prompt = prompt;
            if (prompt != null)
                console.Write(prompt);

            position = _console.CursorLeft;
            sb = new StringBuilder();
            _color = Color.NoColor
                .WithForeground(_console.ForegroundColor.ToString())
                .WithBackground(_console.BackgroundColor.ToString());
        }

        /// <summary>
        /// Gets the current text displayed by this <see cref="ConsoleReader"/>.
        /// </summary>
        public string Text => sb.ToString();
        /// <summary>
        /// Gets the length of the text displayed by this <see cref="ConsoleReader"/>.
        /// </summary>
        public int Length => sb.Length;

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

                    var current = _console.CursorLeft;
                    _console.CursorLeft = position;
                    Write(sb.ToString());
                    _console.CursorLeft = current;
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
            get { return _console.CursorLeft - position; }
            set
            {
                if (value > Index)
                {
                    if (value <= sb.Length)
                        _console.CursorLeft = value + position;
                }
                else if (value < Index)
                {
                    if (value >= 0)
                        _console.CursorLeft = value + position;
                }
            }
        }

        /// <summary>
        /// Gets the location where the readline is displayed. If a prompt was passed to the constructer, this points to the start of that prompt.
        /// </summary>
        public ConsolePoint Origin => origin;

        /// <summary>
        /// Gets or sets the type of cleanup that should be applied when disposing the <see cref="ConsoleReader" />.
        /// </summary>
        public InputCleanup Cleanup { get; set; }

        /// <summary>
        /// Inserts the specified text at the cursors current position (<see cref="Index"/>).
        /// </summary>
        /// <param name="text">The text to insert.</param>
        public void Insert(string text)
        {
            var old = Text;

            if (_console.CursorLeft == position + sb.Length)
            {
                Write(text);
                sb.Append(text);
            }
            else
            {
                int temp = _console.CursorLeft;

                sb.Insert(Index, text);
                Write(sb.ToString().Substring(Index));

                _console.CursorLeft = temp + text.Length;
            }

            if (!isDisposed)
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
                sb.Append(info);
            }
            else
            {
                int temp = _console.CursorLeft;

                sb.Insert(_console.CursorLeft - position, info);
                Write(sb.ToString().Substring(_console.CursorLeft - position));

                _console.CursorLeft = temp + 1;
            }

            if (!isDisposed)
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

                sb.Remove(Index + length, -length);

                var replace = new string(' ', -length);
                if (Index != Length - length)
                    replace = sb.ToString().Substring(Index + length) + replace;

                int temp = _console.CursorLeft;
                _console.CursorLeft += length;
                Write(replace);
                _console.CursorLeft = temp + length;
            }
            else if (length > 0)
            {
                if (Index == Length)
                    return;
                if (Index + length > Length)
                    length = Length - Index;

                int temp = _console.CursorLeft;
                sb.Remove(Index, length);
                Write(sb.ToString().Substring(Index) + new string(' ', length));
                _console.CursorLeft = temp;
            }

            if (!isDisposed)
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
            _console.Write(ConsoleString.FromContent(content, Color));
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

        /// <summary>
        /// Performs cleanup of the reader as specified by <see cref="Cleanup"/>.
        /// </summary>
        public void Dispose()
        {
            isDisposed = true;
            int? promptLength = prompt?.Length;

            switch (Cleanup)
            {
                case InputCleanup.None:
                    _console.Render(Environment.NewLine);
                    break;

                case InputCleanup.Clean:
                    {
                        var value = Text;

                        Index = 0;
                        Delete(value.Length);

                        if (promptLength.HasValue)
                        {
                            _console.CursorLeft -= promptLength.Value;
                            _console.Render(new string(' ', promptLength.Value));
                            _console.CursorLeft -= promptLength.Value;
                        }
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(Cleanup));
            }
        }
    }
}

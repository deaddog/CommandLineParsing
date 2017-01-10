using System;
using System.Linq;
using System.Text;

namespace CommandLineParsing.Input
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

        private readonly int position;
        private readonly StringBuilder sb;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleReader"/> class.
        /// </summary>
        public ConsoleReader()
        {
            position = Console.CursorLeft;
            sb = new StringBuilder();
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
        /// Occurs when <see cref="Text"/> changes value.
        /// </summary>
        public event ConsoleReaderTextChanged TextChanged;

        /// <summary>
        /// Gets or sets the cursors index in the input string.
        /// Index 0 (zero) places the cursor in front of the first character.
        /// </summary>
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

        /// <summary>
        /// Inserts the specified text at the cursors current position (<see cref="Index"/>).
        /// </summary>
        /// <param name="text">The text to insert.</param>
        public void Insert(string text)
        {
            var old = Text;

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
        /// <summary>
        /// Inserts the specified character at the cursors current position (<see cref="Index"/>).
        /// </summary>
        /// <param name="info">The character to insert.</param>
        public void Insert(char info)
        {
            var old = Text;

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
                        var value = Text;

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

            int i = Text.Substring(0, index - 1).LastIndexOf(' ') + 1;
            if (i == index - 1)
            {
                while (i > 0 && chars.Contains(Text[i - 1]))
                    i--;
            }

            return i;
        }
        public int IndexOfNext(params char[] chars)
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
                    if (ConsoleReader.IsInputCharacter(key))
                        Insert(key.KeyChar);
                    break;
            }
        }
    }
}

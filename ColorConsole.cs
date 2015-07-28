using System;
using System.Text;
using System.Text.RegularExpressions;

namespace CommandLineParsing
{
    /// <summary>
    /// Provides methods for outputting color-coded text to the console using a simple format.
    /// The string "[Color:Text]" will print Text to the console using Color as the foreground color.
    /// </summary>
    public static class ColorConsole
    {
        private static readonly Regex colorRegex;

        static ColorConsole()
        {
            colorRegex = new Regex(@"(?<!\\)\[(?<color>[^:]+):(?<content>([^\\\]\[]|\\\]|\\\[|\\\\)*)\]");
        }

        /// <summary>
        /// Writes the specified string value to the standard output stream.
        /// </summary>
        /// <param name="format">The string format to write, included color information.
        /// The string "[Color:Text]" will print Text to the console using Color as the foreground color.</param>
        /// <param name="args">An array of arguments used for the <paramref name="format"/> string.</param>
        public static void Write(string format, params object[] args)
        {
            handle(args.Length == 0 ? format : string.Format(format, args), true, false);
        }
        /// <summary>
        /// Writes the specified string value, followed by the current line terminator, to the standard output stream.
        /// </summary>
        /// <param name="format">The string format to write, included color information.
        /// The string "[Color:Text]" will print Text to the console using Color as the foreground color.</param>
        /// <param name="args">An array of arguments used for the <paramref name="format"/> string.</param>
        public static void WriteLine(string format, params object[] args)
        {
            handle(args.Length == 0 ? format : string.Format(format, args), true, true);
        }

        /// <summary>
        /// Writes the specified string value to the standard output stream.
        /// </summary>
        /// <param name="format">The string format to write. Any color information is discarded.</param>
        /// <param name="args">An array of arguments used for the <paramref name="format"/> string.</param>
        public static void WriteNoColor(string format, params object[] args)
        {
            Console.Write(ClearColors(args.Length == 0 ? format : string.Format(format, args)));
        }
        /// <summary>
        /// Writes the specified string value, followed by the current line terminator, to the standard output stream.
        /// </summary>
        /// <param name="format">The string format to write. Any color information is discarded.</param>
        /// <param name="args">An array of arguments used for the <paramref name="format"/> string.</param>
        public static void WriteLineNoColor(string format, params object[] args)
        {
            Console.WriteLine(ClearColors(args.Length == 0 ? format : string.Format(format, args)));
        }

        /// <summary>
        /// Removes color-coding information from a string.
        /// The string "[Color:Text]" will print Text to the console using the default color as the foreground color.
        /// </summary>
        /// <param name="input">The string from which color information should be removed.</param>
        /// <returns>A new string, without any color information.</returns>
        public static string ClearColors(string input)
        {
            StringBuilder sb = new StringBuilder();

            Match m;
            while ((m = colorRegex.Match(input)).Success)
            {
                sb.Append(input.Substring(0, m.Index));
                sb.Append(m.Groups["content"].Value);
                input = input.Substring(m.Index + m.Length);
            }
            sb.Append(input);

            return sb.ToString();
        }

        /// <summary>
        /// Determines whether the specified string includes coloring syntax.
        /// </summary>
        /// <param name="input">The string that should be examined.</param>
        /// <returns><c>true</c>, if <paramref name="input"/> contains any "[Color:Text]" strings; otherwise, <c>false</c>.</returns>
        public static bool HasColors(string input)
        {
            return colorRegex.IsMatch(input);
        }

        private static void handle(string input, bool allowcolor, bool newline)
        {
            var m = colorRegex.Match(input);
            if (m.Success)
            {
                string pre = replaceEscaped(input.Substring(0, m.Index));
                string post = input.Substring(m.Index + m.Length);

                string content = replaceEscaped(m.Groups["content"].Value).Replace("\\\\", "\\");
                if (!allowcolor)
                    Console.Write(pre + content);
                else
                {
                    Console.Write(pre);
                    if (content.Length > 0)
                    {
                        var c = getColor(m.Groups["color"].Value);
                        if (c.HasValue)
                            Console.ForegroundColor = c.Value;
                        Console.Write(content);
                        Console.ResetColor();
                    }
                }

                handle(post, allowcolor, newline);
            }
            else if (newline)
                Console.WriteLine(replaceEscaped(input));
            else
                Console.Write(replaceEscaped(input));
        }

        private static string replaceEscaped(string input)
        {
            return input.Replace("\\[", "[").Replace("\\]", "]");

        }

        private static ConsoleColor? getColor(string color)
        {
            ConsoleColor c;
            if (!Enum.TryParse(color, out c))
                return null;
            else
                return c;
        }

        /// <summary>
        /// Writes <paramref name="prompt"/> to <see cref="Console"/>, reads user input and returns a parsed value.
        /// </summary>
        /// <typeparam name="T">The type of input that the method should accept.</typeparam>
        /// <param name="prompt">A prompt message to display to the user before input. <c>null</c> indicates that no prompt message should be displayed.</param>
        /// <param name="defaultString">A <see cref="string"/> that the inputtext is initialized to.
        /// The <see cref="string"/> can be edited in the <see cref="Console"/> and is part of the parsed <see cref="string"/> if not modified.
        /// <c>null</c> indicates that no initial value should be used.</param>
        /// <param name="validator">The <see cref="Validator{T}"/> object that should be used to validate a parsed value.
        /// <c>null</c> indicates that no validation should be applied.</param>
        /// <returns>A <typeparamref name="T"/> element parsed from user input, that meets the requirements of <paramref name="validator"/>.</returns>
        public static T ReadLine<T>(string prompt = null, string defaultString = null, Validator<T> validator = null)
        {
            if (prompt != null)
                ColorConsole.Write(prompt);

            var tryparse = ParserLookup.Table.GetParser<T>(false);

            int l = System.Console.CursorLeft, t = System.Console.CursorTop;
            string input = "";
            T result = default(T);
            bool parsed = false;

            while (!parsed)
            {
                System.Console.SetCursorPosition(l, t);
                System.Console.Write("".PadRight(input.Length, ' '));
                System.Console.SetCursorPosition(l, t);

                input = ColorConsole.ReadLine(defaultString);
                parsed = tryparse(input, out result);

                Message msg = Message.NoError;

                if (parsed)
                    msg = validator == null ? Message.NoError : validator.Validate(result);
                else
                    msg = $"{input} is not a {typeof(T).Name} value.";

                if (msg.IsError)
                {
                    parsed = false;
                    System.Console.SetCursorPosition(l, t);
                    System.Console.Write("".PadRight(input.Length, ' '));

                    input = msg.GetMessage();
                    System.Console.ForegroundColor = ConsoleColor.Red;
                    System.Console.SetCursorPosition(l, t);
                    System.Console.Write(input);
                    System.Console.ResetColor();

                    System.Console.ReadKey(true);
                }
            }

            return result;
        }

        /// <summary>
        /// Reads a <see cref="string"/> from <see cref="Console"/>.
        /// </summary>
        /// <param name="prompt">A prompt message to display to the user before input. <c>null</c> indicates that no prompt message should be displayed.</param>
        /// <param name="defaultString">A <see cref="string"/> that the inputtext is initialized to.
        /// The <see cref="string"/> can be edited in the <see cref="Console"/> and is part of the returned <see cref="string"/> if not modified.
        /// <c>null</c> indicates that no initial value should be used.</param>
        /// <returns>A <see cref="string"/> containing the user input.</returns>
        public static string ReadLine(string prompt = null, string defaultString = null)
        {
            if (prompt != null)
                ColorConsole.Write(prompt);

            int pos = Console.CursorLeft;
            Console.Write(defaultString);
            ConsoleKeyInfo info;

            StringBuilder sb = new StringBuilder(defaultString);

            while (true)
            {
                info = Console.ReadKey(true);
                if (info.Key == ConsoleKey.Backspace)
                {
                    if (Console.CursorLeft <= pos) continue;
                    sb.Remove(Console.CursorLeft - pos - 1, 1);
                    if (Console.CursorLeft == pos + sb.Length + 1)
                    {
                        Console.CursorLeft -= 1;
                        Console.Write(' ');
                        Console.CursorLeft -= 1;
                    }
                    else
                    {
                        int temp = Console.CursorLeft;
                        Console.CursorLeft -= 1;
                        var cover = sb.ToString().Substring(Console.CursorLeft - pos) + " ";
                        Console.Write(sb.ToString().Substring(Console.CursorLeft - pos) + " ");
                        Console.CursorLeft = temp - 1;
                    }

                }
                else if (info.Key == ConsoleKey.Delete)
                {
                    if (Console.CursorLeft == pos + sb.Length) continue;

                    int temp = Console.CursorLeft;
                    sb.Remove(Console.CursorLeft - pos, 1);
                    Console.Write(sb.ToString().Substring(Console.CursorLeft - pos) + " ");
                    Console.CursorLeft = temp;
                }

                else if (info.Key == ConsoleKey.Enter) { Console.Write(Environment.NewLine); break; }
                else if (info.Key == ConsoleKey.LeftArrow) { if (Console.CursorLeft > pos) Console.CursorLeft--; }
                else if (info.Key == ConsoleKey.RightArrow) { if (Console.CursorLeft < pos + sb.Length) Console.CursorLeft++; }
                else if (info.Key == ConsoleKey.Home) Console.CursorLeft = pos;
                else if (info.Key == ConsoleKey.End) Console.CursorLeft = pos + sb.Length;

                else if (isConsoleChar(info))
                {
                    if (Console.CursorLeft == pos + sb.Length)
                    {
                        Console.Write(info.KeyChar);
                        sb.Append(info.KeyChar);
                    }
                    else
                    {
                        int temp = Console.CursorLeft;
                        sb.Insert(Console.CursorLeft - pos, info.KeyChar);
                        Console.Write(sb.ToString().Substring(Console.CursorLeft - pos));
                        Console.CursorLeft = temp + 1;
                    }
                }
            }
            return sb.ToString();
        }

        private static bool isConsoleChar(ConsoleKeyInfo info)
        {
            return char.IsLetterOrDigit(info.KeyChar) || char.IsPunctuation(info.KeyChar) || char.IsSymbol(info.KeyChar) || char.IsSeparator(info.KeyChar);
        }
    }
}

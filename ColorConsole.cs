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
            colorRegex = new Regex(@"(?<!\\)\[(?<color>[^:]+):(?<content>([^\\\]]|\\\])*)\]");
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

        private static void handle(string input, bool allowcolor, bool newline)
        {
            var m = colorRegex.Match(input);
            if (m.Success)
            {
                string pre = input.Substring(0, m.Index).Replace("\\[", "[").Replace("\\]", "]");
                string post = input.Substring(m.Index + m.Length);

                string content = m.Groups["content"].Value.Replace("\\[", "[").Replace("\\]", "]");
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
                Console.WriteLine(input);
            else
                Console.Write(input);
        }

        private static ConsoleColor? getColor(string color)
        {
            ConsoleColor c;
            if (!Enum.TryParse(color, out c))
                return null;
            else
                return c;
        }

        public static T Read<T>(string prompt, Func<T, bool> predicate)
        {
            if (prompt == null)
                throw new ArgumentNullException("text");

            var tryparse = ParserLookup.Table.GetParser<T>(false);

            System.Console.Write(prompt);

            int l = System.Console.CursorLeft, t = System.Console.CursorTop;
            string input = "";
            T result = default(T);
            bool parsed = false;

            while (!parsed)
            {
                System.Console.SetCursorPosition(l, t);
                System.Console.Write("".PadRight(input.Length, ' '));
                System.Console.SetCursorPosition(l, t);
                input = System.Console.ReadLine();
                parsed = tryparse(input, out result);
                if (parsed)
                    parsed = predicate(result);
            }

            return result;
        }
    }
}

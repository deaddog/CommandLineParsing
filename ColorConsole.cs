using System;
using System.Collections.Generic;
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
        private static readonly ColorTable colors;

        static ColorConsole()
        {
            colors = new ColorTable();
        }

        /// <summary>
        /// Gets the <see cref="ColorTable"/> that handles the available colornames for the <see cref="ColorConsole"/>.
        /// </summary>
        public static ColorTable Colors
        {
            get { return colors; }
        }

        /// <summary>
        /// Writes the specified string value to the standard output stream.
        /// </summary>
        /// <param name="value">The string format to write, included color information.
        /// The string "[Color:Text]" will print Text to the console using Color as the foreground color.</param>
        /// <param name="allowcolor">if set to <c>false</c> any color information passed in <paramref name="value"/> is disregarded.</param>
        public static void Write(string value, bool allowcolor = true)
        {
            value = value ?? string.Empty;

            if (!allowcolor)
            {
                Console.Write(ClearColors(value).Replace("\\\\", "\\"));
                return;
            }
            
            int index = 0;

            while (index < value.Length)
                switch (value[index])
                {
                    case '[': // Coloring
                        {
                            int end = findEnd(value, index, '[', ']');
                            var block = value.Substring(index + 1, end - index - 1);
                            int colon = block.IndexOf(':');
                            if (colon != -1 && block[colon - 1] == '\\')
                                colon = -1;

                            if (colon == -1)
                                Console.Write("[" + block + "]");
                            else
                            {
                                var color = colors[block.Substring(0, colon)];
                                string content = block.Substring(colon + 1);

                                if (color.HasValue && content.Trim().Length > 0)
                                {
                                    ConsoleColor temp = Console.ForegroundColor;
                                    Console.ForegroundColor = color.Value;
                                    Write(content);
                                    Console.ForegroundColor = temp;
                                }
                                else
                                    Write(content);
                            }
                            index += block.Length + 2;
                        }
                        break;

                    case '\\':
                        if (value.Length == index + 1)
                            index++;
                        else
                        {
                            Console.Write(value[index + 1]);
                            index += 2;
                        }
                        break;

                    default: // Skip content
                        int nIndex = value.IndexOfAny(new char[] { '[', '\\' }, index);
                        if (nIndex < 0) nIndex = value.Length;
                        Console.Write(value.Substring(index, nIndex - index));
                        index = nIndex;
                        break;
                }
        }
        /// <summary>
        /// Writes the specified string value, followed by the current line terminator, to the standard output stream.
        /// </summary>
        /// <param name="value">The string format to write, included color information.
        /// The string "[Color:Text]" will print Text to the console using Color as the foreground color.</param>
        /// <param name="allowcolor">if set to <c>false</c> any color information passed in <paramref name="value"/> is disregarded.</param>
        public static void WriteLine(string value, bool allowcolor = true)
        {
            Write(value, allowcolor);
            Console.WriteLine();
        }

        /// <summary>
        /// Evaluates <paramref name="format"/> given the current state of the <see cref="FormattedPrinter"/>, by applying the format translation.
        /// </summary>
        /// <param name="format">The text that should be evaluated.</param>
        /// <param name="formatter">The <see cref="IFormatter"/> that should be used to define the available elements in the format.</param>
        /// <returns>The result of the evaluation.</returns>
        public static string EvaluateFormat(string format, IFormatter formatter)
        {
            int index = 0;

            while (index < format.Length)
                switch (format[index])
                {
                    case '[': // Coloring
                        {
                            int end = findEnd(format, index, '[', ']');
                            var block = format.Substring(index + 1, end - index - 1);
                            string replace = colorBlock(block, formatter);
                            format = format.Substring(0, index) + replace + format.Substring(end + 1);
                            index += replace.Length;
                        }
                        break;

                    case '?': // Conditional
                        {
                            var match = Regex.Match(format.Substring(index), @"\?[^\{]*");
                            var end = findEnd(format, index + match.Value.Length, '{', '}');
                            var block = format.Substring(index + match.Value.Length + 1, end - index - match.Value.Length - 1);

                            string replace = "";
                            var condition = formatter.ValidateCondition(match.Value.Substring(1));
                            if (!condition.HasValue)
                                replace = "?" + match.Value + "{" + EvaluateFormat(block, formatter) + "}";
                            else if (condition.Value)
                                replace = EvaluateFormat(block, formatter);

                            format = format.Substring(0, index) + replace + format.Substring(end + 1);
                            index += replace.Length;
                        }
                        break;

                    case '@': // Listing/Function
                        {
                            var match = Regex.Match(format.Substring(index), @"\@[^\{]*");
                            var end = findEnd(format, index + match.Value.Length, '{', '}');

                            var args = format.Substring(index + match.Value.Length + 1, end - index - match.Value.Length - 1);
                            string name = match.Value.Substring(1);

                            string replace = formatter.EvaluateFunction(name, args.Split('@'));
                            if (replace == null)
                                replace = $"@{name}{{{args}}}";

                            format = format.Substring(0, index) + replace + format.Substring(end + 1);
                            index += replace.Length;
                        }
                        break;

                    case '$': // Variable
                        {
                            var match = Regex.Match(format.Substring(index), @"^\$([a-z]|\+)+");
                            var end = match.Index + index + match.Length;
                            string replace = getVariable(match.Value.Substring(1), formatter);
                            format = format.Substring(0, index) + replace + format.Substring(end);
                            index += replace.Length;
                        }
                        break;
                    case '\\':
                        if (format.Length == index + 1)
                            index++;
                        else if (format[index + 1] == '[' || format[index + 1] == ']')
                            index += 2;
                        else
                        {
                            format = format.Substring(0, index) + format.Substring(index + 1);
                            index++;
                        }
                        break;

                    default: // Skip content
                        index = format.IndexOfAny(new char[] { '[', '?', '@', '$', '\\' }, index);
                        if (index < 0) index = format.Length;
                        break;
                }

            return format;
        }

        private static string getVariable(string variable, IFormatter formatter)
        {
            bool padLeft = variable[0] == '+';
            bool padRight = variable[variable.Length - 1] == '+';

            if (padLeft) variable = variable.Substring(1);
            if (padRight) variable = variable.Substring(0, variable.Length - 1);

            string res = formatter.GetVariable(variable);
            if (res == null)
                return "$" + (padLeft ? "+" : "") + variable + (padRight ? "+" : "");

            if (padLeft || padRight)
            {
                int? size = formatter.GetAlignedLength(variable);
                if (size.HasValue)
                {
                    if (padLeft && padRight)
                        res = res.PadLeft(size.Value / 2).PadRight(size.Value - (size.Value / 2));
                    else if (padLeft)
                        res = res.PadLeft(size.Value);
                    else
                        res = res.PadRight(size.Value);
                }
            }

            return res;
        }
        private static string colorBlock(string format, IFormatter formatter)
        {
            Match m = Regex.Match(format, "^(?<color>[^:]+):(?<content>.*)$", RegexOptions.Singleline);
            if (!m.Success)
                return string.Empty;

            string color_str = m.Groups["color"].Value;
            string content = m.Groups["content"].Value;

            if (color_str.ToLower() == "auto")
            {
                Match autoColor = Regex.Match(content, @"\$([a-z]|\+)+");

                if (autoColor.Success)
                {
                    string variable = autoColor.Value.Substring(1);
                    if (variable[0] == '+') variable = variable.Substring(1);
                    if (variable[variable.Length - 1] == '+') variable = variable.Substring(0, variable.Length - 1);

                    color_str = formatter.GetAutoColor(variable) ?? string.Empty;
                }
                else
                    color_str = string.Empty;
            }

            color_str = color_str.Trim();
            if (color_str.Length == 0)
                return EvaluateFormat(content, formatter);
            else
                return $"[{color_str}:{EvaluateFormat(content, formatter)}]";
        }

        private static int findEnd(string text, int index, char open, char close)
        {
            int count = 0;
            do
            {
                if (text[index] == '\\') { index += 2; continue; }
                if (text[index] == open) count++;
                else if (text[index] == close) count--;
                index++;
            } while (count > 0 && index < text.Length);
            if (count == 0) index--;

            return index;
        }

        /// <summary>
        /// Removes color-coding information from a string.
        /// The string "[Color:Text]" will print Text to the console using the default color as the foreground color.
        /// </summary>
        /// <param name="input">The string from which color information should be removed.</param>
        /// <returns>A new string, without any color information.</returns>
        public static string ClearColors(string input)
        {
            int index = 0;

            while (index < input.Length)
                switch (input[index])
                {
                    case '[': // Coloring
                        {
                            int end = findEnd(input, index, '[', ']');
                            var block = input.Substring(index + 1, end - index - 1);
                            int colon = block.IndexOf(':');
                            if (colon != -1 && block[colon - 1] == '\\')
                                colon = -1;

                            if (colon == -1)
                                return input;
                            else
                            {
                                string content = block.Substring(colon + 1);
                                input = input.Substring(0, index) + content + input.Substring(end + 1);
                                index += content.Length;
                            }
                        }
                        break;

                    case '\\':
                        if (input.Length == index + 1)
                            input = input.Substring(0, input.Length - 1);
                        else
                        {
                            input = input.Substring(0, index) + input.Substring(index + 1);
                            index++;
                        }
                        break;

                    default: // Skip content
                        int nIndex = input.IndexOfAny(new char[] { '[', '\\' }, index);
                        if (nIndex < 0) nIndex = input.Length;
                        index = nIndex;
                        break;
                }

            return input;
        }
        /// <summary>
        /// Escapes color-coding information in a string such that it can be printed using the <see cref="ColorConsole"/> without color being applied.
        /// </summary>
        /// <param name="input">The string in which color-coding should be escaped.</param>
        /// <returns>A new string, where all color-coding is escaped.</returns>
        public static string EscapeColor(string input)
        {
            return input?.Replace("[", "\\[")?.Replace("]", "\\]");
        }
        /// <summary>
        /// Determines whether the specified string includes coloring syntax.
        /// </summary>
        /// <param name="input">The string that should be examined.</param>
        /// <returns><c>true</c>, if <paramref name="input"/> contains any "[Color:Text]" strings; otherwise, <c>false</c>.</returns>
        public static bool HasColors(string input)
        {
            int index = 0;

            while (index < input.Length)
                switch (input[index])
                {
                    case '[': // Coloring
                        {
                            int end = findEnd(input, index, '[', ']');
                            var block = input.Substring(index + 1, end - index - 1);
                            int colon = block.IndexOf(':');
                            if (colon != -1 && block[colon - 1] == '\\')
                                colon = -1;

                            if (colon != -1)
                                return true;

                            index += block.Length + 2;
                        }
                        break;

                    case '\\':
                        if (input.Length == index + 1)
                            return false;
                        else
                            index += 2;
                        break;

                    default: // Skip content
                        int nIndex = input.IndexOfAny(new char[] { '[', '\\' }, index);
                        if (nIndex < 0) nIndex = input.Length;
                        index = nIndex;
                        break;
                }

            return false;
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

            int l = Console.CursorLeft, t = Console.CursorTop;
            string input = "";
            T result = default(T);
            bool parsed = false;

            while (!parsed)
            {
                Console.SetCursorPosition(l, t);
                Console.Write(new string(' ', input.Length));
                Console.SetCursorPosition(l, t);

                input = ColorConsole.ReadLine(null, defaultString);
                parsed = tryparse(input, out result);

                Message msg = Message.NoError;

                if (parsed)
                    msg = validator == null ? Message.NoError : validator.Validate(result);
                else
                    msg = $"{input} is not a {typeof(T).Name} value.";

                if (msg.IsError)
                {
                    Console.CursorVisible = false;
                    parsed = false;
                    Console.SetCursorPosition(l, t);
                    Console.Write(new string(' ', input.Length));

                    input = msg.GetMessage();
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.SetCursorPosition(l, t);
                    Console.Write(input);
                    Console.ResetColor();

                    Console.ReadKey(true);
                    Console.CursorVisible = true;
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
        /// <summary>
        /// Reads a password from <see cref="Console"/> without printing the input characters.
        /// </summary>
        /// <param name="prompt">A prompt message to display to the user before input. <c>null</c> indicates that no prompt message should be displayed.</param>
        /// <param name="passChar">An optional character to display in place of input symbols. <c>null</c> will display nothing to the user.</param>
        /// <param name="singleSymbol">if set to <c>true</c> <paramref name="passChar"/> will only be printed once, on input.</param>
        /// <returns>A <see cref="string"/> containing the password.</returns>
        public static string ReadPassword(string prompt = null, char? passChar = '*', bool singleSymbol = true)
        {
            if (prompt != null)
                ColorConsole.Write(prompt);

            int pos = Console.CursorLeft;
            ConsoleKeyInfo info;

            StringBuilder sb = new StringBuilder(string.Empty);

            while (true)
            {
                info = Console.ReadKey(true);
                if (info.Key == ConsoleKey.Backspace)
                {
                    Console.CursorLeft = pos;
                    Console.Write(new string(' ', (singleSymbol || !passChar.HasValue) ? 1 : sb.Length));
                    Console.CursorLeft = pos;
                    sb.Clear();
                }

                else if (info.Key == ConsoleKey.Enter) { Console.Write(Environment.NewLine); break; }

                else if (isConsoleChar(info))
                {
                    sb.Append(info.KeyChar);
                    if (passChar.HasValue)
                    {
                        if (!singleSymbol || sb.Length == 1)
                            Console.Write(passChar.Value);
                    }
                }
            }
            return sb.ToString();
        }

        private static bool isConsoleChar(ConsoleKeyInfo info)
        {
            return char.IsLetterOrDigit(info.KeyChar) || char.IsPunctuation(info.KeyChar) || char.IsSymbol(info.KeyChar) || char.IsSeparator(info.KeyChar);
        }

        /// <summary>
        /// Provides a collection of <see cref="string"/>-><see cref="ConsoleColor"/> relations.
        /// </summary>
        public class ColorTable
        {
            private Dictionary<string, ConsoleColor> colors;

            internal ColorTable()
            {
                colors = new Dictionary<string, ConsoleColor>();

                foreach (var c in Enum.GetValues(typeof(ConsoleColor)))
                    colors.Add(c.ToString().ToLowerInvariant(), (ConsoleColor)c);
            }

            /// <summary>
            /// Gets or sets the <see cref="System.Nullable{ConsoleColor}"/> with the specified name.
            /// A value of <c>null</c> (in both getter and setter) is equivalent of no color.
            /// </summary>
            /// <param name="name">The name associated with the <see cref="ConsoleColor"/>.
            /// This name does not have to pre-exist in the <see cref="ConsoleColor"/> enum.
            /// The name is case insensitive, meaning that "Red" and "red" will refer to the same color, if any.</param>
            /// <returns>The <see cref="ConsoleColor"/> associated with <paramref name="name"/> or <c>null</c>, if no color is associated with <paramref name="name"/>.</returns>
            public ConsoleColor? this[string name]
            {
                get
                {
                    if (name == null)
                        throw new ArgumentNullException(nameof(name));
                    if (name.Trim().Length == 0)
                        throw new ArgumentException("Color name must be non-empty.", nameof(name));

                    ConsoleColor c;
                    if (!colors.TryGetValue(name.ToLowerInvariant(), out c))
                        return null;
                    else
                        return c;
                }
                set
                {
                    if (name == null)
                        throw new ArgumentNullException(nameof(name));
                    if (name.Trim().Length == 0)
                        throw new ArgumentException("Color name must be non-empty.", nameof(name));

                    if (value.HasValue)
                        colors[name.ToLowerInvariant()] = value.Value;
                    else
                        colors.Remove(name.ToLowerInvariant());
                }
            }
        }
    }
}

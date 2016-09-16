using CommandLineParsing.Internals;
using System;
using System.Collections.Generic;
using System.Linq;
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
        private static ConsoleCache.Builder cacheBuilder;

        static ColorConsole()
        {
            colors = new ColorTable();
            cacheBuilder = null;
        }

        /// <summary>
        /// Gets the <see cref="ColorTable"/> that handles the available colornames for the <see cref="ColorConsole"/>.
        /// </summary>
        public static ColorTable Colors
        {
            get { return colors; }
        }

        /// <summary>
        /// Provides functionality for caching the console output.
        /// Cached output can be printed one line at a time, supporting up/down movement.
        /// </summary>
        public static class Caching
        {
            /// <summary>
            /// Starts caching the console output.
            /// Any calls to write content using the <see cref="ColorConsole"/> will not be visible in the console, but stored in the cache.
            /// Use <see cref="End"/> to retrieve the <see cref="ConsoleCache"/> constructed.
            /// </summary>
            public static void Start()
            {
                if (cacheBuilder != null)
                    throw new InvalidOperationException($"Caching is already started. End with {nameof(End)} or use the {nameof(Enabled)} property to check.");

                cacheBuilder = new ConsoleCache.Builder();
            }
            /// <summary>
            /// Ends caching.
            /// Anything that is printed when caching was enabled will be included in the resulting <see cref="ConsoleCache"/> object.
            /// </summary>
            /// <param name="write">If <c>true</c>, the cached result is written to the console using the default parameters for <see cref="ConsoleCache.Write(string, Action{ConsoleKeyInfo, ConsoleCache.DisplayChange})"/>.
            /// If <c>false</c>, nothing is written.</param>
            /// <returns>A <see cref="ConsoleCache"/> with all the lines that were captured by the caching.</returns>
            public static ConsoleCache End(bool write = false)
            {
                if (cacheBuilder == null)
                    throw new InvalidOperationException($"Caching must first be started, see {nameof(Start)}.");

                var cache = cacheBuilder.ConstructCache();
                cacheBuilder = null;
                if (write)
                    cache.Write();

                return cache;
            }

            /// <summary>
            /// Gets or sets a value indicating whether <see cref="Caching"/> is enabled.
            /// If it is, it can be ended using the <see cref="End"/> method; otherwise it can be started using the <see cref="Start"/> method.
            /// </summary>
            public static bool Enabled => cacheBuilder != null;
        }

        /// <summary>
        /// Gets or sets the position of the cursor within the buffer area.
        /// </summary>
        public static ConsolePoint CursorPosition
        {
            get { return new ConsolePoint(Console.CursorLeft, Console.CursorTop); }
            set { Console.SetCursorPosition(value.Left, value.Top); }
        }
        /// <summary>
        /// Gets or sets the position of the window area, relative to the screen buffer.
        /// </summary>
        public static ConsolePoint WindowPosition
        {
            get { return new ConsolePoint(Console.WindowLeft, Console.WindowTop); }
            set { Console.SetWindowPosition(value.Left, value.Top); }
        }
        /// <summary>
        /// Gets or sets the size of the console window.
        /// </summary>
        public static ConsoleSize WindowSize
        {
            get { return new ConsoleSize(Console.WindowWidth, Console.WindowHeight); }
            set { Console.SetWindowSize(value.Width, value.Height); }
        }
        /// <summary>
        /// Gets or sets the size of the buffer area.
        /// </summary>
        public static ConsoleSize BufferSize
        {
            get { return new ConsoleSize(Console.BufferWidth, Console.BufferHeight); }
            set { Console.SetBufferSize(value.Width, value.Height); }
        }

        /// <summary>
        /// Writes the specified string value to the standard output stream.
        /// </summary>
        /// <param name="value">The string format to write, included color information.
        /// The string "[Color:Text]" will print Text to the console using Color as the foreground color.</param>
        /// <param name="allowcolor">if set to <c>false</c> any color information passed in <paramref name="value"/> is disregarded.</param>
        public static void Write(string value, bool allowcolor = true)
        {
            foreach (var p in SimpleEvaluation.Evaluate(value, false))
            {
                var color = p.HasColor ? colors[p.Color] : null;
                if (allowcolor && color.HasValue)
                {
                    ConsoleColor temp = Console.ForegroundColor;
                    Console.ForegroundColor = color.Value;
                    write(p.Content);
                    Console.ForegroundColor = temp;
                }
                else
                    write(p.Content);
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
            Write(value + "\n", allowcolor);
        }

        private static void write(string value)
        {
            if (cacheBuilder != null)
                cacheBuilder.WriteString(value);
            else
                Console.Write(value);
        }

        /// <summary>
        /// Evaluates <paramref name="format"/> using a <see cref="IFormatter"/> and writes the result to the standard output stream.
        /// </summary>
        /// <param name="format">The string format that should be written. See <see cref="EvaluateFormat(string, IFormatter)"/> for details about the format.</param>
        /// <param name="formatter">The <see cref="IFormatter"/> that should be used to define the available elements in the format.</param>
        /// <param name="allowcolor">if set to <c>false</c> any color information passed in <paramref name="format"/> is disregarded.</param>
        public static void WriteFormat(string format, IFormatter formatter, bool allowcolor = true)
        {
            Write(EvaluateFormat(format, formatter), allowcolor);
        }
        /// <summary>
        /// Evaluates <paramref name="format"/> using a <see cref="IFormatter"/> and writes the result, followed by the current line terminator, to the standard output stream.
        /// </summary>
        /// <param name="format">The string format that should be written. See <see cref="EvaluateFormat(string, IFormatter)"/> for details about the format.</param>
        /// <param name="formatter">The <see cref="IFormatter"/> that should be used to define the available elements in the format.</param>
        /// <param name="allowcolor">if set to <c>false</c> any color information passed in <paramref name="format"/> is disregarded.</param>
        public static void WriteFormatLine(string format, IFormatter formatter, bool allowcolor = true)
        {
            WriteLine(EvaluateFormat(format, formatter), allowcolor);
        }

        private const string NO_CONDITION_FORMAT = "[red:UNKNOWN CONDITION '{0}']";
        private const string NO_FUNCTION_FORMAT = "[red:UNKNOWN FUNCTION/PARAMETER '{0}']";
        private const string NO_VARIABLE_FORMAT = "[red:UNKNOWN VARIABLE '{0}']";

        /// <summary>
        /// Evaluates <paramref name="format"/> using a <see cref="IFormatter"/> to specify to string translation.
        /// </summary>
        /// <param name="format">The string format that should be evaluated.</param>
        /// <param name="formatter">The <see cref="IFormatter"/> that should be used to define the available elements in the format.</param>
        /// <returns>The result of the string translation.</returns>
        /// <remarks>
        /// Text in the <paramref name="format"/> string is printed literally with the following exceptions:
        /// - <code>"$variable"</code> | Results in a call to <see cref="IFormatter.GetVariable(string)"/> with <code>"variable"</code> as parameter, replacing the variable with some other content.;
        /// - <code>"$variable+"</code>, <code>"$+variable"</code> or <code>"$+variable+"</code> | Allows for padding of a variable by calling <see cref="IFormatter.GetAlignedLength(string)"/> with <code>"variable"</code> as parameter. The location of the + indicates which end of the variable that is padded. $+variable+ indicates centering.
        /// - <code>"[color:text]"</code> | Prints <code>"text"</code> using <code>"color"</code> as color. The color string is looked up in <see cref="ColorConsole.Colors"/>.
        /// - <code>"[auto:text $variable text]"</code> | As the above, but calls <see cref="IFormatter.GetAutoColor(string)"/> with <code>"variable"</code> as parameter to obtain the color used before looking it up.
        /// - <code>"?condition{content}"</code> | Calls <see cref="IFormatter.ValidateCondition(string)"/> with <code>"condition"</code> as parameter and only prints <code>"content"</code> if the method returns true.
        /// - <code>"@function{arg1@arg2@arg3...}</code> | Calls <see cref="IFormatter.EvaluateFunction(string, string[])"/> with <code>"function"</code> as first parameter and an array with <code>{ "arg1", "arg2", "arg3, ...}</code> as second parameter.
        /// All of the above elements allow for nesting within each other.
        /// </remarks>
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
                            var match = Regex.Match(format.Substring(index), @"\?(!?)([^\{]*)");
                            var end = findEnd(format, index + match.Value.Length, '{', '}');
                            var block = format.Substring(index + match.Value.Length + 1, end - index - match.Value.Length - 1);

                            string replace = "";
                            var condition = formatter.ValidateCondition(match.Groups[2].Value);
                            var negate = match.Groups[1].Value == "!";

                            if (!condition.HasValue)
                                replace = string.Format(NO_CONDITION_FORMAT, match.Groups[2].Value);
                            else if (condition.Value ^ negate)
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
                                replace = string.Format(NO_FUNCTION_FORMAT, name);

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
            var match = Regex.Match(variable, @"^\+?([^\+]+)\+?$");
            if (!match.Success)
                return string.Format(NO_VARIABLE_FORMAT, variable);

            bool padLeft = variable[0] == '+';
            bool padRight = variable[variable.Length - 1] == '+';
            string variableName = match.Groups[1].Value;

            string res = formatter.GetVariable(variableName);

            if (res == null)
                return string.Format(NO_VARIABLE_FORMAT, variableName);

            bool preserveColor = formatter.GetPreserveColor(variableName);

            if (padLeft || padRight)
            {
                int? size = formatter.GetAlignedLength(variableName);
                if (size.HasValue)
                {
                    if (preserveColor)
                        size += res.Length - ClearColors(res).Length;

                    if (padLeft && padRight)
                        res = res.PadLeft(size.Value / 2).PadRight(size.Value - (size.Value / 2));
                    else if (padLeft)
                        res = res.PadLeft(size.Value);
                    else
                        res = res.PadRight(size.Value);
                }
            }

            return EscapeSpecialCharacters(res, !preserveColor);
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
            return SimpleEvaluation.Evaluate(input, true).Aggregate("", (r, t) => r + t.Content);
        }
        /// <summary>
        /// Escapes any special characters (including color-coding) such that a string can be printed literally using the <see cref="ColorConsole"/>.
        /// </summary>
        /// <param name="input">The string in which characters should be escaped.</param>
        /// <param name="escapeColor">If set to <c>true</c> color-coding information is escaped.</param>
        /// <returns>A new string, where all special characters are escaped.</returns>
        public static string EscapeSpecialCharacters(string input, bool escapeColor = true)
        {
            return SimpleEvaluation.EscapeSpecialCharacters(input, escapeColor);
        }
        /// <summary>
        /// Determines whether the specified string includes coloring syntax.
        /// </summary>
        /// <param name="input">The string that should be examined.</param>
        /// <returns><c>true</c>, if <paramref name="input"/> contains any "[Color:Text]" strings; otherwise, <c>false</c>.</returns>
        public static bool HasColors(string input)
        {
            return SimpleEvaluation.Evaluate(input, false).Any(x => x.HasColor);
        }

        private static SmartParser<T> getParser<T>()
        {
            string typename = typeof(T).Name;

            return new SmartParser<T>()
            {
                EnumIgnoreCase = true,
                NoParserExceptionMessage = $"The type { typename } is not supported. A {nameof(TryParse<T>)} or {nameof(MessageTryParse<T>)} method must be defined in {typename}.",
                NoValueMessage = Message.NoError,
                MultipleValuesMessage = "Only one value can be specified.",
                TypeErrorMessage = x => $"{x} is not a {typename} value.",
                UseParserMessage = true
            };
        }

        /// <summary>
        /// Writes <paramref name="prompt"/> to <see cref="Console"/>, reads user input and returns a parsed value.
        /// </summary>
        /// <typeparam name="T">The type of input that the method should accept.</typeparam>
        /// <param name="prompt">A prompt message to display to the user before input. <c>null</c> indicates that no prompt message should be displayed.</param>
        /// <param name="defaultString">A <see cref="string"/> that the inputtext is initialized to.
        /// The <see cref="string"/> can be edited in the <see cref="Console"/> and is part of the parsed <see cref="string"/> if not modified.
        /// <c>null</c> indicates that no initial value should be used.</param>
        /// <param name="parser">The <see cref="ParameterTryParse{T}"/> method that should be used when parsing the ReadLine input.<para />
        /// <c>null</c> indicates that the static <see cref="TryParse{T}"/> or <see cref="MessageTryParse{T}"/> method defined in <typeparamref name="T"/> should be used for parsing.
        /// An exception is thrown if such a method is not defined.</param>
        /// <param name="validator">The <see cref="Validator{T}"/> object that should be used to validate a parsed value.
        /// <c>null</c> indicates that no validation should be applied.</param>
        /// <returns>A <typeparamref name="T"/> element parsed from user input, that meets the requirements of <paramref name="validator"/>.</returns>
        public static T ReadLine<T>(string prompt = null, string defaultString = null, ParameterTryParse<T> parser = null, Validator<T> validator = null)
        {
            var smartparser = getParser<T>();
            if (parser != null)
                smartparser.Parser = parser;

            return ReadLine<T>(smartparser, prompt, defaultString, validator);
        }
        internal static T ReadLine<T>(SmartParser<T> parser, string prompt = null, string defaultString = null, Validator<T> validator = null)
        {
            if (ColorConsole.Caching.Enabled)
                throw new InvalidOperationException("ReadLine cannot be used while caching is enabled.");

            if (prompt != null)
                ColorConsole.Write(prompt);

            int l = Console.CursorLeft, t = Console.CursorTop;
            string input = "";
            T result = default(T);
            Message msg = Message.NoError;

            do
            {
                Console.SetCursorPosition(l, t);
                Console.Write(new string(' ', input.Length));
                Console.SetCursorPosition(l, t);

                input = ColorConsole.ReadLine(null, defaultString);
                string[] parseData = typeof(T).IsArray ? Command.SimulateParse(input) : new string[] { input };
                msg = parser.Parse(parseData, out result);

                if (!msg.IsError)
                    msg = validator == null ? Message.NoError : validator.Validate(result);

                if (msg.IsError)
                {
                    Console.CursorVisible = false;
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
            } while (msg.IsError);

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
            if (ColorConsole.Caching.Enabled)
                throw new InvalidOperationException("ReadLine cannot be used while caching is enabled.");

            if (prompt != null)
                ColorConsole.Write(prompt);

            var readline = new ReadLineHelper();
            readline.Insert(defaultString);

            while (true)
            {
                var info = Console.ReadKey(true);
                switch (info.Key)
                {
                    case ConsoleKey.Backspace:
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
                        break;

                    case ConsoleKey.Delete:
                        {
                            if (Console.CursorLeft == pos + sb.Length) continue;

                            int temp = Console.CursorLeft;
                            sb.Remove(Console.CursorLeft - pos, 1);
                            Console.Write(sb.ToString().Substring(Console.CursorLeft - pos) + " ");
                            Console.CursorLeft = temp;
                        }
                        break;

                    case ConsoleKey.Enter:
                        Console.Write(Environment.NewLine);
                        return readline.Value;

                    case ConsoleKey.LeftArrow:
                        readline.Index--;
                        break;
                    case ConsoleKey.RightArrow:
                        readline.Index++;
                        break;
                    case ConsoleKey.Home:
                        readline.Index = 0;
                        break;
                    case ConsoleKey.End:
                        readline.Index = readline.Length;
                        break;

                    default:
                        if (isConsoleChar(info))
                            readline.Insert(info.KeyChar);
                        break;
                }
            }
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
            if (ColorConsole.Caching.Enabled)
                throw new InvalidOperationException("ReadPassword cannot be used while caching is enabled.");

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
        /// Displays a <see cref="Menu{T}"/> or <see cref="SelectionMenu{T}"/> where a enumeration value of type <typeparamref name="TEnum"/> can be selected.
        /// The type of menu displayed is determined by whether the enum definition has the <see cref="FlagsAttribute"/> attribute.
        /// If it does, a combination of values can be selected (using a <see cref="SelectionMenu{T}"/>); otherwise only a single value can be selected (using a <see cref="Menu{T}"/>).
        /// </summary>
        /// <typeparam name="TEnum">The type of the enum.</typeparam>
        /// <param name="settings">A <see cref="MenuSettings"/> that expresses the settings used when displaying the menu, or <c>null</c> to use the default settings.</param>
        /// <returns>The selected <typeparamref name="TEnum"/> value.</returns>
        public static TEnum MenuSelectEnum<TEnum>(MenuSettings settings)
        {
            var flags = typeof(TEnum).GetCustomAttributes(typeof(FlagsAttribute), false).Length > 0;

            return MenuSelectEnum<TEnum>(settings, flags);
        }
        /// <summary>
        /// Displays a <see cref="Menu{T}"/> or <see cref="SelectionMenu{T}"/> where a enumeration value of type <typeparamref name="TEnum"/> can be selected.
        /// </summary>
        /// <typeparam name="TEnum">The type of the enum.</typeparam>
        /// <param name="settings">A <see cref="MenuSettings"/> that expresses the settings used when displaying the menu, or <c>null</c> to use the default settings.</param>
        /// <param name="allowflags">if set to <c>true</c> a combination of values can be selected (using a <see cref="SelectionMenu{T}"/>); otherwise only a single value can be selected (using a <see cref="Menu{T}"/>).</param>
        /// <returns>The selected <typeparamref name="TEnum"/> value.</returns>
        public static TEnum MenuSelectEnum<TEnum>(MenuSettings settings, bool allowflags)
        {
            if (!typeof(TEnum).IsEnum)
                throw new ArgumentException($"The {nameof(MenuSelectEnum)} method only support Enum types as type-parameter.");

            var values = (TEnum[])Enum.GetValues(typeof(TEnum));

            if (allowflags)
            {
                if (settings == null)
                    settings = new MenuSettings() { MinimumSelected = 1 };
                if (settings.MinimumSelected == 0)
                    settings = new MenuSettings(settings) { MinimumSelected = 1 };

                var selection = values.MenuSelectMultiple(settings);

                dynamic agg = selection[0];
                for (int i = 1; i < selection.Length; i++)
                    agg |= (dynamic)selection[i];

                return agg;
            }
            else
                return values.MenuSelect(settings);
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
                    else
                        name = name.Trim().ToLowerInvariant();

                    if (name.Length == 0)
                        return null;

                    ConsoleColor c;
                    if (!colors.TryGetValue(name, out c))
                        return null;
                    else
                        return c;
                }
                set
                {
                    if (name == null)
                        throw new ArgumentNullException(nameof(name));
                    else
                        name = name.Trim().ToLowerInvariant();

                    if (name.Length == 0)
                        throw new ArgumentException("Color name must be non-empty.", nameof(name));

                    if (value.HasValue)
                        colors[name] = value.Value;
                    else
                        colors.Remove(name.ToLowerInvariant());
                }
            }
        }

        private static class SimpleEvaluation
        {
            public struct Pair
            {
                public readonly string Content;
                public readonly string Color;

                public bool HasColor => Color != null;

                public Pair(string content, string color)
                {
                    this.Content = content;
                    this.Color = color;
                }

                public override string ToString() => HasColor ? $"[{Color}:{Content}]" : Content;
            }

            public static IEnumerable<Pair> Evaluate(string value, bool maintainEscape)
            {
                return mergeByColor(evaluate(value, maintainEscape, null));
            }
            private static IEnumerable<Pair> evaluate(string value, bool maintainEscape, string currentColor)
            {
                if (string.IsNullOrEmpty(value))
                    yield break;

                int index = 0;

                while (index < value.Length)
                    switch (value[index])
                    {
                        case '[': // Coloring
                            {
                                int end = findEnd(value, index, '[', ']');
                                var block = value.Substring(index + 1, end - index - 1);
                                int colon = block.IndexOf(':');
                                if (colon > 0 && block[colon - 1] == '\\')
                                    colon = -1;

                                if (colon == -1)
                                    yield return new Pair($"[{block}]", currentColor);
                                else
                                {
                                    var color = block.Substring(0, colon);
                                    string content = block.Substring(colon + 1);

                                    foreach (var p in evaluate(content, maintainEscape, color))
                                        yield return p;
                                }
                                index += block.Length + 2;
                            }
                            break;

                        case '\\':
                            if (value.Length == index + 1)
                                index++;
                            else
                            {
                                if (maintainEscape)
                                    yield return new Pair(value.Substring(index, 2), currentColor);
                                else
                                    yield return new Pair(value[index + 1].ToString(), currentColor);

                                index += 2;
                            }
                            break;

                        default: // Skip content
                            int nIndex = value.IndexOfAny(new char[] { '[', '\\' }, index);
                            if (nIndex < 0) nIndex = value.Length;
                            yield return new Pair(value.Substring(index, nIndex - index), currentColor);
                            index = nIndex;
                            break;
                    }
            }

            private static IEnumerable<Pair> mergeByColor(IEnumerable<Pair> pairs)
            {
                var e = pairs.GetEnumerator();

                if (!e.MoveNext())
                    yield break;

                var temp = e.Current;

                while (e.MoveNext())
                    if (temp.Color == e.Current.Color)
                        temp = new Pair(temp.Content + e.Current.Content, temp.Color);
                    else
                    {
                        yield return temp;
                        temp = e.Current;
                    }

                yield return temp;
            }

            public static string EscapeSpecialCharacters(string value, bool escapeColor)
            {
                if (escapeColor)
                    return value
                        .Replace("\\", "\\\\")
                        .Replace("[", "\\[")
                        .Replace("]", "\\]");
                else
                    return value
                        .Replace("\\", "\\\\");
            }
        }
    }
}

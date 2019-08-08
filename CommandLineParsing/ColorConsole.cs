using CommandLineParsing.Consoles;
using CommandLineParsing.Input;
using CommandLineParsing.Output;
using CommandLineParsing.Output.Formatting;
using CommandLineParsing.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
        private static IConsole _activeConsole;

        static ColorConsole()
        {
            colors = new ColorTable();
            cacheBuilder = null;
            _activeConsole = SystemConsole.Instance;
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
        /// Gets or sets the implementation of <see cref="IConsole"/> used by <see cref="ColorConsole"/>.
        /// This defaults to <see cref="SystemConsole.Instance"/>.
        /// </summary>
        public static IConsole ActiveConsole
        {
            get { return _activeConsole; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                _activeConsole = value;
            }
        }

        /// <summary>
        /// Gets or sets the position of the cursor within the buffer area.
        /// </summary>
        public static ConsolePoint CursorPosition
        {
            get { return new ConsolePoint(_activeConsole.CursorLeft, _activeConsole.CursorTop); }
            set { _activeConsole.SetCursorPosition(value.Left, value.Top); }
        }
        /// <summary>
        /// Gets or sets the position of the window area, relative to the screen buffer.
        /// </summary>
        public static ConsolePoint WindowPosition
        {
            get { return new ConsolePoint(_activeConsole.WindowLeft, _activeConsole.WindowTop); }
            set { _activeConsole.SetWindowPosition(value.Left, value.Top); }
        }
        /// <summary>
        /// Gets or sets the size of the console window.
        /// </summary>
        public static ConsoleSize WindowSize
        {
            get { return new ConsoleSize(_activeConsole.WindowWidth, _activeConsole.WindowHeight); }
            set { _activeConsole.SetWindowSize(value.Width, value.Height); }
        }
        /// <summary>
        /// Gets or sets the size of the buffer area.
        /// </summary>
        public static ConsoleSize BufferSize
        {
            get { return new ConsoleSize(_activeConsole.BufferWidth, _activeConsole.BufferHeight); }
            set { _activeConsole.SetBufferSize(value.Width, value.Height); }
        }

        /// <summary>
        /// Executes an operation and then restores <see cref="CursorPosition"/> to where it was when this method was called.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        /// <param name="hideCursor">if set to <c>true</c> the cursor will be hidden while executing <paramref name="action"/>.</param>
        public static void TemporaryShift(Action action, bool hideCursor = true)
        {
            bool wasVisible = _activeConsole.CursorVisible;

            var temp = ColorConsole.CursorPosition;
            if (hideCursor && wasVisible)
                _activeConsole.CursorVisible = false;

            action();

            if (hideCursor && wasVisible)
                _activeConsole.CursorVisible = true;
            ColorConsole.CursorPosition = temp;
        }
        /// <summary>
        /// Sets <see cref="CursorPosition"/> to <paramref name="point"/>, executes an operation and then restores <see cref="CursorPosition"/> to where it was when this method was called.
        /// </summary>
        /// <param name="point">The point to which <see cref="CursorPosition"/> should be shifted temporarily.</param>
        /// <param name="action">The action to execute.</param>
        /// <param name="hideCursor">if set to <c>true</c> the cursor will be hidden while executing <paramref name="action"/>.</param>
        public static void TemporaryShift(this ConsolePoint point, Action action, bool hideCursor = true)
        {
            TemporaryShift(() => { CursorPosition = point; action(); }, hideCursor);
        }

        /// <summary>
        /// Writes the specified string value to the standard output stream.
        /// </summary>
        /// <param name="value">The string format to write, included color information.
        /// The string "[Color:Text]" will print Text to the console using Color as the foreground color.</param>
        /// <param name="allowcolor">if set to <c>false</c> any color information passed in <paramref name="value"/> is disregarded.</param>
        public static void Write(ConsoleString value, bool allowcolor = true)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            foreach (var p in value)
            {
                var color = p.HasColor ? colors[p.Color] : null;
                if (allowcolor && color.HasValue)
                {
                    ConsoleColor temp = _activeConsole.ForegroundColor;
                    _activeConsole.ForegroundColor = color.Value;
                    write(p.Content);
                    _activeConsole.ForegroundColor = temp;
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
        public static void WriteLine(ConsoleString value, bool allowcolor = true)
        {
            Write(value + ConsoleString.Parse("\n", false), allowcolor);
        }

        private static void write(string value)
        {
            if (cacheBuilder != null)
                cacheBuilder.WriteString(value);
            else
                _activeConsole.Write(value);
        }

        /// <summary>
        /// Evaluates <paramref name="format"/> using a <see cref="IFormatter"/> and writes the result to the standard output stream.
        /// </summary>
        /// <param name="format">The string format that should be written. See <see cref="EvaluateFormat(string, IFormatter)"/> for details about the format.</param>
        /// <param name="formatter">The <see cref="IFormatter"/> that should be used to define the available elements in the format.</param>
        /// <param name="allowcolor">if set to <c>false</c> any color information passed in <paramref name="format"/> is disregarded.</param>
        public static void WriteFormat(string format, IFormatter formatter, bool allowcolor = true)
        {
            Write(ConsoleString.Parse(EvaluateFormat(format, formatter), false), allowcolor);
        }
        /// <summary>
        /// Evaluates <paramref name="format"/> using a <see cref="IFormatter"/> and writes the result, followed by the current line terminator, to the standard output stream.
        /// </summary>
        /// <param name="format">The string format that should be written. See <see cref="EvaluateFormat(string, IFormatter)"/> for details about the format.</param>
        /// <param name="formatter">The <see cref="IFormatter"/> that should be used to define the available elements in the format.</param>
        /// <param name="allowcolor">if set to <c>false</c> any color information passed in <paramref name="format"/> is disregarded.</param>
        public static void WriteFormatLine(string format, IFormatter formatter, bool allowcolor = true)
        {
            WriteLine(ConsoleString.Parse(EvaluateFormat(format, formatter), false), allowcolor);
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

            var consoleString = ConsoleString.Parse(res, false);
            if (preserveColor)
                consoleString = consoleString.EscapeColors();

            return consoleString.Content;
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
            return ConsoleString.Parse(input, true).ClearColors().Content;
        }
        /// <summary>
        /// Determines whether the specified string includes coloring syntax.
        /// </summary>
        /// <param name="input">The string that should be examined.</param>
        /// <returns><c>true</c>, if <paramref name="input"/> contains any "[Color:Text]" strings; otherwise, <c>false</c>.</returns>
        public static bool HasColors(string input)
        {
            return ConsoleString.Parse(input, false).HasColors;
        }

        private static ParserSettings GetParserSettings<T>()
        {
            string typename = typeof(T).Name;

            return new ParserSettings()
            {
                EnumIgnoreCase = true,
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
        /// <param name="cleanup">Determines the type of cleanup that should be applied after the line read has completed.</param>
        /// <param name="parser">The <see cref="ParameterTryParse{T}"/> method that should be used when parsing the ReadLine input.<para />
        /// <c>null</c> indicates that the static <see cref="TryParse{T}"/> or <see cref="MessageTryParse{T}"/> method defined in <typeparamref name="T"/> should be used for parsing.
        /// An exception is thrown if such a method is not defined.</param>
        /// <param name="validator">The <see cref="Validator{T}"/> object that should be used to validate a parsed value.
        /// <c>null</c> indicates that no validation should be applied.</param>
        /// <returns>A <typeparamref name="T"/> element parsed from user input, that meets the requirements of <paramref name="validator"/>.</returns>
        public static T ReadLine<T>(ConsoleString prompt = null, string defaultString = null, ReadLineCleanup cleanup = ReadLineCleanup.None, ParameterTryParse<T> parser = null, Validator<T> validator = null)
        {
            return ReadLine<T>(parser, parser == null ? GetParserSettings<T>() : null, prompt, defaultString, cleanup, validator);
        }
        internal static T ReadLine<T>(ParameterTryParse<T> customParser, ParserSettings parserSettings, ConsoleString prompt = null, string defaultString = null, ReadLineCleanup cleanup = ReadLineCleanup.None, Validator<T> validator = null)
        {
            readLine(customParser, parserSettings, out var result, prompt, defaultString, cleanup, ReadLineCleanup.None, validator);
            return result;
        }
        /// <summary>
        /// Reads and parses user input from <see cref="Console"/>, allowing the user to cancel input by pressing escape.
        /// </summary>
        /// <typeparam name="T">The type of input that the method should accept.</typeparam>
        /// <param name="result">The <typeparamref name="T"/> elememnt parsed from user input. If input could not be parsed on escape, this value is unknown.</param>
        /// <param name="prompt">A prompt message to display to the user before input. <c>null</c> indicates that no prompt message should be displayed.</param>
        /// <param name="defaultString">A <see cref="string"/> that the inputtext is initialized to.
        /// The <see cref="string"/> can be edited in the <see cref="Console"/> and is part of the returned <see cref="string"/> if not modified.
        /// <c>null</c> indicates that no initial value should be used.</param>
        /// <param name="cleanup">Determines the type of cleanup that should be applied after the line read has completed.</param>
        /// <param name="escapeCleanup">Determines the type of cleanup that should be applied if the readline did not complete succesfully.</param>
        /// <param name="parser">The parser.</param>
        /// <param name="validator">The <see cref="Validator{T}"/> object that should be used to validate a parsed value.
        /// <c>null</c> indicates that no validation should be applied.</param>
        /// <returns>A <see cref="bool"/> indicating weather the call completed without the user pressing escape.</returns>
        public static bool TryReadLine<T>(out T result, ConsoleString prompt = null, string defaultString = null, ReadLineCleanup cleanup = ReadLineCleanup.None, ReadLineCleanup escapeCleanup = ReadLineCleanup.RemoveAll, ParameterTryParse<T> parser = null, Validator<T> validator = null)
        {
            return TryReadLine<T>(parser, parser == null ? GetParserSettings<T>() : null, out result, prompt, defaultString, cleanup, escapeCleanup, validator);
        }
        internal static bool TryReadLine<T>(ParameterTryParse<T> customParser, ParserSettings parserSettings, out T result, ConsoleString prompt = null, string defaultString = null, ReadLineCleanup cleanup = ReadLineCleanup.None, ReadLineCleanup escapeCleanup = ReadLineCleanup.RemoveAll, Validator<T> validator = null)
        {
            return readLine(customParser, parserSettings, out result, prompt, defaultString, cleanup, escapeCleanup, validator);
        }

        private static bool readLine<T>(ParameterTryParse<T> customParser, ParserSettings parserSettings, out T result, ConsoleString prompt, string defaultString, ReadLineCleanup cleanup, ReadLineCleanup escapeCleanup, Validator<T> validator)
        {
            if (ColorConsole.Caching.Enabled)
                throw new InvalidOperationException("ReadLine cannot be used while caching is enabled.");

            var promptPosition = CursorPosition;
            if (prompt != null)
                ColorConsole.Write(prompt);

            var valuePosition = CursorPosition;
            bool cancelled = false;
            string input = "";
            result = default(T);
            Message msg = Message.NoError;

            do
            {
                CursorPosition = valuePosition;
                _activeConsole.Write(new string(' ', input.Length));
                CursorPosition = valuePosition;

                cancelled = !ColorConsole.TryReadLine(out input, null, defaultString, ReadLineCleanup.None, ReadLineCleanup.None);

                string[] parseData = typeof(T).IsArray ? Command.SimulateParse(input) : new string[] { input };
                if (customParser != null)
                    msg = customParser(parseData, out result);
                else
                    ParserLookup.TryParse(parserSettings, parseData, out result);

                if (cancelled)
                    break;

                if (!msg.IsError)
                    msg = validator == null ? Message.NoError : validator.Validate(result);

                if (msg.IsError)
                {
                    _activeConsole.CursorVisible = false;
                    CursorPosition = valuePosition;
                    _activeConsole.Write(new string(' ', input.Length));

                    input = msg.GetMessage();
                    _activeConsole.ForegroundColor = ConsoleColor.Red;
                    CursorPosition = valuePosition;
                    _activeConsole.Write(input);
                    _activeConsole.ResetColor();

                    _activeConsole.ReadKey(true);
                    _activeConsole.CursorVisible = true;
                }
            } while (msg.IsError);

            var cl = cancelled ? escapeCleanup : cleanup;
            if (cl != ReadLineCleanup.None)
            {
                CursorPosition = valuePosition;
                _activeConsole.Write(new string(' ', input.Length));

                CursorPosition = promptPosition;
                _activeConsole.Write(new string(' ', prompt.Length));
                CursorPosition = promptPosition;

                if (cl == ReadLineCleanup.RemovePrompt)
                    _activeConsole.WriteLine(input);
            }

            return !cancelled;
        }

        /// <summary>
        /// Reads a <see cref="string"/> from <see cref="Console"/>.
        /// </summary>
        /// <param name="prompt">A prompt message to display to the user before input. <c>null</c> indicates that no prompt message should be displayed.</param>
        /// <param name="defaultString">A <see cref="string"/> that the inputtext is initialized to.
        /// The <see cref="string"/> can be edited in the <see cref="Console"/> and is part of the returned <see cref="string"/> if not modified.
        /// <c>null</c> indicates that no initial value should be used.</param>
        /// <param name="cleanup">Determines the type of cleanup that should be applied after the line read has completed.</param>
        /// <returns>A <see cref="string"/> containing the user input.</returns>
        public static string ReadLine(ConsoleString prompt = null, string defaultString = null, ReadLineCleanup cleanup = ReadLineCleanup.None)
        {
            string result;
            readLine(out result, false, prompt, defaultString, cleanup, ReadLineCleanup.None);
            return result;
        }
        /// <summary>
        /// Reads a <see cref="string"/> from <see cref="Console"/>, allowing the user to cancel input by pressing escape.
        /// </summary>
        /// <param name="result">The string that was read from <see cref="Console"/>. This value is the same regardless if input was cancelled.</param>
        /// <param name="prompt">A prompt message to display to the user before input. <c>null</c> indicates that no prompt message should be displayed.</param>
        /// <param name="defaultString">A <see cref="string"/> that the inputtext is initialized to.
        /// The <see cref="string"/> can be edited in the <see cref="Console"/> and is part of the returned <see cref="string"/> if not modified.
        /// <c>null</c> indicates that no initial value should be used.</param>
        /// <param name="cleanup">Determines the type of cleanup that should be applied after the line read has completed.</param>
        /// <param name="escapeCleanup">Determines the type of cleanup that should be applied if the readline did not complete succesfully.</param>
        /// <returns>A <see cref="bool"/> indicating weather the call completed without the user pressing escape.</returns>
        public static bool TryReadLine(out string result, ConsoleString prompt, string defaultString, ReadLineCleanup cleanup, ReadLineCleanup escapeCleanup)
        {
            return readLine(out result, true, prompt, defaultString, cleanup, escapeCleanup);
        }
        /// <summary>
        /// Reads a password from <see cref="Console"/> without printing the input characters.
        /// </summary>
        /// <param name="prompt">A prompt message to display to the user before input. <c>null</c> indicates that no prompt message should be displayed.</param>
        /// <param name="passChar">An optional character to display in place of input symbols. <c>null</c> will display nothing to the user.</param>
        /// <param name="singleSymbol">if set to <c>true</c> <paramref name="passChar"/> will only be printed once, on input.</param>
        /// <returns>A <see cref="string"/> containing the password.</returns>
        public static string ReadPassword(ConsoleString prompt = null, char? passChar = '*', bool singleSymbol = true)
        {
            if (ColorConsole.Caching.Enabled)
                throw new InvalidOperationException("ReadPassword cannot be used while caching is enabled.");

            if (prompt != null)
                ColorConsole.Write(prompt);

            int pos = _activeConsole.CursorLeft;
            ConsoleKeyInfo info;

            StringBuilder sb = new StringBuilder(string.Empty);

            while (true)
            {
                info = _activeConsole.ReadKey(true);
                if (info.Key == ConsoleKey.Backspace)
                {
                    _activeConsole.CursorLeft = pos;
                    _activeConsole.Write(new string(' ', (singleSymbol || !passChar.HasValue) ? 1 : sb.Length));
                    _activeConsole.CursorLeft = pos;
                    sb.Clear();
                }

                else if (info.Key == ConsoleKey.Enter) { _activeConsole.Write(Environment.NewLine); break; }

                else if (ConsoleReader.IsInputCharacter(info))
                {
                    sb.Append(info.KeyChar);
                    if (passChar.HasValue)
                    {
                        if (!singleSymbol || sb.Length == 1)
                            _activeConsole.Write(passChar.Value);
                    }
                }
            }
            return sb.ToString();
        }

        private static bool readLine(out string result, bool allowEscape, ConsoleString prompt, string defaultString, ReadLineCleanup cleanup, ReadLineCleanup escapeCleanup)
        {
            if (ColorConsole.Caching.Enabled)
                throw new InvalidOperationException("ReadLine cannot be used while caching is enabled.");

            result = null;
            bool resultOk = false;
            ReadLineCleanup finalCleanup = ReadLineCleanup.None;

            bool done = false;

            using (var readline = new ConsoleReader(prompt))
            {
                readline.Insert(defaultString);

                while (!done)
                {
                    var info = _activeConsole.ReadKey(true);
                    switch (info.Key)
                    {
                        case ConsoleKey.Escape:
                        case ConsoleKey.Enter:
                            var escape = info.Key == ConsoleKey.Escape;
                            if (escape && !allowEscape)
                                continue;
                            var value = readline.Text;

                            finalCleanup = escape ? escapeCleanup : cleanup;
                            readline.Cleanup = finalCleanup == ReadLineCleanup.None ? InputCleanup.None : InputCleanup.Clean;

                            result = value;
                            resultOk = !escape;
                            done = true;
                            break;

                        default:
                            readline.HandleKey(info);
                            break;
                    }
                }
            }

            if (finalCleanup == ReadLineCleanup.RemovePrompt)
                _activeConsole.WriteLine(result);

            return resultOk;
        }

        /// <summary>
        /// Displays a menu where a enumeration value of type <typeparamref name="TEnum"/> can be selected.
        /// </summary>
        /// <typeparam name="TEnum">The type of the enum.</typeparam>
        /// <param name="keySelector">A function that gets the <see cref="ConsoleString"/> that should be displayed for each enum value.</param>
        /// <param name="labeling">The type of labeling (option prefix) that should be applied when displaying the menu.</param>
        /// <param name="cleanup">The cleanup applied after displaying the menu.</param>
        /// <param name="allowflags">If set to <c>true</c> a combination of values can be selected; otherwise only a single value can be selected.
        /// <c>null</c> indicates that multiple values can be selected if the type has the <see cref="FlagsAttribute"/>.
        /// </param>
        /// <returns>The selected <typeparamref name="TEnum"/> value.</returns>
        public static TEnum MenuSelectEnum<TEnum>(Func<TEnum, ConsoleString> keySelector = null, MenuLabeling labeling = MenuLabeling.NumbersAndLetters, MenuCleanup cleanup = MenuCleanup.None, bool? allowflags = null)
        {
            var typeinfo = typeof(TEnum).GetTypeInfo();

            if (!typeinfo.IsEnum)
                throw new ArgumentException($"The {nameof(MenuSelectEnum)} method only support Enum types as type-parameter.");

            if (!allowflags.HasValue)
                allowflags = typeinfo.GetCustomAttribute<FlagsAttribute>(false) != null;

            var values = (TEnum[])Enum.GetValues(typeof(TEnum));

            Func<IEnumerable<TEnum>, TEnum> merge = x =>
            {
                int val = (int)Convert.ChangeType(x.First(), typeof(int));
                foreach (var v in x.Skip(1))
                    val |= (int)Convert.ChangeType(v, typeof(int));

                return (TEnum)Enum.ToObject(typeof(TEnum), val);
            };

            if (allowflags.Value)
            {
                var selection = values.MenuSelectMultiple(isSelectionValid: x => x.Count() >= 1, onKeySelector: keySelector, labeling: labeling, cleanup: cleanup == MenuCleanup.RemoveMenuShowChoice ? MenuCleanup.RemoveMenu : cleanup);

                if (cleanup == MenuCleanup.RemoveMenuShowChoice)
                {
                    for (int i = 0; i < selection.Length; i++)
                    {
                        if (i > 0) _activeConsole.Write(", ");
                        ColorConsole.Write(selection[i].ToString());
                    }
                    _activeConsole.WriteLine();
                }

                long val = (long)Convert.ChangeType(selection[0], typeof(long));
                for (int i = 1; i < selection.Length; i++)
                    val |= (long)Convert.ChangeType(selection[i], typeof(long));

                return (TEnum)Enum.ToObject(typeof(TEnum), val);
            }
            else
            {
                return values.MenuSelect(keySelector, labeling, cleanup);
            }
        }
    }
}

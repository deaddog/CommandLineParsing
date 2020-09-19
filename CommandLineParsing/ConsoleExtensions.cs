using CommandLineParsing.Input;
using CommandLineParsing.Input.Reading;
using CommandLineParsing.Output;
using CommandLineParsing.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CommandLineParsing
{
    /// <summary>
    /// Provides methods for outputting color-coded text to the console using a simple format.
    /// The string "[Color:Text]" will print Text to the console using Color as the foreground color.
    /// </summary>
    public static class ConsoleExtensions
    {
        private static readonly ColorTable colors;

        static ConsoleExtensions()
        {
            colors = new ColorTable();
        }

        /// <summary>
        /// Gets the <see cref="ColorTable"/> that handles the available colornames for consoles/>.
        /// </summary>
        public static ColorTable Colors
        {
            get { return colors; }
        }

        /// <summary>
        /// Gets the position of the cursor within the buffer area.
        /// </summary>
        public static ConsolePoint GetCursorPosition(this IConsole console) => new ConsolePoint(console.CursorLeft, console.CursorTop);
        /// <summary>
        /// sets the position of the cursor within the buffer area.
        /// </summary>
        public static void SetCursorPosition(this IConsole console, ConsolePoint position) => console.SetCursorPosition(position.Left, position.Top);
        /// <summary>
        /// Gets the position of the window area, relative to the screen buffer.
        /// </summary>
        public static ConsolePoint GetWindowPosition(this IConsole console) => new ConsolePoint(console.WindowLeft, console.WindowTop);
        /// <summary>
        /// Sets the position of the window area, relative to the screen buffer.
        /// </summary>
        public static void SetWindowPosition(this IConsole console, ConsolePoint position) => console.SetWindowPosition(position.Left, position.Top);
        /// <summary>
        /// Gets the size of the console window.
        /// </summary>
        public static ConsoleSize GetWindowSize(this IConsole console) => new ConsoleSize(console.WindowWidth, console.WindowHeight);
        /// <summary>
        /// Sets the size of the console window.
        /// </summary>
        public static void SetWindowSize(this IConsole console, ConsoleSize size) => console.SetWindowSize(size.Width, size.Height);
        /// <summary>
        /// Gets the size of the buffer area.
        /// </summary>
        public static ConsoleSize GetBufferSize(this IConsole console) => new ConsoleSize(console.BufferWidth, console.BufferHeight);
        /// <summary>
        /// Sets the size of the buffer area.
        /// </summary>
        public static void SetBufferSize(this IConsole console, ConsoleSize size) => console.SetBufferSize(size.Width, size.Height);

        /// <summary>
        /// Executes an operation and then restores the cursor position to where it was when this method was called.
        /// </summary>
        /// <param name="console">The console on which the action is carried out.</param>
        /// <param name="action">The action to execute.</param>
        /// <param name="hideCursor">if set to <c>true</c> the cursor will be hidden while executing <paramref name="action"/>.</param>
        public static void TemporaryShift(this IConsole console, Action action, bool hideCursor = true)
        {
            bool wasVisible = console.CursorVisible;

            var temp = console.GetCursorPosition();
            if (hideCursor && wasVisible)
                console.CursorVisible = false;

            action();

            if (hideCursor && wasVisible)
                console.CursorVisible = true;
            console.SetCursorPosition(temp);
        }
        /// <summary>
        /// Sets the cursor position to <paramref name="point"/>, executes an operation, and then restores the cursor position to where it was when this method was called.
        /// </summary>
        /// <param name="console">The console on which the action is carried out.</param>
        /// <param name="point">The point to which the cursor position should be shifted temporarily.</param>
        /// <param name="action">The action to execute.</param>
        /// <param name="hideCursor">if set to <c>true</c> the cursor will be hidden while executing <paramref name="action"/>.</param>
        public static void TemporaryShift(this IConsole console, ConsolePoint point, Action action, bool hideCursor = true)
        {
            TemporaryShift(console, () => { console.SetCursorPosition(point); action(); }, hideCursor);
        }

        /// <summary>
        /// Writes the specified string value, followed by the current line terminator, to.
        /// </summary>
        /// <param name="console">The console on which the action is carried out.</param>
        /// <param name="value">The value to write.</param>
        public static void RenderLine(this IConsole console, string value)
        {
            console.Render(value + Environment.NewLine);
        }

        /// <summary>
        /// Writes the specified string value to the standard output stream.
        /// </summary>
        /// <param name="console">The console on which the action is carried out.</param>
        /// <param name="value">The string format to write, included color information.
        /// The string "[Color:Text]" will print Text to the console using Color as the foreground color.</param>
        /// <param name="allowcolor">if set to <c>false</c> any color information passed in <paramref name="value"/> is disregarded.</param>
        public static void Write(this IConsole console, ConsoleString value, bool allowcolor = true)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            foreach (var p in value)
            {
                var fgColor = (p.Color.HasForeground ? colors[p.Color.Foreground] : null) ?? console.ForegroundColor;
                var bgColor = (p.Color.HasBackground ? colors[p.Color.Background] : null) ?? console.BackgroundColor;
                if (allowcolor && (fgColor != console.ForegroundColor || bgColor != console.BackgroundColor))
                {
                    ConsoleColor tempFg = console.ForegroundColor;
                    ConsoleColor tempBg = console.BackgroundColor;
                    console.ForegroundColor = fgColor;
                    console.BackgroundColor = bgColor;
                    console.Render(p.Content);
                    console.ForegroundColor = tempFg;
                    console.BackgroundColor = tempBg;
                }
                else
                    console.Render(p.Content);
            }
        }
        /// <summary>
        /// Writes the specified string value, followed by the current line terminator, to the standard output stream.
        /// </summary>
        /// <param name="console">The console on which the action is carried out.</param>
        /// <param name="value">The string format to write, included color information.
        /// The string "[Color:Text]" will print Text to the console using Color as the foreground color.</param>
        /// <param name="allowcolor">if set to <c>false</c> any color information passed in <paramref name="value"/> is disregarded.</param>
        public static void WriteLine(this IConsole console, ConsoleString value, bool allowcolor = true)
        {
            Write(console, value + Environment.NewLine, allowcolor);
        }
        /// <summary>
        /// Writes the current line terminator to <paramref name="console"/>.
        /// </summary>
        /// <param name="console">The console used for writing.</param>
        public static void WriteLine(this IConsole console) => console.Render(Environment.NewLine);

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

        public static string ReadLine(this IConsole console) => new ConsoleReader(console).ReadLine();
        public static bool ReadLineOrCancel(this IConsole console, out string value) => new ConsoleReader(console).ReadLineOrCancel(out value);

        /// <summary>
        /// Displays a menu where a enumeration value of type <typeparamref name="TEnum"/> can be selected.
        /// </summary>
        /// <typeparam name="TEnum">The type of the enum.</typeparam>
        /// <param name="console">The console on which the action is carried out.</param>
        /// <param name="keySelector">A function that gets the <see cref="ConsoleString"/> that should be displayed for each enum value.</param>
        /// <param name="labeling">The type of labeling (option prefix) that should be applied when displaying the menu.</param>
        /// <param name="cleanup">The cleanup applied after displaying the menu.</param>
        /// <param name="allowflags">If set to <c>true</c> a combination of values can be selected; otherwise only a single value can be selected.
        /// <c>null</c> indicates that multiple values can be selected if the type has the <see cref="FlagsAttribute"/>.
        /// </param>
        /// <returns>The selected <typeparamref name="TEnum"/> value.</returns>
        public static TEnum MenuSelectEnum<TEnum>(this IConsole console, Func<TEnum, ConsoleString> keySelector = null, MenuLabeling labeling = MenuLabeling.NumbersAndLetters, MenuCleanup cleanup = MenuCleanup.None, bool? allowflags = null)
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
                var selection = console.MenuSelectMultiple(values, isSelectionValid: x => x.Count() >= 1, onKeySelector: keySelector, labeling: labeling, cleanup: cleanup == MenuCleanup.RemoveMenuShowChoice ? MenuCleanup.RemoveMenu : cleanup);

                if (cleanup == MenuCleanup.RemoveMenuShowChoice)
                {
                    for (int i = 0; i < selection.Length; i++)
                    {
                        if (i > 0) console.Render(", ");
                        console.Render(selection[i].ToString());
                    }
                    console.WriteLine();
                }

                long val = (long)Convert.ChangeType(selection[0], typeof(long));
                for (int i = 1; i < selection.Length; i++)
                    val |= (long)Convert.ChangeType(selection[i], typeof(long));

                return (TEnum)Enum.ToObject(typeof(TEnum), val);
            }
            else
            {
                return console.MenuSelect(values, keySelector, labeling, cleanup);
            }
        }
    }
}

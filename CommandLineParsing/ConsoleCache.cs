using System;
using System.Collections.Generic;

namespace CommandLineParsing
{
    /// <summary>
    /// Represents a collection of text-lines that are printable in the console.
    /// </summary>
    public class ConsoleCache
    {
        #region Capture console cache

        internal class Builder
        {
            private static readonly char[] control = new char[] { '\n', '\t', '\r', '\b', '\a' };

            private readonly int lineWidth;
            private int left;

            private List<ConsoleLine> lines = new List<ConsoleLine>();
            private ConsoleLine current;

            public Builder()
            {
                this.lineWidth = Console.BufferWidth;
                this.left = 0;

                this.lines = new List<ConsoleLine>();
                this.current = new ConsoleLine();
            }

            private void addCurrent()
            {
                lines.Add(current);
                current = new ConsoleLine();
                left = 0;
            }

            public void WriteString(string value)
            {
                if (value == null || value.Length == 0)
                    return;

                ConsoleColor fore = Console.ForegroundColor;
                ConsoleColor back = Console.BackgroundColor;

                int index = 0;
                while (index < value.Length)
                {
                    switch (value[index])
                    {
                        case '\n':
                            addCurrent();
                            index++;
                            break;

                        case '\t':
                            int l = left % 8 == 0 ? 8 : left % 8;
                            current.InsertSegment(new ConsoleSegment(new string(' ', l)), left);
                            left += l;
                            index++;
                            break;

                        case '\r':
                            left = 0;
                            index++;
                            break;

                        case '\b':
                            if (left > 0) left--;
                            index++;
                            break;

                        case '\a':
                            index++;
                            break;

                        default:
                            int nIndex = value.IndexOfAny(control, index);
                            if (nIndex < 0) nIndex = value.Length;

                            while (index < nIndex)
                            {
                                int len = nIndex - index;

                                if (len >= lineWidth - left)
                                {
                                    len = lineWidth - left;
                                    current.InsertSegment(new ConsoleSegment(value.Substring(index, len), fore, back), left);
                                    addCurrent();
                                    index += len;
                                }
                                else
                                {
                                    current.InsertSegment(new ConsoleSegment(value.Substring(index, len), fore, back), left);
                                    left += len;
                                    index += len;
                                }
                            }
                            break;
                    }
                }
            }

            public ConsoleCache ConstructCache()
            {
                if (!current.Empty)
                    addCurrent();

                var arr = lines.ToArray();
                lines.Clear();

                return new ConsoleCache(arr);
            }
        }

        #endregion

        private ConsoleLine[] lines;

        private ConsoleCache(ConsoleLine[] lines)
        {
            this.lines = lines;
        }

        /// <summary>
        /// Writes all the lines contained by this <see cref="ConsoleCache"/>.
        /// </summary>
        public void WriteAll()
        {
            if (Console.CursorLeft > 0)
                Console.WriteLine();

            if (lines.Length == 0)
                return;

            foreach (var l in lines)
                l.WriteToConsole();
        }

        /// <summary>
        /// Writes a page of the lines contained by this <see cref="ConsoleCache"/> and allows the user to moved up/down through lines.
        /// </summary>
        /// <param name="message">The message that should be displayed below the visible lines.</param>
        /// <param name="mover">A method that specifies which key input(s) signal up/down/quit.
        /// Specify <c>null</c> to use the default setup; Up, Down, PageUp, PageDown and Q.</param>
        public void Write(string message = ":", Action<ConsoleKeyInfo, DisplayChange> mover = null)
        {
            if (Console.CursorLeft > 0)
                Console.WriteLine();

            if (lines.Length == 0)
                return;

            if (message == null)
                message = "";

            if (message.Length >= Console.BufferWidth)
                throw new ArgumentOutOfRangeException(nameof(message), "Message must be smaller than the width of the console buffer.");

            if (mover == null)
                mover = defaultMover;

            LineWriter writer = new LineWriter(lines);

            while (writer.Offset < 0)
                if (!writer.ShowLine() || writer.Hidden == 0)
                    return;

            while (true)
            {
                Console.Write(message);
                var key = Console.ReadKey(true);
                Console.Write("\r" + new string(' ', message.Length) + "\r");

                DisplayChange display = new DisplayChange();
                mover(key, display);

                if (display.Offset < 0)
                    writer.HideLines(-display.Offset);
                else if (display.Offset > 0)
                    writer.ShowLines(display.Offset);

                if (display.Quit)
                    return;
            }
        }

        /// <summary>
        /// Represents a state-change when using the <see cref="ConsoleCache.Write(string, Action{ConsoleKeyInfo, DisplayChange})"/> method.
        /// </summary>
        public class DisplayChange
        {
            private int offset;
            private bool quit;

            /// <summary>
            /// Initializes a new instance of the <see cref="DisplayChange"/> class.
            /// </summary>
            public DisplayChange()
            {
                this.offset = 0;
                this.quit = false;
            }

            /// <summary>
            /// Indicates that an additional line should be visible.
            /// Call multiple times for multiple lines.
            /// </summary>
            public void ShowLine()
            {
                offset++;
            }
            /// <summary>
            /// Indicates that a line should be hidden.
            /// Call multiple times for multiple lines.
            /// </summary>
            public void HideLine()
            {
                offset--;
            }
            /// <summary>
            /// Indicates that an additional page should be visible.
            /// Call multiple times for multiple pages.
            /// </summary>
            public void ShowPage()
            {
                offset += Console.WindowHeight - 1;
            }
            /// <summary>
            /// Indicates that a full page should be hidden.
            /// Call multiple times for multiple pages.
            /// </summary>
            public void HidePage()
            {
                offset -= Console.WindowHeight - 1;
            }

            /// <summary>
            /// Gets or sets the change in line offset.
            /// A positive value of x will display x additional lines.
            /// A negative value of x will hide x lines.
            /// </summary>
            public int Offset
            {
                get { return offset; }
                set { offset = value; }
            }

            /// <summary>
            /// Gets or sets a value indicating whether the active <see cref="ConsoleCache.Write(string, Action{ConsoleKeyInfo, DisplayChange})"/> should termine.
            /// </summary>
            /// <value>
            ///   <c>true</c> if line-writing should quit; otherwise, <c>false</c>.
            /// </value>
            public bool Quit
            {
                get { return quit; }
                set { quit = value; }
            }
        }

        private static void defaultMover(ConsoleKeyInfo key, DisplayChange display)
        {
            switch (key.Key)
            {
                case ConsoleKey.DownArrow:
                    display.ShowLine();
                    break;

                case ConsoleKey.UpArrow:
                    display.HideLine();
                    break;

                case ConsoleKey.PageDown:
                    display.ShowPage();
                    break;

                case ConsoleKey.PageUp:
                    display.HidePage();
                    break;

                case ConsoleKey.Q:
                    display.Quit = true;
                    break;
            }
        }

        #region Write console cache

        private class LineWriter
        {
            public readonly int FirstLine;
            public int Offset => Console.CursorTop - FirstLine - Console.WindowHeight + 1;

            private Stack<ConsoleLine> visible;
            private Stack<ConsoleLine> hidden;

            public LineWriter(ConsoleLine[] lines)
            {
                this.FirstLine = Console.CursorTop;

                this.visible = new Stack<ConsoleLine>();
                this.hidden = new Stack<ConsoleLine>();

                for (int i = lines.Length - 1; i >= 0; i--)
                    hidden.Push(lines[i]);
            }

            public bool ShowLine()
            {
                if (hidden.Count > 0)
                {
                    var temp = hidden.Pop();
                    visible.Push(temp);
                    temp.WriteToConsole();

                    return true;
                }
                else
                    return false;
            }
            public bool HideLine()
            {
                if (Offset == 0)
                    return false;

                hidden.Push(visible.Pop());
                Console.SetCursorPosition(0, Console.CursorTop - 1);
                Console.Write(new string(' ', Console.WindowWidth));
                Console.SetCursorPosition(0, Console.CursorTop - 1);
                Console.WindowTop--;

                return true;
            }

            public void ShowLines(int count)
            {
                for (int i = 0; i < count; i++)
                    if (!ShowLine())
                        return;
            }
            public void HideLines(int count)
            {
                for (int i = 0; i < count; i++)
                    if (!HideLine())
                        return;
            }

            public int Visible => visible.Count;
            public int Hidden => hidden.Count;
        }

        #endregion
    }
}

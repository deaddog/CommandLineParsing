using System;

namespace CommandLineParsing.Consoles
{
    /// <summary>
    /// Provides methods and properties for interacting with the console.
    /// Use <see cref="SystemConsole"/> to interact with <see cref="Console"/>.
    /// </summary>
    public interface IConsole
    {
        /// <summary>
        /// Gets or sets the width of the buffer area.
        /// </summary>
        int BufferWidth { get; set; }
        /// <summary>
        /// Gets or sets the height of the buffer area.
        /// </summary>
        int BufferHeight { get; set; }
        /// <summary>
        /// Sets the height and width of the screen buffer area to the specified values.
        /// </summary>
        /// <param name="width">The width of the buffer area measured in columns.</param>
        /// <param name="height">The height of the buffer area measured in rows.</param>
        void SetBufferSize(int width, int height);
        /// <summary>
        /// Gets or sets the column position of the cursor within the buffer area.
        /// </summary>
        int CursorLeft { get; set; }
        /// <summary>
        /// Gets or sets the row position of the cursor within the buffer area.
        /// </summary>
        int CursorTop { get; set; }
        /// <summary>
        /// Sets the position of the cursor.
        /// </summary>
        /// <param name="left">The column position of the cursor. Columns are numbered from left to right starting at 0.</param>
        /// <param name="top">The row position of the cursor. Rows are numbered from top to bottom starting at 0.</param>
        void SetCursorPosition(int left, int top);

        /// <summary>
        /// Gets or sets the width of the console window.
        /// </summary>
        int WindowWidth { get; set; }
        /// <summary>
        /// Gets or sets the height of the console window area.
        /// </summary>
        int WindowHeight { get; set; }
        /// <summary>
        /// Sets the height and width of the console window to the specified values.
        /// </summary>
        /// <param name="width">The width of the console window measured in columns.</param>
        /// <param name="height">The height of the console window measured in rows.</param>
        void SetWindowSize(int width, int height);
        /// <summary>
        /// Gets or sets the leftmost position of the console window area relative to the screen buffer.
        /// </summary>
        int WindowLeft { get; set; }
        /// <summary>
        /// Gets or sets the top position of the console window area relative to the screen buffer.
        /// </summary>
        int WindowTop { get; set; }
        /// <summary>
        /// Sets the position of the console window relative to the screen buffer.
        /// </summary>
        /// <param name="left">The column position of the upper left corner of the console window.</param>
        /// <param name="top">The row position of the upper left corner of the console window.</param>
        void SetWindowPosition(int left, int top);

        /// <summary>
        /// Gets or sets a value indicating whether the cursor is visible.
        /// </summary>
        bool CursorVisible { get; set; }
        /// <summary>
        /// Gets or sets the foreground color of the console.
        /// </summary>
        ConsoleColor ForegroundColor { get; set; }
        /// <summary>
        /// Gets or sets the background color of the console.
        /// </summary>
        ConsoleColor BackgroundColor { get; set; }
        /// <summary>
        /// Sets the foreground and background console colors to their defaults.
        /// </summary>
        void ResetColor();

        /// <summary>
        /// Writes the specified string value to the console.
        /// </summary>
        /// <param name="value">The value to write.</param>
        void Write(string value);
        /// <summary>
        /// Writes the specified string value, followed by the current line terminator, to.
        /// </summary>
        /// <param name="value">The value to write.</param>
        void WriteLine(string value);
        /// <summary>
        /// Obtains the next character or function key pressed by the user. The pressed key is optionally displayed in the console window.
        /// </summary>
        /// <param name="intercept">Determines whether to display the pressed key in the console window. true to not display the pressed key; otherwise, false.</param>
        ConsoleKeyInfo ReadKey(bool intercept);
        /// <summary>
        /// Reads the next line of characters from the standard input stream.
        /// </summary>
        string ReadLine();
    }
}

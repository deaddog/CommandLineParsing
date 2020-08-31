using System;

namespace CommandLineParsing.Consoles
{
    /// <summary>
    /// Provides an implementation of <see cref="IConsole"/> that forwards method calls to <see cref="Console"/>
    /// </summary>
    public class SystemConsole : IConsole
    {
        private static SystemConsole _singleton = new SystemConsole();

        /// <summary>
        /// Gets the singleton instance of <see cref="SystemConsole"/>, used to interact with <see cref="Console"/>.
        /// </summary>
        public static SystemConsole Instance => _singleton;

        private SystemConsole() { }

        public int BufferWidth
        {
            get { return Console.BufferWidth; }
            set { Console.BufferWidth = value; }
        }
        public int BufferHeight
        {
            get { return Console.BufferHeight; }
            set { Console.BufferHeight = value; }
        }
        public void SetBufferSize(int width, int height) => Console.SetBufferSize(width, height);

        public int CursorLeft
        {
            get { return Console.CursorLeft; }
            set { Console.CursorLeft = value; }
        }
        public int CursorTop
        {
            get { return Console.CursorTop; }
            set { Console.CursorTop = value; }
        }
        public void SetCursorPosition(int left, int top) => Console.SetCursorPosition(left, top);

        public int WindowWidth
        {
            get { return Console.WindowWidth; }
            set { Console.WindowWidth = value; }
        }
        public int WindowHeight
        {
            get { return Console.WindowHeight; }
            set { Console.WindowHeight = value; }
        }
        public void SetWindowSize(int width, int height) => Console.SetWindowSize(width, height);

        public int WindowLeft
        {
            get { return Console.WindowLeft; }
            set { Console.WindowLeft = value; }
        }
        public int WindowTop
        {
            get { return Console.WindowTop; }
            set { Console.WindowTop = value; }
        }
        public void SetWindowPosition(int left, int top) => Console.SetWindowPosition(left, top);

        public bool CursorVisible
        {
            get { return Console.CursorVisible; }
            set { Console.CursorVisible = value; }
        }
        public ConsoleColor ForegroundColor
        {
            get { return Console.ForegroundColor; }
            set { Console.ForegroundColor = value; }
        }
        public ConsoleColor BackgroundColor
        {
            get { return Console.BackgroundColor; }
            set { Console.BackgroundColor = value; }
        }
        public void ResetColor() => Console.ResetColor();

        public void Render(string value) => Console.Write(value);
        public ConsoleKeyInfo ReadKey(bool intercept) => Console.ReadKey(intercept);
    }
}

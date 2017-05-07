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

        int IConsole.BufferWidth
        {
            get { return Console.BufferWidth; }
            set { Console.BufferWidth = value; }
        }
        int IConsole.BufferHeight
        {
            get { return Console.BufferHeight; }
            set { Console.BufferHeight = value; }
        }
        void IConsole.SetBufferSize(int width, int height) => Console.SetBufferSize(width, height);

        int IConsole.CursorLeft
        {
            get { return Console.CursorLeft; }
            set { Console.CursorLeft = value; }
        }
        int IConsole.CursorTop
        {
            get { return Console.CursorTop; }
            set { Console.CursorTop = value; }
        }
        void IConsole.SetCursorPosition(int left, int top) => Console.SetCursorPosition(left, top);

        int IConsole.WindowWidth
        {
            get { return Console.WindowWidth; }
            set { Console.WindowWidth = value; }
        }
        int IConsole.WindowHeight
        {
            get { return Console.WindowHeight; }
            set { Console.WindowHeight = value; }
        }
        void IConsole.SetWindowSize(int width, int height) => Console.SetWindowSize(width, height);

        int IConsole.WindowLeft
        {
            get { return Console.WindowLeft; }
            set { Console.WindowLeft = value; }
        }
        int IConsole.WindowTop
        {
            get { return Console.WindowTop; }
            set { Console.WindowTop = value; }
        }
        void IConsole.SetWindowPosition(int left, int top) => Console.SetWindowPosition(left, top);

        bool IConsole.CursorVisible
        {
            get { return Console.CursorVisible; }
            set { Console.CursorVisible = value; }
        }
        ConsoleColor IConsole.ForegroundColor
        {
            get { return Console.ForegroundColor; }
            set { Console.ForegroundColor = value; }
        }
        ConsoleColor IConsole.BackgroundColor
        {
            get { return Console.BackgroundColor; }
            set { Console.BackgroundColor = value; }
        }
        void IConsole.ResetColor() => Console.ResetColor();

        void IConsole.Write(string value) => Console.Write(value);
        void IConsole.WriteLine(string value) => Console.WriteLine(value);
        ConsoleKeyInfo IConsole.ReadKey(bool intercept) => Console.ReadKey(intercept);
        string IConsole.ReadLine() => Console.ReadLine();
    }
}

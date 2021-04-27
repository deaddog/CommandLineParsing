using System;

namespace ConsoleTools
{
    public static class Consoles
    {
        public static IConsole System { get; } = new SystemConsole();

        private class SystemConsole : IConsole
        {
            public Vector BufferSize => (Console.BufferWidth, Console.BufferHeight);
            public Vector CursorPosition
            {
                get => (Console.CursorLeft, Console.CursorTop);
                set => Console.SetCursorPosition(value.Horizontal, value.Vertical);
            }
            public Vector WindowSize => (Console.WindowWidth, Console.WindowHeight);
            public Vector WindowPosition => (Console.WindowLeft, Console.WindowHeight);

            public ConsoleColor ForegroundColor
            {
                get => Console.ForegroundColor;
                set => Console.ForegroundColor = value;
            }
            public ConsoleColor BackgroundColor
            {
                get => Console.BackgroundColor;
                set => Console.BackgroundColor = value;
            }
            public void ResetColor() => Console.ResetColor();

            public void Render(string value) => Console.Write(value);
            public ConsoleKeyInfo ReadKey(bool intercept) => Console.ReadKey(intercept);
        }
    }
}

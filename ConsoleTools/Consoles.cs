using ConsoleTools.Colors;
using System;

namespace ConsoleTools
{
    public static class Consoles
    {
        public static IConsole System { get; } = new SystemConsole(ColorTable.Default);
        public static IConsole SystemCustom(IColorTable colorTable) => new SystemConsole(colorTable);

        private class SystemConsole : IConsole
        {
            public SystemConsole(IColorTable colorTable)
            {
                ColorTable = colorTable ?? throw new ArgumentNullException(nameof(colorTable));
            }

            public Vector BufferSize => (Console.BufferWidth, Console.BufferHeight);
            public Vector CursorPosition
            {
                get => (Console.CursorLeft, Console.CursorTop);
                set => Console.SetCursorPosition(value.Horizontal, value.Vertical);
            }
            public Vector WindowSize => (Console.WindowWidth, Console.WindowHeight);
            public Vector WindowPosition => (Console.WindowLeft, Console.WindowHeight);

            public IColorTable ColorTable { get; }
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

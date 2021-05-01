using ConsoleTools.Colors;
using System;

namespace ConsoleTools
{
    public interface IConsole
    {
        Vector BufferSize { get; }
        Vector CursorPosition { get; set; }

        Vector WindowSize { get; }
        Vector WindowPosition { get; }

        IColorTable ColorTable {get;}
        ConsoleColor ForegroundColor { get; set; }
        ConsoleColor BackgroundColor { get; set; }
        void ResetColor();

        void Render(string value);
        ConsoleKeyInfo ReadKey(bool intercept);
    }
}

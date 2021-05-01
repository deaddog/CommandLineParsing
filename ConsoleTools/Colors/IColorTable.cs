using System;

namespace ConsoleTools.Colors
{
    public interface IColorTable
    {
        ConsoleColor? this[string name] { get; }
    }
}

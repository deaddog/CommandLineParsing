using System;

namespace ConsoleTools.Coloring
{
    public interface IColorTable
    {
        ConsoleColor? this[string name] { get; }
    }
}

using ConsoleTools.Formatting.Structure;

namespace ConsoleTools.Formatting
{
    public interface IFormatter<in T>
    {
        ConsoleString Format(Format format, T item);
    }
}

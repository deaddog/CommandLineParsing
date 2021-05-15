using ConsoleTools.Formatting.Structure;
using System.Collections.Immutable;

namespace ConsoleTools.Formatting
{
    public interface IFunction<T>
    {
        ConsoleString Evaluate(T item, IImmutableList<Format> arguments);
    }
}

using CommandLineParsing.Output.Formatting.Structure;
using System.Collections.Immutable;

namespace CommandLineParsing.Output.Formatting
{
    /// <summary>
    /// Represents a function that can be evaluated in a format.
    /// </summary>
    /// <param name="item">The item being formatted.</param>
    /// <param name="arguments">The format elements used as arguments in the format being evaluated.</param>
    /// <returns>A <see cref="ConsoleString"/> which is the result of evaluating the function.</returns>
    public delegate ConsoleString FunctionEvaluator<T>(T item, IImmutableList<FormatElement> arguments);
}

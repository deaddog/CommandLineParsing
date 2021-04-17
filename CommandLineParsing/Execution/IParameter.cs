using System.Collections.Immutable;

namespace CommandLineParsing.Execution
{
    public interface IParameter
    {
        ArgumentSet Resolve(ArgumentSet arguments, ImmutableArray<string> args);
    }
}

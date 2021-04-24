using System.Collections.Immutable;

namespace CommandLineParsing.Applications
{
    public interface IParameter
    {
        ArgumentSet Resolve(ArgumentSet arguments, string name, ImmutableArray<string> args);
    }
}

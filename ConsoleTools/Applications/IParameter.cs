using System;
using System.Collections.Immutable;

namespace ConsoleTools.Applications
{
    public interface IParameter
    {
        ImmutableArray<string> Names { get; }
        ArgumentSet Resolve(ImmutableArray<Input> values);
    }

    public class Input
    {
        public Input(string name, ImmutableArray<string> args)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Args = args;
        }

        public string Name { get; }
        public ImmutableArray<string> Args { get; }
    }
}

using System;
using System.Collections.Immutable;
using System.Linq;

namespace ConsoleTools.Formatting.Structure
{
    public class FunctionFormat : Format
    {
        public FunctionFormat(string name, IImmutableList<Format> arguments)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Arguments = arguments ?? throw new ArgumentNullException(nameof(arguments));

            for (int i = 0; i < arguments.Count; i++)
            {
                if (arguments[i] is null)
                    throw new ArgumentNullException(nameof(arguments), "One of the format arguments is null.");
            }
        }

        public string Name { get; }
        public IImmutableList<Format> Arguments { get; }

        public override bool Equals(Format? other)
        {
            return other is FunctionFormat obj &&
                Name.Equals(obj.Name) &&
                Arguments.Count == obj.Arguments.Count &&
                Arguments.Zip(obj.Arguments, (x, y) => x.Equals(y)).All(x => x);
        }
        public override int GetHashCode()
        {
            HashCode code = default;

            code.Add(Name);

            for (int i = 0; i < Arguments.Count; i++)
                code.Add(Arguments[i]);

            return code.ToHashCode();
        }
    }
}

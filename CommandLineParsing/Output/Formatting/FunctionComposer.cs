using System;
using System.Collections.Immutable;

#pragma warning disable CS1591 // Interface documentation should be enough.

namespace CommandLineParsing.Output.Formatting
{
    public class FunctionComposer<T> : FormatterComposer<T>, IFunctionComposer<T>
    {
        public FunctionComposer(Function<T> function,
            IImmutableDictionary<string, Variable<T>> variables,
            IImmutableDictionary<string, Condition<T>> conditions,
            IImmutableDictionary<string, Function<T>> functions) : base(variables, conditions, functions)
        {
            Function = function ?? throw new ArgumentNullException(nameof(function));
        }

        public Function<T> Function { get; }
    }
}

#pragma warning restore CS1591 // Interface documentation should be enough.

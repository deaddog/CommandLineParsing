using System;
using System.Collections.Immutable;

#pragma warning disable CS1591 // Interface documentation should be enough.

namespace CommandLineParsing.Output.Formatting
{
    public class VariableComposer<T> : FormatterComposer<T>, IVariableComposer<T>
    {
        public VariableComposer(Variable<T> variable,
            IImmutableDictionary<string, Variable<T>> variables,
            IImmutableDictionary<string, Condition<T>> conditions,
            IImmutableDictionary<string, Function<T>> functions) : base(variables, conditions, functions)
        {
            Variable = variable ?? throw new ArgumentNullException(nameof(variable));
        }

        public Variable<T> Variable { get; }
    }
}

#pragma warning restore CS1591 // Interface documentation should be enough.

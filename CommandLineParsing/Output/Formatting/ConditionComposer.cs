using System;
using System.Collections.Immutable;

#pragma warning disable CS1591 // Interface documentation should be enough.

namespace CommandLineParsing.Output.Formatting
{
    public class ConditionComposer<T> : FormatterComposer<T>, IConditionComposer<T>
    {
        public ConditionComposer(Condition<T> condition,
            IImmutableDictionary<string, Variable<T>> variables,
            IImmutableDictionary<string, Condition<T>> conditions,
            IImmutableDictionary<string, Function<T>> functions) : base(variables, conditions, functions)
        {
            Condition = condition ?? throw new ArgumentNullException(nameof(condition));
        }

        public Condition<T> Condition { get; }
    }
}

#pragma warning restore CS1591 // Interface documentation should be enough.

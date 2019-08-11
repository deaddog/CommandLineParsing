using System;
using System.Collections.Immutable;

namespace CommandLineParsing.Output.Formatting
{
    /// <summary>
    /// Provides methods for generating format composers.
    /// </summary>
    public static class FormatterComposer
    {
        /// <summary>
        /// Create a new composer for building a formatter.
        /// </summary>
        /// <typeparam name="T">The type of elements that the formatter should handle.</typeparam>
        /// <returns>A new format composer.</returns>
        public static FormatterComposer<T> Create<T>()
        {
            return new FormatterComposer<T>
            (
                ImmutableDictionary<string, Variable<T>>.Empty,
                ImmutableDictionary<string, Condition<T>>.Empty,
                ImmutableDictionary<string, Function<T>>.Empty
            );
        }
    }

#pragma warning disable CS1591 // Interface documentation should be enough.
    public class FormatterComposer<T> : IFormatterComposer<T>
    {
        public FormatterComposer(
            IImmutableDictionary<string, Variable<T>> variables,
            IImmutableDictionary<string, Condition<T>> conditions,
            IImmutableDictionary<string, Function<T>> functions)
        {
            Variables = variables ?? throw new ArgumentNullException(nameof(variables));
            Conditions = conditions ?? throw new ArgumentNullException(nameof(conditions));
            Functions = functions ?? throw new ArgumentNullException(nameof(functions));
        }

        public IFormatter<T> GetFormatter()
        {
            return new Formatter<T>
            (
                variables: Variables,
                conditions: Conditions,
                functions: Functions
            );
        }

        public IVariableComposer<T> With(Variable<T> variable)
        {
            return new VariableComposer<T>
            (
                variable: variable,
                variables: Variables.SetItem(variable.Name, variable),
                conditions: Conditions,
                functions: Functions
            );
        }
        public IConditionComposer<T> With(Condition<T> condition)
        {
            return new ConditionComposer<T>
            (
                condition: condition,
                variables: Variables,
                conditions: Conditions.SetItem(condition.Name, condition),
                functions: Functions
            );
        }
        public IFunctionComposer<T> With(Function<T> function)
        {
            return new FunctionComposer<T>
            (
                function: function,
                variables: Variables,
                conditions: Conditions,
                functions: Functions.SetItem(function.Name, function)
            );
        }

        public IImmutableDictionary<string, Variable<T>> Variables { get; }
        public IImmutableDictionary<string, Condition<T>> Conditions { get; }
        public IImmutableDictionary<string, Function<T>> Functions { get; }
    }
#pragma warning restore CS1591 // Interface documentation should be enough.
}

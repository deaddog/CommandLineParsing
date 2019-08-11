using System;
using System.Collections.Immutable;

#pragma warning disable CS1591 // Interface documentation should be enough.

namespace CommandLineParsing.Output.Formatting
{
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
}

#pragma warning restore CS1591 // Interface documentation should be enough.

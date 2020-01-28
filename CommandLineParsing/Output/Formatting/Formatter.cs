using CommandLineParsing.Output.Formatting.Helpers;
using CommandLineParsing.Output.Formatting.Structure;
using System;
using System.Collections.Immutable;
using System.Linq;

namespace CommandLineParsing.Output.Formatting
{
    /// <summary>
    /// Converts a format into console output using variable, condition and function definitions.
    /// </summary>
    /// <typeparam name="T">The type of elements that this formatter applies to.</typeparam>
    public class Formatter<T> : FormatVisitor<ConsoleString, T>, IFormatter<T>
    {
        private static ConsoleString GetErrorString(string error, string elementName)
        {
            return new ConsoleString(new[]
            {
                new ConsoleStringSegment($"[{error}:", Color.Parse("darkred")),
                new ConsoleStringSegment(elementName, Color.Parse("red")),
                new ConsoleStringSegment("]", Color.Parse("darkred"))
            });
        }

        private readonly VariablesExtractor _variablesExtractor;
        private readonly IImmutableDictionary<string, Variable<T>> _variables;
        private readonly IImmutableDictionary<string, Condition<T>> _conditions;
        private readonly IImmutableDictionary<string, Function<T>> _functions;

        /// <summary>
        /// Initializes a new formatter.
        /// </summary>
        /// <param name="variables">A variable dictionary in which <see cref="FormatVariableElement"/>s are looked up.</param>
        /// <param name="conditions">A condition dictionary in which <see cref="FormatConditionElement"/>s are looked up.</param>
        /// <param name="functions">A function dictionary in which <see cref="FormatFunctionElement"/>s are looked up.</param>
        public Formatter(
            IImmutableDictionary<string, Variable<T>> variables,
            IImmutableDictionary<string, Condition<T>> conditions,
            IImmutableDictionary<string, Function<T>> functions)
        {
            _variablesExtractor = new VariablesExtractor();
            _variables = variables ?? throw new ArgumentNullException(nameof(variables));
            _conditions = conditions ?? throw new ArgumentNullException(nameof(conditions));
            _functions = functions ?? throw new ArgumentNullException(nameof(functions));
        }

        ConsoleString IFormatter<T>.Format(FormatElement format, T item) => Visit(format, item);

#pragma warning disable CS1591 // Documentation of each Visit method is redundant

        public override ConsoleString Visit(FormatVariableElement format, T item)
        {
            if (_variables.TryGetValue(format.Name, out var variable))
            {
                var content = variable.Selector(item);

                int diff = (variable.PaddedLength ?? content.Length) - content.Length;

                if (diff > 0)
                {
                    switch (format.Padding)
                    {
                        case FormatVariablePaddings.PadLeft: return ConsoleString.FromContent(new string(' ', diff) + content);
                        case FormatVariablePaddings.PadRight: return ConsoleString.FromContent(content + new string(' ', diff));
                        case FormatVariablePaddings.PadBoth: return ConsoleString.FromContent(new string(' ', diff / 2) + content + new string(' ', diff - (diff / 2)));
                    }
                }

                return ConsoleString.FromContent(content);
            }
            else
                return GetErrorString("UNKNOWN VARIABLE", format.Name);
        }
        public override ConsoleString Visit(FormatColorElement format, T item)
        {
            var color = _variablesExtractor
                .Visit(format.Content)
                .Select(v => _variables.TryGetValue(v.Name, out var variable) && variable.DynamicColors.TryGetValue(format.Color, out var colorFunc) ? colorFunc : null)
                .FirstOrDefault(x => !(x is null))
                ?.Invoke(item) ?? Color.NoColor;

            var content = Visit(format.Content, item);
            var segments = content.Select(x => x.HasColor ? x : new ConsoleStringSegment(x.Content, color));
            return new ConsoleString(segments);
        }
        public override ConsoleString Visit(FormatConcatenationElement format, T item)
        {
            return format.Elements.Aggregate(seed: ConsoleString.Empty, (s, f) => s + Visit(f, item));
        }
        public override ConsoleString Visit(FormatConditionElement format, T item)
        {
            if (_conditions.TryGetValue(format.Name, out var condition))
            {
                if (condition.Predicate(item) != format.IsNegated)
                    return Visit(format.Content, item);
                else
                    return ConsoleString.Empty;
            }
            else
                return GetErrorString("UNKNOWN CONDITION", format.Name);
        }
        public override ConsoleString Visit(FormatFunctionElement format, T item)
        {
            if (_functions.TryGetValue(format.Name, out var function))
                return function.Evaluator(item, format.Arguments.ToImmutableList());
            else
                return GetErrorString("UNKNOWN FUNCTION", format.Name);
        }
        public override ConsoleString Visit(FormatNoContentElement format, T item)
        {
            return ConsoleString.Empty;
        }
        public override ConsoleString Visit(FormatTextElement format, T item)
        {
            return ConsoleString.FromContent(format.Text);
        }

#pragma warning restore CS1591 // Documentation of each Visit method is redundant
    }
}

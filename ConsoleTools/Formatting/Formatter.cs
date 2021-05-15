using ConsoleTools.Coloring;
using ConsoleTools.Formatting.Structure;
using ConsoleTools.Formatting.Visitors;
using System;
using System.Collections.Immutable;
using System.Linq;

namespace ConsoleTools.Formatting
{
    public class Formatter<T> : FormatVisitor<ConsoleString, T>, IFormatter<T>
    {
        private readonly IImmutableDictionary<string, Variable<T>> _variables;
        private readonly IImmutableDictionary<string, Predicate<T>> _conditions;
        private readonly IImmutableDictionary<string, IFunction<T>> _functions;

        public Formatter(
            IImmutableDictionary<string, Variable<T>> variables,
            IImmutableDictionary<string, Predicate<T>> conditions,
            IImmutableDictionary<string, IFunction<T>> functions)
        {
            _variables = variables ?? throw new ArgumentNullException(nameof(variables));
            _conditions = conditions ?? throw new ArgumentNullException(nameof(conditions));
            _functions = functions ?? throw new ArgumentNullException(nameof(functions));

            foreach (var v in _variables)
            {
                if (string.IsNullOrWhiteSpace(v.Key))
                    throw new ArgumentException("One of the variables does not have a name.", nameof(variables));
                if (!v.Key.Equals(v.Key.Trim(), StringComparison.OrdinalIgnoreCase))
                    throw new ArgumentException($"Variable name '{v.Key}' is padded.", nameof(variables));
            };

            foreach (var c in _conditions)
            {
                if (string.IsNullOrWhiteSpace(c.Key))
                    throw new ArgumentException("One of the conditions does not have a name.", nameof(conditions));
                if (!c.Key.Equals(c.Key.Trim(), StringComparison.OrdinalIgnoreCase))
                    throw new ArgumentException($"Condition name '{c.Key}' is padded.", nameof(conditions));
            };

            foreach (var f in _functions)
            {
                if (string.IsNullOrWhiteSpace(f.Key))
                    throw new ArgumentException("One of the functions does not have a name.", nameof(functions));
                if (!f.Key.Equals(f.Key.Trim(), StringComparison.OrdinalIgnoreCase))
                    throw new ArgumentException($"Function name '{f.Key}' is padded.", nameof(functions));
            };
        }

        public ConsoleString Format(Format format, T item) => Visit(format, item);

        public override ConsoleString Visit(VariableFormat format, T item)
        {
            if (_variables.TryGetValue(format.Name, out var variable))
            {
                var content = variable.Selector(item);

                int diff = (variable.PaddedLength ?? content.Length) - content.Length;

                if (diff > 0)
                {
                    switch (format.Padding)
                    {
                        case VariableFormat.Paddings.PadLeft: return ConsoleString.FromContent(new string(' ', diff) + content);
                        case VariableFormat.Paddings.PadRight: return ConsoleString.FromContent(content + new string(' ', diff));
                        case VariableFormat.Paddings.PadBoth: return ConsoleString.FromContent(new string(' ', diff / 2) + content + new string(' ', diff - (diff / 2)));
                    }
                }

                return ConsoleString.FromContent(content);
            }
            else
                return ConsoleString.FromContent($"${format.Name}", Colors.UnknownFormatVariable);
        }
        public override ConsoleString Visit(ColorFormat format, T item)
        {
            var color = VariablesExtractor.Instance
                .Visit(format.Content)
                .Select(v => _variables.TryGetValue(v.Name, out var variable) && variable.DynamicColors.TryGetValue(format.Color, out var colorFunc) ? colorFunc : null)
                .FirstOrDefault(x => !(x is null))
                ?.Invoke(item) ?? Color.Parse(format.Color);

            var content = Visit(format.Content, item);
            var segments = content.Select(x => x.HasColor ? x : new ConsoleString.Segment(x.Content, color));
            return new ConsoleString(segments.ToImmutableList());
        }
        public override ConsoleString Visit(ConcatenationFormat format, T item)
        {
            return format.Elements.Aggregate(seed: ConsoleString.Empty, (s, f) => s + Visit(f, item));
        }
        public override ConsoleString Visit(ConditionFormat format, T item)
        {
            if (_conditions.TryGetValue(format.Name, out var condition))
            {
                if (condition(item) != format.IsNegated)
                    return Visit(format.Content, item);
                else
                    return ConsoleString.Empty;
            }
            else
                return ConsoleString.FromContent($"?{format.Name}", Colors.UnknownFormatCondition);
        }
        public override ConsoleString Visit(FunctionFormat format, T item)
        {
            if (_functions.TryGetValue(format.Name, out var function))
                return function.Evaluate(item, format.Arguments.ToImmutableList());
            else
                return ConsoleString.FromContent($"@{format.Name}{{...}}", Colors.UnknownFormatFunction);
        }
        public override ConsoleString Visit(NoContentFormat format, T item)
        {
            return ConsoleString.Empty;
        }
        public override ConsoleString Visit(TextFormat format, T item)
        {
            return ConsoleString.FromContent(format.Text);
        }
    }
}

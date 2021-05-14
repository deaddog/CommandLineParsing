using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;

namespace ConsoleTools.Formatting.Structure
{
    public abstract class Format : IEquatable<Format>
    {
        public sealed override bool Equals(object? obj)
        {
            return Equals(obj as Format);
        }
        public abstract bool Equals(Format? other);
        public override int GetHashCode() => base.GetHashCode();

        public static Format Parse(string format)
        {
            int index = 0;
            return Parse(format, ref index);
        }

        public static Format Combine(Format element1, Format element2)
        {
            if (element1 is NoContentFormat)
                return element2;
            if (element2 is NoContentFormat)
                return element1;

            if (element1 is TextFormat text1 && element2 is TextFormat text2)
                return new TextFormat(text1.Text + text2.Text);

            if (element1 is ColorFormat color1 && element2 is ColorFormat color2 && color1.Color.Equals(color2.Color))
                return new ColorFormat(color1.Color, Combine(color1.Content, color2.Content));

            if (element1 is ConcatenationFormat c1)
            {
                var head1 = c1.Elements.Take(c1.Elements.Count - 1);
                var tail1 = c1.Elements[c1.Elements.Count - 1];

                if (element2 is ConcatenationFormat c2)
                {
                    var head2 = c2.Elements[0];
                    var tail2 = c2.Elements.Skip(1);

                    return new ConcatenationFormat
                    (
                        elements: ImmutableList
                            .CreateRange(head1)
                            .Add(Combine(tail1, head2))
                            .AddRange(tail2)
                    );
                }

                return new ConcatenationFormat
                (
                    elements: ImmutableList
                        .CreateRange(head1)
                        .Add(Combine(tail1, element2))
                );
            }
            else if (element2 is ConcatenationFormat c2)
            {
                var head2 = c2.Elements[0];
                var tail2 = c2.Elements.Skip(1);

                return new ConcatenationFormat
                (
                    elements: ImmutableList
                        .Create(Combine(element1, head2))
                        .AddRange(tail2)
                );
            }
            else
            {
                return new ConcatenationFormat
                (
                    elements: ImmutableList
                        .Create(element1)
                        .Add(element2)
                );
            }
        }

        private static Format Parse(string format, ref int index, params char[] stopat)
        {
            Format element = NoContentFormat.Element;

            while (index < format.Length)
            {
                switch (format[index])
                {
                    case '[': // Coloring
                        element = Combine(element, ParseColor(format, ref index));
                        break;

                    case '?': // Conditional
                        element = Combine(element, ParseCondition(format, ref index));
                        break;

                    case '@': // Listing/Function
                        element = Combine(element, ParseFunction(format, ref index));
                        break;

                    case '$': // Variable
                        element = Combine(element, ParseVariable(format, ref index));
                        break;
                    case '\\':
                        if (format.Length == index + 1)
                            index++;
                        else
                        {
                            element = Combine(element, new TextFormat(format.Substring(index + 1, 1)));
                            index += 2;
                        }
                        break;

                    default: // Skip content
                        var nextIndex = format.IndexOfAny(new char[] { '[', '?', '@', '$', '\\' }, index);
                        if (nextIndex < 0) nextIndex = format.Length;

                        var stopatIndex = format.IndexOfAny(stopat, index);
                        if (stopatIndex < nextIndex && stopatIndex >= 0)
                        {
                            if (stopatIndex - index > 0)
                            {
                                element = Combine(element, new TextFormat(format[index..stopatIndex]));
                                index = stopatIndex;
                            }
                            return element;
                        }
                        else
                        {
                            element = Combine(element, new TextFormat(format[index..nextIndex]));
                            index = nextIndex;
                        }
                        break;
                }
            }

            return element;
        }

        private static Format ParseColor(string format, ref int index)
        {
            index++;

            var colonIndex = format.IndexOf(':', index);
            var color = "";
            if (colonIndex >= 0)
            {
                color = format[index..colonIndex];
                index = colonIndex + 1;
            }

            var content = Parse(format, ref index, ']');
            if (index < format.Length && format[index] == ']')
                index++;

            if (string.IsNullOrWhiteSpace(color) || content is NoContentFormat)
                return content;
            else
                return new ColorFormat(color, content);
        }
        private static Format ParseVariable(string format, ref int index)
        {
            var match = Regex.Match(format[index..], @"^\$(\+?\p{L}[\w-_]*\+?)", RegexOptions.IgnoreCase);

            if (!match.Success)
            {
                index++;
                return new TextFormat("$");
            }
            else
            {
                index += match.Length;
                var variableName = match.Groups[1].Value;

                return new VariableFormat(variableName.Trim('+'), GetVariablePadding(variableName));
            }
        }
        private static Format ParseCondition(string format, ref int index)
        {
            var match = Regex.Match(format[index..], @"\?(!?)(\p{L}[\w-_]*)\{");

            if (!match.Success)
            {
                index++;
                return new TextFormat("?");
            }
            else
            {
                index += match.Length;
                var negate = match.Groups[1].Value == "!";
                var variableName = match.Groups[2].Value;
                var content = Parse(format, ref index, '}');
                if (index < format.Length && format[index] == '}')
                    index++;

                return new ConditionFormat(variableName, negate, content);
            }
        }
        private static Format ParseFunction(string format, ref int index)
        {
            var match = Regex.Match(format[index..], @"\@(\p{L}[\w-_]*)\{");

            if (!match.Success)
            {
                index++;
                return new TextFormat("@");
            }
            else
            {
                index += match.Length;
                var functionName = match.Groups[1].Value;

                var arguments = ImmutableList<Format>.Empty;
                while (index < format.Length && format[index - 1] != '}')
                {
                    arguments = arguments.Add(Parse(format, ref index, '}', ','));
                    index++;
                }

                return new FunctionFormat(functionName, arguments);
            }
        }

        private static VariableFormat.Paddings GetVariablePadding(string variableWithPadding)
        {
            if (variableWithPadding[0] == '+')
            {
                if (variableWithPadding[variableWithPadding.Length - 1] == '+')
                    return VariableFormat.Paddings.PadBoth;
                else
                    return VariableFormat.Paddings.PadLeft;
            }
            else
            {
                if (variableWithPadding[variableWithPadding.Length - 1] == '+')
                    return VariableFormat.Paddings.PadRight;
                else
                    return VariableFormat.Paddings.None;
            }
        }
    }
}

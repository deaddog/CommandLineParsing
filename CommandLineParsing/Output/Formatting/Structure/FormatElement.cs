using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CommandLineParsing.Output.Formatting.Structure
{
    /// <summary>
    /// Represents part of a format syntax tree.
    /// </summary>
    public abstract class FormatElement : IEquatable<FormatElement>
    {
#pragma warning disable CS1591
        public abstract bool Equals(FormatElement other);
#pragma warning restore CS1591
        
        /// <summary>
        /// Parses a string format into a <see cref="FormatElement"/> structure.
        /// The parsed structure can be evaluated and printed in the console using a <see cref="IFormatter"/>.
        /// </summary>
        /// <param name="format">The string format that should be parsed.</param>
        /// <returns>A tree structure represented the parsed string.</returns>
        /// <remarks>
        /// Text in the <paramref name="format"/> string is represented literally with the following exceptions:
        /// - <code>"$variable"</code> | Represents a variable, that can be replaced by text.
        /// - <code>"$variable+"</code>, <code>"$+variable"</code> or <code>"$+variable+"</code> | Allows for padding of a variable. The location of the + indicates which end of the variable that is padded. $+variable+ indicates centering.
        /// - <code>"[color:text]"</code> | Uses the same color-syntax as <see cref="ConsoleString"/>.
        /// - <code>"[auto:text $variable text]"</code> | As the above, but uses <code>"variable"</code> to obtain the color used when rendering the string.
        /// - <code>"?condition{content}"</code> | Represents a conditional format segment.
        /// - <code>"@function{arg1,arg2,arg3...}</code> | Represents evaluation of a format function (such as for listing items).
        /// All of the above elements allow for nesting within each other.
        /// </remarks>
        public static FormatElement Parse(string format)
        {
            int index = 0;
            return Parse(format, ref index);
        }

        /// <summary>
        /// Combines two format elements, flattening when possible.
        /// </summary>
        /// <param name="element1">The first element.</param>
        /// <param name="element2">The second element.</param>
        /// <returns>
        /// The combined format element; possibly a <see cref="FormatConcatenationElement"/>.
        /// </returns>
        public static FormatElement operator +(FormatElement element1, FormatElement element2)
        {
            if (element1 is FormatNoContentElement)
                return element2;
            else if (element2 is FormatNoContentElement)
                return element1;

            if (element1 is FormatTextElement text1 && element2 is FormatTextElement text2)
                return new FormatTextElement(text1.Text + text2.Text);

            if (element1 is FormatColorElement color1 && element2 is FormatColorElement color2 && color1.Color.Equals(color2.Color))
                return new FormatColorElement(color1.Color, color1.Content + color2.Content);

            if (element1 is FormatConcatenationElement c1)
                return c1 + element2;

            if (element2 is FormatConcatenationElement c2)
                return element1 + c2;

            return new FormatConcatenationElement(new[] { element1, element2 });
        }

        private static FormatElement Parse(string format, ref int index, params char[] stopat)
        {
            FormatElement element = FormatNoContentElement.Element;

            while (index < format.Length)
            {
                switch (format[index])
                {
                    case '[': // Coloring
                        element += ParseColor(format, ref index);
                        break;

                    case '?': // Conditional
                        element += ParseCondition(format, ref index);
                        break;

                    case '@': // Listing/Function
                        element += ParseFunction(format, ref index);
                        break;

                    case '$': // Variable
                        element += ParseVariable(format, ref index);
                        break;
                    case '\\':
                        if (format.Length == index + 1)
                            index++;
                        else
                        {
                            element += new FormatTextElement(format.Substring(index + 1, 1));
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
                                element += new FormatTextElement(format.Substring(index, stopatIndex - index));
                                index = stopatIndex;
                            }
                            return element;
                        }
                        else
                        {
                            element += new FormatTextElement(format.Substring(index, nextIndex - index));
                            index = nextIndex;
                        }
                        break;
                }
            }

            return element;
        }

        private static FormatElement ParseColor(string format, ref int index)
        {
            index++;

            var colonIndex = format.IndexOf(':', index);
            var color = "";
            if (colonIndex >= 0)
            {
                color = format.Substring(index, colonIndex - index);
                index = colonIndex + 1;
            }

            var content = Parse(format, ref index, ']');
            if (index < format.Length && format[index] == ']')
                index++;

            if (string.IsNullOrWhiteSpace(color) || content is FormatNoContentElement)
                return content;
            else
                return new FormatColorElement(color, content);
        }
        private static FormatElement ParseVariable(string format, ref int index)
        {
            var match = Regex.Match(format.Substring(index), @"^\$(\+?\p{L}[\w-_]*\+?)", RegexOptions.IgnoreCase);

            if (!match.Success)
            {
                index++;
                return new FormatTextElement("$");
            }
            else
            {
                index += match.Length;
                var variableName = match.Groups[1].Value;

                return new FormatVariableElement(variableName.Trim('+'), GetVariablePadding(variableName));
            }
        }
        private static FormatElement ParseCondition(string format, ref int index)
        {
            var match = Regex.Match(format.Substring(index), @"\?(!?)(\p{L}[\w-_]*)\{");

            if (!match.Success)
            {
                index++;
                return new FormatTextElement("?");
            }
            else
            {
                index += match.Length;
                var negate = match.Groups[1].Value == "!";
                var variableName = match.Groups[2].Value;
                var content = Parse(format, ref index, '}');
                if (index < format.Length && format[index] == '}')
                    index++;

                return new FormatConditionElement(variableName, negate, content);
            }
        }
        private static FormatElement ParseFunction(string format, ref int index)
        {
            var match = Regex.Match(format.Substring(index), @"\@(\p{L}[\w-_]*)\{");

            if (!match.Success)
            {
                index++;
                return new FormatTextElement("@");
            }
            else
            {
                index += match.Length;
                var functionName = match.Groups[1].Value;

                var arguments = new List<FormatElement>();
                while (index < format.Length && format[index - 1] != '}')
                {
                    arguments.Add(Parse(format, ref index, '}', ','));
                    index++;
                }

                if (arguments.Count == 0)
                    arguments.Add(FormatNoContentElement.Element);

                return new FormatFunctionElement(functionName, arguments);
            }
        }

        private static FormatVariablePaddings GetVariablePadding(string variableWithPadding)
        {
            if (variableWithPadding[0] == '+')
            {
                if (variableWithPadding[variableWithPadding.Length - 1] == '+')
                    return FormatVariablePaddings.PadBoth;
                else
                    return FormatVariablePaddings.PadLeft;
            }
            else
            {
                if (variableWithPadding[variableWithPadding.Length - 1] == '+')
                    return FormatVariablePaddings.PadRight;
                else
                    return FormatVariablePaddings.None;
            }
        }
    }
}

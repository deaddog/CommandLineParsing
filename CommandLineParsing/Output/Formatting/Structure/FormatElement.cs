using System;
using System.Collections.Generic;

namespace CommandLineParsing.Output.Formatting.Structure
{
    /// <summary>
    /// Represents part of a format syntax tree.
    /// </summary>
    public abstract class FormatElement
    {
        public static FormatElement Parse(string format)
        {
            int index = 0;
            return Parse(format, ref index);
        }

        private static FormatElement Parse(string format, ref int index)
        {
            var elements = new List<FormatElement>();

            while (index < format.Length)
            {
                switch (format[index])
                {
                    case '[': // Coloring
                        elements.Add(ParseColor(format, ref index));
                        break;

                    case '?': // Conditional
                        elements.Add(ParseCondition(format, ref index));
                        break;

                    case '@': // Listing/Function
                        elements.Add(ParseFunction(format, ref index));
                        break;

                    case '$': // Variable
                        elements.Add(ParseVariable(format, ref index));
                        break;
                    case '\\':
                        if (format.Length == index + 1)
                            index++;
                        else
                        {
                            elements.Add(new FormatText(format.Substring(index + 1, 1)));
                            index++;
                        }
                        break;

                    default: // Skip content
                        var nextIndex = format.IndexOfAny(new char[] { '[', '?', '@', '$', '\\' }, index);
                        if (nextIndex < 0) nextIndex = format.Length;
                        elements.Add(new FormatText(format.Substring(index, nextIndex - index)));
                        index = nextIndex;
                        break;
                }

                if (elements.Count > 1 &&
                    elements[elements.Count - 1] is FormatText current &&
                    elements[elements.Count - 2] is FormatText previous)
                {
                    elements.RemoveRange(elements.Count - 2, 2);
                    elements.Add(new FormatText(previous.Text + current.Text));
                }
            }

            if (elements.Count == 0)
                return new FormatText(string.Empty);
            else if (elements.Count == 1)
                return elements[0];
            else
                return new FormatConcatenation(elements);
        }

        private static FormatElement ParseColor(string format, ref int index)
        {
            throw new NotImplementedException();
        }
        private static FormatElement ParseVariable(string format, ref int index)
        {
            throw new NotImplementedException();
        }
        private static FormatElement ParseCondition(string format, ref int index)
        {
            throw new NotImplementedException();
        }
        private static FormatElement ParseFunction(string format, ref int index)
        {
            throw new NotImplementedException();
        }
    }
}

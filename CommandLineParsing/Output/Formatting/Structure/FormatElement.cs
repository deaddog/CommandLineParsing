using System;
using System.Linq;

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

        public static FormatElement Parse(string format)
        {
            int index = 0;
            return Parse(format, ref index);
        }

        public static FormatElement operator +(FormatElement element1, FormatElement element2)
        {
            if (element1 is FormatNoContent)
                return element2;
            else if (element2 is FormatNoContent)
                return element1;

            if (element1 is FormatText text1 && element2 is FormatText text2)
                return new FormatText(text1.Text + text2.Text);

            if (element1 is FormatColor color1 && element2 is FormatColor color2 && color1.Color.Equals(color2.Color))
                return new FormatColor(color1.Color, color1.Content + color2.Content);

            if (element1 is FormatConcatenation c1)
            {
                if (element2 is FormatConcatenation c2)
                    return new FormatConcatenation(c1.Elements.Concat(c2.Elements));
                else
                    return new FormatConcatenation(c1.Elements.Concat(new[] { element2 }));
            }
            else if (element2 is FormatConcatenation c2)
                return new FormatConcatenation(new[] { element2 }.Concat(c2.Elements));

            return new FormatConcatenation(new[] { element1, element2 });
        }

        private static FormatElement Parse(string format, ref int index, char? stopat = null)
        {
            var stopChar = stopat ?? '\\';
            FormatElement element = FormatNoContent.Element;

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
                            element += new FormatText(format.Substring(index + 1, 1));
                            index++;
                        }
                        break;

                    default: // Skip content
                        var nextIndex = format.IndexOfAny(new char[] { '[', '?', '@', '$', '\\' }, index);
                        if (nextIndex < 0) nextIndex = format.Length;

                        var stopatIndex = format.IndexOf(stopChar, index);
                        if (stopatIndex < nextIndex && stopatIndex >= 0)
                        {
                            if (stopatIndex - index > 0)
                            {
                                element += new FormatText(format.Substring(index, stopatIndex - index));
                                index = stopatIndex;
                            }
                            return element;
                        }
                        else
                        {
                            element += new FormatText(format.Substring(index, nextIndex - index));
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
            if (format[index] == ']')
                index++;
            
            if (string.IsNullOrWhiteSpace(color) || content is FormatNoContent)
                return content;
            else
                return new FormatColor(color, content);
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

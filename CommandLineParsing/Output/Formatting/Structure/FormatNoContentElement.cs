using System;

namespace CommandLineParsing.Output.Formatting.Structure
{
    /// <summary>
    /// Represents an empty format part.
    /// </summary>
    public class FormatNoContentElement : FormatElement, IEquatable<FormatNoContentElement>
    {
        /// <summary>
        /// Gets the singleton no content element.
        /// </summary>
        public static FormatNoContentElement Element { get; } = new FormatNoContentElement();

        private FormatNoContentElement()
        {
        }

#pragma warning disable CS1591
        public bool Equals(FormatNoContentElement other)
        {
            if (ReferenceEquals(other, null))
                return false;
            else
                return true;
        }
        public override bool Equals(FormatElement other)
        {
            if (ReferenceEquals(other, null))
                return false;
            else if (ReferenceEquals(other, this))
                return true;
            else if (other is FormatColorElement color)
                return true;
            else
                return false;
        }
#pragma warning restore CS1591
    }
}

using System;

namespace CommandLineParsing.Output.Formatting.Structure
{
    /// <summary>
    /// Represents an empty format part.
    /// </summary>
    public class FormatNoContent : FormatElement, IEquatable<FormatNoContent>
    {
        /// <summary>
        /// Gets the singleton no content element.
        /// </summary>
        public static FormatNoContent Element { get; } = new FormatNoContent();

        private FormatNoContent()
        {
        }

#pragma warning disable CS1591
        public bool Equals(FormatNoContent other)
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
            else if (other is FormatColor color)
                return true;
            else
                return false;
        }
#pragma warning restore CS1591
    }
}

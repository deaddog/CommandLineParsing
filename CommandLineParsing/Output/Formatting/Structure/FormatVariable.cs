using System;

namespace CommandLineParsing.Output.Formatting.Structure
{
    /// <summary>
    /// Represents a variable part of a format structure.
    /// </summary>
    public class FormatVariable : FormatElement, IEquatable<FormatVariable>
    {
        /// <summary>
        /// Gets the name of the variable.
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// Gets the type of padding that should be applied to the variable.
        /// </summary>
        public FormatPaddings Padding { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FormatVariable"/> class.
        /// </summary>
        /// <param name="name">The name of the variable.</param>
        /// <param name="padding">The type of padding applied to the variable.</param>
        public FormatVariable(string name, FormatPaddings padding)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Padding = padding;

            if (Name.Contains(" "))
                throw new ArgumentException("Variable names cannot contain spaces.", nameof(name));
        }

#pragma warning disable CS1591
        public override int GetHashCode()
        {
            return Name.GetHashCode() ^ Padding.GetHashCode();
        }

        public bool Equals(FormatVariable other)
        {
            if (ReferenceEquals(other, null))
                return false;
            else if (ReferenceEquals(other, this))
                return true;
            else
                return Name.Equals(other.Name) && Padding.Equals(other.Padding);
        }
        public override bool Equals(FormatElement other)
        {
            if (ReferenceEquals(other, null))
                return false;
            else if (ReferenceEquals(other, this))
                return true;
            else if (other is FormatVariable var)
                return Equals(var);
            else
                return false;
        }
#pragma warning restore CS1591
    }
}

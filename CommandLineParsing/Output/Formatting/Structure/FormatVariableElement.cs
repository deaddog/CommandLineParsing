using System;

namespace CommandLineParsing.Output.Formatting.Structure
{
    /// <summary>
    /// Represents a variable part of a format structure.
    /// </summary>
    public class FormatVariableElement : FormatElement, IEquatable<FormatVariableElement>
    {
        /// <summary>
        /// Gets the name of the variable.
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// Gets the type of padding that should be applied to the variable.
        /// </summary>
        public FormatVariablePaddings Padding { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FormatVariableElement"/> class.
        /// </summary>
        /// <param name="name">The name of the variable.</param>
        /// <param name="padding">The type of padding applied to the variable.</param>
        public FormatVariableElement(string name, FormatVariablePaddings padding)
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

        public bool Equals(FormatVariableElement other)
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
            else if (other is FormatVariableElement var)
                return Equals(var);
            else
                return false;
        }
#pragma warning restore CS1591
    }
}

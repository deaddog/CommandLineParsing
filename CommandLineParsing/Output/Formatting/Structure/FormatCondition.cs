using System;

namespace CommandLineParsing.Output.Formatting.Structure
{
    /// <summary>
    /// Represents a condition part of a format structure.
    /// </summary>
    public class FormatCondition : FormatElement, IEquatable<FormatCondition>
    {
        /// <summary>
        /// Gets the name of the condition that should be evaluated.
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// Gets a value indicating whether the evaluation of the condition should be negated.
        /// </summary>
        public bool IsNegated { get; }
        /// <summary>
        /// Gets the content that should be inserted if condition evaluation is true (or false when negated).
        /// </summary>
        public FormatElement Content { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FormatCondition"/> class.
        /// </summary>
        /// <param name="name">The name of the condition.</param>
        /// <param name="isNegated"><c>true</c> if the format structure should represent a negated condition.</param>
        /// <param name="content">The content inserted based on condition evaluation.</param>
        public FormatCondition(string name, bool isNegated, FormatElement content)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            IsNegated = isNegated;
            Content = content ?? throw new ArgumentNullException(nameof(content));

            if (Name.Contains(" "))
                throw new ArgumentException("Condition names cannot contain spaces.", nameof(name));
        }

#pragma warning disable CS1591
        public override int GetHashCode()
        {
            return Name.GetHashCode() ^ IsNegated.GetHashCode() ^ Content.GetHashCode();
        }

        public bool Equals(FormatCondition other)
        {
            if (ReferenceEquals(other, null))
                return false;
            else if (ReferenceEquals(other, this))
                return true;
            else
                return Name.Equals(other.Name) && IsNegated.Equals(other.IsNegated) && Content.Equals(other.Content);
        }
        public override bool Equals(FormatElement other)
        {
            if (ReferenceEquals(other, null))
                return false;
            else if (ReferenceEquals(other, this))
                return true;
            else if (other is FormatCondition cond)
                return Equals(cond);
            else
                return false;
        }
#pragma warning restore CS1591
    }
}

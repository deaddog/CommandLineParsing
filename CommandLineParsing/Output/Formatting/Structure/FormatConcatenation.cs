using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace CommandLineParsing.Output.Formatting.Structure
{
    /// <summary>
    /// Represents a sequence of multiple format elements.
    /// </summary>
    public class FormatConcatenation : FormatElement, IEquatable<FormatConcatenation>
    {
        /// <summary>
        /// Gets the sequence of elements.
        /// </summary>
        public ReadOnlyCollection<FormatElement> Elements { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FormatConcatenation"/> class.
        /// </summary>
        /// <param name="elements">A sequence of elements.</param>
        public FormatConcatenation(IEnumerable<FormatElement> elements)
        {
            if (elements == null)
                throw new ArgumentNullException(nameof(elements));

            Elements = new ReadOnlyCollection<FormatElement>(elements.ToList());
        }

#pragma warning disable CS1591
        public override int GetHashCode()
        {
            return Elements.GetHashCode();
        }

        public bool Equals(FormatConcatenation other)
        {
            if (ReferenceEquals(other, null))
                return false;
            else if (ReferenceEquals(other, this))
                return true;
            else
                return Elements.Count.Equals(other.Elements.Count) &&
                    Elements.Zip(other.Elements, (x, y) => x.Equals(y)).All(x => x);
        }
        public override bool Equals(FormatElement other)
        {
            if (ReferenceEquals(other, null))
                return false;
            else if (ReferenceEquals(other, this))
                return true;
            else if (other is FormatConcatenation con)
                return Equals(con);
            else
                return false;
        }
#pragma warning restore CS1591
    }
}

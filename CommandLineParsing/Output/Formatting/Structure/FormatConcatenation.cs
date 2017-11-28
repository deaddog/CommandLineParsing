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

        public static FormatElement operator +(FormatConcatenation element1, FormatConcatenation element2)
        {
            int size = element1.Elements.Count;
            var list = element1.Elements.Concat(element2.Elements).ToList();

            var combined = list[size - 1] + list[size];
            if (!(combined is FormatConcatenation))
            {
                list[size - 1] = combined;
                list.RemoveAt(size);
            }

            if (list.Count == 1)
                return list[0];
            else
                return new FormatConcatenation(list);
        }
        public static FormatElement operator +(FormatElement element1, FormatConcatenation element2)
        {
            if (element1 is FormatConcatenation con)
                return con + element2;
            else
                return new FormatConcatenation(new[] { element1 }) + element2;
        }
        public static FormatElement operator +(FormatConcatenation element1, FormatElement element2)
        {
            if (element2 is FormatConcatenation con)
                return element1 + con;
            else
                return element1 + new FormatConcatenation(new[] { element2 });
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

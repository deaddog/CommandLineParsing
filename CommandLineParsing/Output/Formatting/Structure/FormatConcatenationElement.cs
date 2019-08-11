using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace CommandLineParsing.Output.Formatting.Structure
{
    /// <summary>
    /// Represents a sequence of multiple format elements.
    /// </summary>
    public class FormatConcatenationElement : FormatElement, IEquatable<FormatConcatenationElement>
    {
        /// <summary>
        /// Gets the sequence of elements.
        /// </summary>
        public ReadOnlyCollection<FormatElement> Elements { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FormatConcatenationElement"/> class.
        /// </summary>
        /// <param name="elements">A sequence of elements.</param>
        public FormatConcatenationElement(IEnumerable<FormatElement> elements)
        {
            if (elements == null)
                throw new ArgumentNullException(nameof(elements));

            Elements = new ReadOnlyCollection<FormatElement>(elements.ToList());
        }

        /// <summary>
        /// Merges two <see cref="FormatConcatenationElement"/> elements by attemping to merge the last element in <paramref name="element1"/> and the first in <paramref name="element2"/>.
        /// </summary>
        /// <param name="element1">The first element.</param>
        /// <param name="element2">The second element.</param>
        /// <returns>
        /// The combined format element; possibly a single element (not <see cref="FormatConcatenationElement"/>).
        /// </returns>
        public static FormatElement operator +(FormatConcatenationElement element1, FormatConcatenationElement element2)
        {
            int size = element1.Elements.Count;
            var list = element1.Elements.Concat(element2.Elements).ToList();

            var combined = list[size - 1] + list[size];
            if (!(combined is FormatConcatenationElement))
            {
                list[size - 1] = combined;
                list.RemoveAt(size);
            }

            if (list.Count == 1)
                return list[0];
            else
                return new FormatConcatenationElement(list);
        }
        /// <summary>
        /// Appends a <see cref="FormatConcatenationElement"/> onto a format element.
        /// </summary>
        /// <param name="element1">The first element.</param>
        /// <param name="element2">The second element.</param>
        /// <returns>
        /// The combined format element; possibly a <see cref="FormatConcatenationElement"/>.
        /// </returns>
        public static FormatElement operator +(FormatElement element1, FormatConcatenationElement element2)
        {
            if (element1 is FormatConcatenationElement con)
                return con + element2;
            else
                return new FormatConcatenationElement(new[] { element1 }) + element2;
        }
        /// <summary>
        /// Appends a format element onto a <see cref="FormatConcatenationElement"/>.
        /// </summary>
        /// <param name="element1">The first element.</param>
        /// <param name="element2">The second element.</param>
        /// <returns>
        /// The combined format element; possibly a <see cref="FormatConcatenationElement"/>.
        /// </returns>
        public static FormatElement operator +(FormatConcatenationElement element1, FormatElement element2)
        {
            if (element2 is FormatConcatenationElement con)
                return element1 + con;
            else
                return element1 + new FormatConcatenationElement(new[] { element2 });
        }

#pragma warning disable CS1591
        public override int GetHashCode()
        {
            return Elements.GetHashCode();
        }

        public bool Equals(FormatConcatenationElement other)
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
            else if (other is FormatConcatenationElement con)
                return Equals(con);
            else
                return false;
        }
#pragma warning restore CS1591
    }
}

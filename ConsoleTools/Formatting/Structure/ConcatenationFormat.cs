using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace ConsoleTools.Formatting.Structure
{
    public class ConcatenationFormat : Format
    {
        public ConcatenationFormat(IImmutableList<Format> elements)
        {
            if (elements is null)
                throw new ArgumentNullException(nameof(elements));

            if (elements.Count == 0)
                throw new ArgumentException("Must contain at least one element.", nameof(elements));

            for (int i = 0; i < elements.Count; i++)
            {
                if (elements[i] is null)
                    throw new ArgumentNullException(nameof(elements), "One of the format elements is null.");

                if (elements[i] is ConcatenationFormat c)
                    elements = elements.RemoveAt(i).InsertRange(i, c.Elements);
            }

            Elements = elements;
        }

        public IImmutableList<Format> Elements { get; }

        public override bool Equals(Format? other)
        {
            return other is ConcatenationFormat obj &&
                Elements.Count == obj.Elements.Count &&
                Elements.Zip(obj.Elements, (x, y) => x.Equals(y)).All(x => x);
        }
        public override int GetHashCode()
        {
            HashCode code = default;

            for (int i = 0; i < Elements.Count; i++)
                code.Add(Elements[i]);

            return code.ToHashCode();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;

namespace CommandLineParsing.Output
{
    /// <summary>
    /// Represents a string with embedded color-information.
    /// </summary>
    public partial class ConsoleString
    {
        private readonly Segment[] content;
        private readonly Lazy<int> length;

        /// <summary>
        /// Concatenates the two <see cref="ConsoleString"/>s together. Similar to <c>string + string</c>.
        /// </summary>
        /// <param name="s1">The first string operand.</param>
        /// <param name="s2">The second string operand.</param>
        /// <returns>
        /// A new <see cref="ConsoleString"/> that is the result of concatenating the two strings, preserving color details.
        /// </returns>
        public static ConsoleString operator +(ConsoleString s1, ConsoleString s2)
        {
            if (s1.content.Length == 0)
                return s2;
            else if (s2.content.Length == 0)
                return s1;
            else if (s1.content[s1.content.Length - 1].Color == s2.content[0].Color)
            {
                var l = new List<Segment>();

                l.AddRange(s1.content.Take(s1.content.Length - 1));
                l.Add(new Segment(s1.content[s1.content.Length - 1].Content + s2.content[0].Content, s2.content[0].Color));
                l.AddRange(s2.content.Skip(1));

                return new ConsoleString(l);
            }
            else
                return new ConsoleString(s1.content.Concat(s2.content));
        }

        /// <summary>
        /// Gets a <see cref="ConsoleString"/> that represents the empty string.
        /// </summary>
        public static ConsoleString Empty => new ConsoleString();

        private ConsoleString(IEnumerable<Segment> segments)
        {
            content = segments.ToArray();
            length = new Lazy<int>(() => content.Length == 0 ? 0 : content.Sum(x => x.Content.Length));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleString"/> class, representing an empty string.
        /// </summary>
        public ConsoleString()
            : this(new Segment[0])
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleString"/> class from a string.
        /// Any color-encoding will be parsed by the constructor.
        /// </summary>
        /// <param name="content">The content that should be contained by the <see cref="ConsoleString"/>.
        /// The string "[Color:Text]" will translate to a <see cref="ConsoleString"/> using Color as the foreground color.</param>
        public ConsoleString(string content)
            : this(Segment.Parse(content, true))
        {
        }

        /// <summary>
        /// Gets the number of characters in the <see cref="ConsoleString"/>.
        /// </summary>
        public int Length => length.Value;
    }
}

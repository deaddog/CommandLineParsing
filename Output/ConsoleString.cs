using System;
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
        /// Gets a <see cref="ConsoleString"/> that represents the empty string.
        /// </summary>
        public static ConsoleString Empty => new ConsoleString();

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleString"/> class, representing an empty string.
        /// </summary>
        public ConsoleString()
        {
            content = new Segment[0];
            length = new Lazy<int>(() => content.Length == 0 ? 0 : content.Sum(x => x.Content.Length));
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleString"/> class from a string.
        /// Any color-encoding will be parsed by the constructor.
        /// </summary>
        /// <param name="content">The content that should be contained by the <see cref="ConsoleString"/>.
        /// The string "[Color:Text]" will translate to a <see cref="ConsoleString"/> using Color as the foreground color.</param>
        public ConsoleString(string content)
            : this()
        {
            this.content = Segment.Parse(content, true).ToArray();
        }

        /// <summary>
        /// Gets the number of characters in the <see cref="ConsoleString"/>.
        /// </summary>
        public int Length => length.Value;
    }
}

using System;

namespace CommandLineParsing.Output
{
    /// <summary>
    /// Represents elements of <see cref="ConsoleString"/>, by their text-content and console color.
    /// </summary>
    public class ConsoleStringSegment
    {
        /// <summary>
        /// Gets the text-content of the segment, without color-details.
        /// </summary>
        public string Content { get; }
        /// <summary>
        /// Gets the color associated with the segment. Colors are evaluated using <see cref="ColorTable"/>.
        /// </summary>
        public Color Color { get; }

        /// <summary>
        /// Gets a value indicating whether this segment has a color.
        /// </summary>
        public bool HasColor { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleStringSegment"/> class.
        /// </summary>
        /// <param name="content">The text-content.</param>
        public ConsoleStringSegment(string content) : this(content, Color.NoColor)
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleStringSegment"/> class.
        /// </summary>
        /// <param name="content">The text-content.</param>
        /// <param name="color">The content color.</param>
        public ConsoleStringSegment(string content, Color color)
        {
            Content = content ?? throw new ArgumentNullException(nameof(content));
            Color = color;

            if (string.IsNullOrWhiteSpace(content))
                Color = Color.WithoutForeground();

            HasColor = Color != Color.NoColor;
        }
    }
}

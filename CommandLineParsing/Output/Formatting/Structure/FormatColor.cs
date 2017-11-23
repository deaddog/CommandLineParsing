using System;

namespace CommandLineParsing.Output.Formatting.Structure
{
    /// <summary>
    /// Represents a color tag in a format structure.
    /// </summary>
    public class FormatColor : FormatElement
    {
        /// <summary>
        /// The color name that should be used when a variable (found in <see cref="FormatColor.Content"/>) should determine the final color.
        /// </summary>
        public const string AutoColor = "auto";

        /// <summary>
        /// Gets the color applied to the content. Colors are resolved using <see cref="ColorConsole.Colors"/>.
        /// </summary>
        public string Color { get; }
        /// <summary>
        /// Gets the content to which color should be applied.
        /// </summary>
        public FormatElement Content { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FormatColor"/> class.
        /// </summary>
        /// <param name="color">The color applied to content. Use <see cref="AutoColor"/> to have color applied based on a variable.</param>
        /// <param name="content">The content to which color should be applied.</param>
        public FormatColor(string color, FormatElement content)
        {
            Color = color?.ToLowerInvariant() ?? throw new ArgumentNullException(nameof(color));
            Content = content ?? throw new ArgumentNullException(nameof(content));
        }
    }
}

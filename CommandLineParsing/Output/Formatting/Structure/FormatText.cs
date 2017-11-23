using System;

namespace CommandLineParsing.Output.Formatting.Structure
{
    /// <summary>
    /// Represents a text-only part of a format structure.
    /// </summary>
    public class FormatText : FormatElement
    {
        /// <summary>
        /// Gets the text represented in the format, without any additional structure.
        /// </summary>
        public string Text { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FormatText"/> class.
        /// </summary>
        /// <param name="text">The format text.</param>
        public FormatText(string text)
        {
            Text = text ?? throw new ArgumentNullException(nameof(text));
        }
    }
}

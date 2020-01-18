using System;

namespace CommandLineParsing.Output
{
    /// <summary>
    /// Represents the usage of a specific color in a string format (see <see cref="ConsoleString"/>.
    /// </summary>
    public class Color
    {
        /// <summary>
        /// Gets a <see cref="Color"/> instance that does not represent an actual color.
        /// </summary>
        public static Color NoColor { get; } = new Color();

        /// <summary>
        /// Initializes a new <see cref="Color"/> instance.
        /// </summary>
        /// <param name="name">The name used to describe the desired color.</param>
        public Color(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        /// <summary>
        /// Gets the name used for the color.
        /// </summary>
        public string Name { get; }
    }
}

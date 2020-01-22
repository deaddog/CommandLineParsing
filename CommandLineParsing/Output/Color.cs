using System;

namespace CommandLineParsing.Output
{
    /// <summary>
    /// Represents the usage of a specific color in a string format (see <see cref="ConsoleString"/>.
    /// </summary>
    public struct Color : IEquatable<Color>
    {
        /// <summary>
        /// Gets a <see cref="Color"/> instance that does not represent an actual color.
        /// </summary>
        public static Color NoColor { get; } = new Color();

        /// <summary>
        /// Parses a <see cref="string"/> into a <see cref="Color"/>.
        /// </summary>
        /// <param name="color">The string that should represent the color.</param>
        /// <returns>A <see cref="Color"/> that represents <paramref name="color"/>.</returns>
        public static Color Parse(string color)
        {
            if (string.IsNullOrWhiteSpace(color))
                return NoColor;

            var pipeIndex = color.IndexOf('|');

            if (pipeIndex >= 0)
                return new Color(color.Substring(0, pipeIndex).Trim(), color.Substring(pipeIndex + 1).Trim());
            else
                return new Color(color.Trim());
        }

        /// <summary>
        /// Initializes a new <see cref="Color"/> instance.
        /// </summary>
        /// <param name="foreground">The name used to describe the desired foreground color.</param>
        public Color(string foreground)
        {
            Foreground = foreground ?? throw new ArgumentNullException(nameof(foreground));
            Background = null;
        }

        /// <summary>
        /// Initializes a new <see cref="Color"/> instance.
        /// </summary>
        /// <param name="foreground">The name used to describe the desired foreground color.</param>
        /// <param name="background">The name used to describe the desired background color.</param>
        public Color(string foreground, string background)
        {
            Foreground = foreground ?? throw new ArgumentNullException(nameof(foreground));
            Background = background ?? throw new ArgumentNullException(nameof(background));
        }

        /// <summary>
        /// Constructs a new <see cref="Color"/> with the specified foreground color.
        /// </summary>
        /// <param name="foreground">The name used to describe the desired foreground color.</param>
        /// <returns>A copy of this <see cref="Color"/> with the specified foreground color</returns>
        public Color WithForeground(string foreground)
        {
            return new Color
            (
                foreground: foreground ?? throw new ArgumentNullException(nameof(foreground)),
                background: Background
            );
        }
        /// <summary>
        /// Constructs a new <see cref="Color"/> with the specified background color.
        /// </summary>
        /// <param name="background">The name used to describe the desired background color.</param>
        /// <returns>A copy of this <see cref="Color"/> with the specified background color</returns>
        public Color WithBackground(string background)
        {
            return new Color
            (
                foreground: Foreground,
                background: background ?? throw new ArgumentNullException(nameof(background))
            );
        }

        /// <summary>
        /// Constructs a new <see cref="Color"/> without a foreground color.
        /// </summary>
        /// <returns>A copy of this <see cref="Color"/> without a foreground color</returns>
        public Color WithoutForeground()
        {
            return new Color
            (
                foreground: null,
                background: Background
            );
        }
        /// <summary>
        /// Constructs a new <see cref="Color"/> without a background color.
        /// </summary>
        /// <returns>A copy of this <see cref="Color"/> without a background color</returns>
        public Color WithoutBackground()
        {
            return new Color
            (
                foreground: Foreground,
                background: null
            );
        }

        /// <summary>
        /// Determines if the two colors are equal.
        /// </summary>
        /// <param name="c1">The first color to compare.</param>
        /// <param name="c2">The second color to compare.</param>
        /// <returns>
        /// <c>true</c> if the two colors are equal; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator ==(Color c1, Color c2)
        {
            return c1.Equals(c2);
        }
        /// <summary>
        /// Determines if the two colors are not equal.
        /// </summary>
        /// <param name="c1">The first color to compare.</param>
        /// <param name="c2">The second color to compare.</param>
        /// <returns>
        /// <c>true</c> if the two colors are not equal; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator !=(Color c1, Color c2)
        {
            return !(c1 == c2);
        }
        /// <summary>
        /// Returns a hash code for this <see cref="Color"/>.
        /// </summary>
        /// <returns>
        /// A hash code for this <see cref="Color"/>, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            return (Foreground?.GetHashCode() ?? 0) ^ (Background?.GetHashCode() ?? 0);
        }

        /// <summary>
        /// Determines whether the specified <see cref="object"/>, is equal to this <see cref="Color"/>.
        /// </summary>
        /// <param name="obj">The <see cref="object"/> to compare with this <see cref="Color"/>.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="object" /> is equal to this <see cref="Color"/>; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            return obj is Color color && Equals(color);
        }
        /// <summary>
        /// Determines whether the specified <see cref="Color"/>, is equal to this <see cref="Color"/>.
        /// </summary>
        /// <param name="other">The <see cref="Color"/> to compare with this <see cref="Color"/>.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="Color" /> is equal to this <see cref="Color"/>; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(Color other)
        {
            return StringComparer.OrdinalIgnoreCase.Equals(Foreground, other.Foreground) && StringComparer.OrdinalIgnoreCase.Equals(Background, other.Background);
        }

        /// <summary>
        /// Gets the name used for the foreground color.
        /// </summary>
        public string Foreground { get; }
        /// <summary>
        /// Gets the name used for the background color.
        /// </summary>
        public string Background { get; }

        /// <summary>
        /// Gets a boolean indicating if this <see cref="Color"/> has a foreground color.
        /// </summary>
        public bool HasForeground => !(Foreground is null);
        /// <summary>
        /// Gets a boolean indicating if this <see cref="Color"/> has a background color.
        /// </summary>
        public bool HasBackground => !(Background is null);

        /// <summary>
        /// Returns a <see cref="string" /> that represents this <see cref="Color"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            if (Background is null)
                return Foreground;
            else
                return $"{Foreground}|{Background}";
        }
    }
}

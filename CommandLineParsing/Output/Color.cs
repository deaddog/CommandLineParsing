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

            return new Color(color.Trim());
        }

        /// <summary>
        /// Initializes a new <see cref="Color"/> instance.
        /// </summary>
        /// <param name="name">The name used to describe the desired color.</param>
        public Color(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
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
            return Name?.GetHashCode() ?? 0;
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
            return StringComparer.OrdinalIgnoreCase.Equals(Name, other.Name);
        }

        /// <summary>
        /// Gets the name used for the color.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this <see cref="Color"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return Name;
        }
    }
}

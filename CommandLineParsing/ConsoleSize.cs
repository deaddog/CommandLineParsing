using System;

namespace CommandLineParsing
{
    /// <summary>
    /// Represents a size (width, height) in the <see cref="Console"/>.
    /// This type can be used with the <see cref="ColorConsole"/>.
    /// </summary>
    public struct ConsoleSize : IEquatable<ConsoleSize>
    {
        private int width;
        private int height;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleSize"/> struct.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public ConsoleSize(int width, int height)
        {
            this.width = width;
            this.height = height;
        }

        /// <summary>
        /// Sums two size into one.
        /// </summary>
        /// <param name="s1">The first <see cref="ConsoleSize"/>.</param>
        /// <param name="s2">The second <see cref="ConsoleSize"/>.</param>
        /// <returns>
        /// The new <see cref="ConsoleSize"/>, which is the sum of the two.
        /// </returns>
        public static ConsoleSize operator +(ConsoleSize s1, ConsoleSize s2) => new ConsoleSize(s1.width + s2.width, s1.height + s2.height);
        /// <summary>
        /// Subtracts one sum from the other.
        /// </summary>
        /// <param name="s1">The first <see cref="ConsoleSize"/>.</param>
        /// <param name="s2">The second <see cref="ConsoleSize"/>.</param>
        /// <returns>
        /// The new <see cref="ConsoleSize"/>, which is the result of subtracting <paramref name="s2"/>'s size from <paramref name="s1"/>.
        /// </returns>
        public static ConsoleSize operator -(ConsoleSize s1, ConsoleSize s2) => new ConsoleSize(s1.width - s2.width, s1.height - s2.height);

        /// <summary>
        /// Determines if the two sizes are equal.
        /// </summary>
        /// <param name="s1">The first size to compare.</param>
        /// <param name="s2">The second size to compare.</param>
        /// <returns>
        /// <c>true</c> if the two sizes are equal; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator ==(ConsoleSize s1, ConsoleSize s2) => s1.Equals(s2);
        /// <summary>
        /// Determines if the two sizes are not equal.
        /// </summary>
        /// <param name="s1">The first size to compare.</param>
        /// <param name="s2">The second size to compare.</param>
        /// <returns>
        /// <c>true</c> if the two sizes are not equal; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator !=(ConsoleSize s1, ConsoleSize s2) => !s1.Equals(s2);
        /// <summary>
        /// Returns a hash code for this <see cref="ConsoleSize"/>.
        /// </summary>
        /// <returns>
        /// A hash code for this <see cref="ConsoleSize"/>, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            return width.GetHashCode() ^ height.GetHashCode();
        }

        /// <summary>
        /// Determines whether the specified <see cref="object"/>, is equal to this <see cref="ConsoleSize"/>.
        /// </summary>
        /// <param name="obj">The <see cref="object"/> to compare with this <see cref="ConsoleSize"/>.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="object" /> is equal to this <see cref="ConsoleSize"/>; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null))
                return false;
            else if (obj is ConsoleSize)
                return Equals((ConsoleSize)obj);
            else
                return false;
        }
        /// <summary>
        /// Determines whether the specified <see cref="ConsoleSize"/>, is equal to this <see cref="ConsoleSize"/>.
        /// </summary>
        /// <param name="other">The <see cref="ConsoleSize"/> to compare with this <see cref="ConsoleSize"/>.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="ConsoleSize" /> is equal to this <see cref="ConsoleSize"/>; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(ConsoleSize other)
        {
            return width == other.width && height == other.height;
        }

        /// <summary>
        /// Gets or sets the width of this <see cref="ConsoleSize"/>.
        /// </summary>
        public int Width
        {
            get { return width; }
            set { width = value; }
        }
        /// <summary>
        /// Gets or sets the height of this <see cref="ConsoleSize"/>.
        /// </summary>
        public int Height
        {
            get { return height; }
            set { height = value; }
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this <see cref="ConsolePoint"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString() => $"{{Width: {width}, Height: {height}}}";
    }
}

using System;

namespace CommandLineParsing
{
    /// <summary>
    /// Represents a point (left/column, top/row) in the <see cref="Console"/>.
    /// This type can be used with the <see cref="ColorConsole"/>.
    /// </summary>
    public struct ConsolePoint : IEquatable<ConsolePoint>
    {
        private int left;
        private int top;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsolePoint"/> struct.
        /// </summary>
        /// <param name="left">The column in the console.</param>
        /// <param name="top">The row in the console.</param>
        public ConsolePoint(int left, int top)
        {
            this.left = left;
            this.top = top;
        }

        /// <summary>
        /// Adds a size to a point effectively "moving" the point.
        /// </summary>
        /// <param name="p">The <see cref="ConsolePoint"/> that should be moved.</param>
        /// <param name="s">The <see cref="ConsoleSize"/> that the point should be moved.</param>
        /// <returns>
        /// The resulting, "moved", <see cref="ConsolePoint"/>.
        /// </returns>
        public static ConsolePoint operator +(ConsolePoint p, ConsoleSize s) => new ConsolePoint(p.left + s.Width, p.top + s.Height);
        /// <summary>
        /// Sutracts a size to a point effectively "moving" the point.
        /// </summary>
        /// <param name="p">The <see cref="ConsolePoint"/> that should be moved.</param>
        /// <param name="s">The <see cref="ConsoleSize"/> that the point should be moved (negatively).</param>
        /// <returns>
        /// The resulting, "moved", <see cref="ConsolePoint"/>.
        /// </returns>
        public static ConsolePoint operator -(ConsolePoint p, ConsoleSize s) => new ConsolePoint(p.left - s.Width, p.top - s.Height);

        /// <summary>
        /// Determines if the two points are equal.
        /// </summary>
        /// <param name="p1">The first point to compare.</param>
        /// <param name="p2">The second point to compare.</param>
        /// <returns>
        /// <c>true</c> if the two points are equal; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator ==(ConsolePoint p1, ConsolePoint p2) => p1.Equals(p2);
        /// <summary>
        /// Determines if the two points are not equal.
        /// </summary>
        /// <param name="p1">The first point to compare.</param>
        /// <param name="p2">The second point to compare.</param>
        /// <returns>
        /// <c>true</c> if the two points are not equal; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator !=(ConsolePoint p1, ConsolePoint p2) => !p1.Equals(p2);
        /// <summary>
        /// Returns a hash code for this <see cref="ConsolePoint"/>.
        /// </summary>
        /// <returns>
        /// A hash code for this <see cref="ConsolePoint"/>, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            return left.GetHashCode() ^ top.GetHashCode();
        }

        /// <summary>
        /// Determines whether the specified <see cref="object"/>, is equal to this <see cref="ConsolePoint"/>.
        /// </summary>
        /// <param name="obj">The <see cref="object"/> to compare with this <see cref="ConsolePoint"/>.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="object" /> is equal to this <see cref="ConsolePoint"/>; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null))
                return false;
            else if (obj is ConsolePoint)
                return Equals((ConsolePoint)obj);
            else
                return false;
        }
        /// <summary>
        /// Determines whether the specified <see cref="ConsolePoint"/>, is equal to this <see cref="ConsolePoint"/>.
        /// </summary>
        /// <param name="other">The <see cref="ConsolePoint"/> to compare with this <see cref="ConsolePoint"/>.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="ConsolePoint" /> is equal to this <see cref="ConsolePoint"/>; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(ConsolePoint other)
        {
            return left == other.left && top == other.top;
        }

        /// <summary>
        /// Gets or sets the column position of this <see cref="ConsolePoint"/>.
        /// </summary>
        public int Left
        {
            get { return left; }
            set { left = value; }
        }
        /// <summary>
        /// Gets or sets the row position of this <see cref="ConsolePoint"/>.
        /// </summary>
        public int Top
        {
            get { return top; }
            set { top = value; }
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this <see cref="ConsolePoint"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString() => $"{{Left: {left}, Top: {top}}}";
    }
}

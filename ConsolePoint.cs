using System;

namespace CommandLineParsing
{
    /// <summary>
    /// Represents a point (left/column, top/row) in the <see cref="Console"/>.
    /// This type can be used with the <see cref="ColorConsole"/>.
    /// </summary>
    public struct ConsolePoint
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

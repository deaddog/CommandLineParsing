using System;

namespace CommandLineParsing
{
    /// <summary>
    /// Represents a size (width, height) in the <see cref="Console"/>.
    /// This type can be used with the <see cref="ColorConsole"/>.
    /// </summary>
    public struct ConsoleSize
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

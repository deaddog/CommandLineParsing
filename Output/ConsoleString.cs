namespace CommandLineParsing.Output
{
    /// <summary>
    /// Represents a string with embedded color-information.
    /// </summary>
    public partial class ConsoleString
    {
        private readonly Segment[] content;

        /// <summary>
        /// Gets a <see cref="ConsoleString"/> that represents the empty string.
        /// </summary>
        public static ConsoleString Empty => new ConsoleString();

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleString"/> class, representing an empty string.
        /// </summary>
        public ConsoleString()
        {
            content = new Segment[0];
        }
    }
}

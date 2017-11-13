using System;

namespace CommandLineParsing.Parsing
{
    /// <summary>
    /// The exception that is thrown when an attempt to parse fails.
    /// Note that this exception is never thrown when a parsing method is not defined. See instead <see cref="MissingParserException"/>.
    /// This exception should not be thrown when a method returns <see cref="Message"/>.
    /// </summary>
    public class ParsingFailedException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MissingParserException" /> class.
        /// </summary>
        /// <param name="parsedType">The type to which a parse was attempted.</param>
        /// <param name="message">The error message from the failed parse attempt.</param>
        public ParsingFailedException(Type parsedType, string message) : base(message)
        {
            ParsedType = parsedType ?? throw new ArgumentNullException(nameof(parsedType));
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="ParsingFailedException"/> class.
        /// </summary>
        /// <param name="parsedType">The type to which a parse was attempted.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
        /// <exception cref="ArgumentNullException">parsedType</exception>
        public ParsingFailedException(Type parsedType, Exception innerException) : base($"An exception occured while parsing {parsedType.Name}.", innerException)
        {
            ParsedType = parsedType ?? throw new ArgumentNullException(nameof(parsedType));
        }

        /// <summary>
        /// Gets the type to which a parse was attempted.
        /// </summary>
        public Type ParsedType { get; }
    }
}

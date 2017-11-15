using System;

namespace CommandLineParsing.Parsing
{
    /// <summary>
    /// The exception that is thrown when an attempt is made to parse a type without a static TryParse method.
    /// </summary>
    public class MissingParserException : Exception
    {
        private static string GetMessage(Type parsedType)
        {
            return $"The type { parsedType.Name } is not supported. A {nameof(TryParse<string>)} or {nameof(MessageTryParse<string>)} method must be defined in {parsedType.Name}. " +
                $"Alternatively a custom parser can be defined for the specific context.";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MissingParserException" /> class.
        /// </summary>
        /// <param name="parsedType">The type to which a parse was attempted.</param>
        public MissingParserException(Type parsedType) : base(GetMessage(parsedType))
        {
            ParsedType = parsedType;
        }

        /// <summary>
        /// Gets the type to which a parse was attempted.
        /// </summary>
        public Type ParsedType { get; }
    }
}

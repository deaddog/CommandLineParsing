using System;

namespace ConsoleTools.Parsing
{
    public class MissingParserException : Exception
    {
        public MissingParserException(Type valueType, string message) : base(message)
        {
            ValueType = valueType ?? throw new ArgumentNullException(nameof(valueType));
        }
        public MissingParserException(Type valueType) : this(valueType, $"No valid parser definition for type '{valueType.Name}'. Consider providing your own.")
        {
        }

        public Type ValueType { get; }
    }
}

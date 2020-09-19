using System;

namespace CommandLineParsing.Parsing
{
    public class ReflectedParserSettings
    {
        public ReflectedParserSettings(bool enumIgnoreCase, Message noValueMessage, Message multipleValuesMessage, Func<string, Message> typeErrorMessage, bool useParserMessage)
        {
            EnumIgnoreCase = enumIgnoreCase;
            NoValueMessage = noValueMessage ?? throw new ArgumentNullException(nameof(noValueMessage));
            MultipleValuesMessage = multipleValuesMessage ?? throw new ArgumentNullException(nameof(multipleValuesMessage));
            TypeErrorMessage = typeErrorMessage ?? throw new ArgumentNullException(nameof(typeErrorMessage));
            UseParserMessage = useParserMessage;
        }

        public bool EnumIgnoreCase { get; }
        public Message NoValueMessage { get; }
        public Message MultipleValuesMessage { get; }
        public Func<string, Message> TypeErrorMessage { get; }
        public bool UseParserMessage { get; }
    }
}

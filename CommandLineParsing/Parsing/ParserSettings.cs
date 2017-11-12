using System;

namespace CommandLineParsing.Parsing
{
    internal class ParserSettings
    {
        public bool EnumIgnoreCase { get; set; }
        public Message NoValueMessage { get; set; }
        public Message MultipleValuesMessage { get; set; }
        public Func<string, Message> TypeErrorMessage { get; set; }
        public bool UseParserMessage { get; set; }
    }
}

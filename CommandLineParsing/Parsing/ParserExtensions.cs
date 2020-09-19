namespace CommandLineParsing.Parsing
{
    public static class ParserExtensions
    {
        public static Message<T> Parse<T>(this IParser<T> parser, string input)
        {
            return parser.Parse(new string[] { input });
        }
    }
}

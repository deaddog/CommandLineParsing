namespace ConsoleTools.Parsing
{
    internal class MissingParser<T> : IParser<T>
    {
        public Message<T> Parse(string arg)
        {
            throw new MissingParserException(typeof(T));
        }
    }
}

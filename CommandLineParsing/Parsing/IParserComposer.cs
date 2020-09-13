namespace CommandLineParsing.Parsing
{
    public interface IParserComposer<T, out TReturn>
    {
        IParser<T> Parser { get; }
        TReturn WithParser(IParser<T> parser);
    }
}

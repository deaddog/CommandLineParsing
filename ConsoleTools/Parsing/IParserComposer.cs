namespace ConsoleTools.Parsing
{
    public interface IParserComposer<T, out TComposer>
    {
        IParser<T> Parser { get; }
        TComposer WithParser(IParser<T> parser);
    }
}

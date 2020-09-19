namespace CommandLineParsing.Parsing
{
    public interface IParser<T>
    {
        Message<T> Parse(string[] args);
    }
}

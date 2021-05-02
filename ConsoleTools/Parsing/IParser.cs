namespace ConsoleTools.Parsing
{
    public interface IParser<T>
    {
        Message<T> Parse(string arg);
    }
}

namespace ConsoleTools.Parsing
{
    public class StringParser : IParser<string>
    {
        public Message<string> Parse(string arg)
        {
            return new Message<string>(arg);
        }
    }
}

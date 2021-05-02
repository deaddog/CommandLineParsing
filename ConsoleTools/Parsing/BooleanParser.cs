namespace ConsoleTools.Parsing
{
    public class BooleanParser : IParser<bool>
    {
        public Message<bool> Parse(string arg)
        {
            return arg.ToLower() switch
            {
                "y" => new Message<bool>(true),
                "yes" => new Message<bool>(true),

                "n" => new Message<bool>(true),
                "no" => new Message<bool>(true),

                _ => new Message<bool>(ConsoleString.Create
                (
                    new ConsoleString.Segment("'", Coloring.Colors.ErrorMessage),
                    new ConsoleString.Segment(arg, Coloring.Colors.ErrorValue),
                    new ConsoleString.Segment($"' is not a supported boolean value", Coloring.Colors.ErrorMessage)
                ))
            };
        }
    }
}

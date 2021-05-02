namespace ConsoleTools.Parsing
{
    public class TryParser<T> : IParser<T>
    {
        public delegate bool TryParse(string arg, out T value);

        private readonly TryParser<T>.TryParse _tryParse;
        private readonly string _aTypeName;

        public TryParser(TryParse tryParse, string aTypeName)
        {
            _tryParse = tryParse ?? throw new System.ArgumentNullException(nameof(tryParse));
            _aTypeName = aTypeName ?? throw new System.ArgumentNullException(nameof(aTypeName));
        }

        public Message<T> Parse(string arg)
        {
            if (_tryParse(arg, out var value))
                return new Message<T>(value);
            else
            {
                return new Message<T>(ConsoleString.Create
                (
                    new ConsoleString.Segment("'", Coloring.Colors.ErrorMessage),
                    new ConsoleString.Segment(arg, Coloring.Colors.ErrorValue),
                    new ConsoleString.Segment($"' is not {_aTypeName}", Coloring.Colors.ErrorMessage)
                ));
            }
        }
    }
}

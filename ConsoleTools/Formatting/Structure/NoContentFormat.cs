namespace ConsoleTools.Formatting.Structure
{
    public class NoContentFormat : Format
    {
        public static NoContentFormat Element { get; } = new NoContentFormat();

        private NoContentFormat()
        {
        }

        public override bool Equals(Format? other)
        {
            return other is NoContentFormat;
        }
        public override int GetHashCode()
        {
            return 1;
        }
    }
}

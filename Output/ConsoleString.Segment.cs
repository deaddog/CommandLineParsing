namespace CommandLineParsing.Output
{
    public partial class ConsoleString
    {
        private class Segment
        {
            public readonly string Content;
            public readonly string Color;

            public Segment(string content, string color)
            {
                Content = content;
                Color = color;
            }
        }
    }
}

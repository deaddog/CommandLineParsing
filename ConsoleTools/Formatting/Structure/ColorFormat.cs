using System;

namespace ConsoleTools.Formatting.Structure
{
    public class ColorFormat : Format
    {
        public ColorFormat(string color, Format content)
        {
            Color = color?.ToLowerInvariant() ?? throw new ArgumentNullException(nameof(color));
            Content = content ?? throw new ArgumentNullException(nameof(content));
        }

        public string Color { get; }
        public Format Content { get; }

        public override bool Equals(Format? other)
        {
            return other is ColorFormat obj &&
                Color.Equals(obj.Color) &&
                Content.Equals(obj.Content);
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(Color, Content);
        }
    }
}

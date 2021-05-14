using System;

namespace ConsoleTools.Formatting.Structure
{
    public class TextFormat : Format
    {
        public TextFormat(string text)
        {
            Text = text ?? throw new ArgumentNullException(nameof(text));
        }

        public string Text { get; }

        public override bool Equals(Format? other)
        {
            return other is TextFormat obj &&
                Text.Equals(obj.Text);
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(Text);
        }
    }
}

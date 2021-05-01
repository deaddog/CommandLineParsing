using System;
using System.Collections.Generic;

namespace ConsoleTools
{
    public partial class ConsoleString
    {
        public class Segment : IEquatable<Segment?>
        {
            public Segment(string content, Color color)
            {
                Content = content ?? throw new ArgumentNullException(nameof(content));
                Color = color;

                if (string.IsNullOrWhiteSpace(content))
                    Color = Color.WithoutForeground();

                HasColor = Color != Color.NoColor;
            }

            public static explicit operator Segment(string content) => new Segment(content, Color.NoColor);

            public string Content { get; }
            public Color Color { get; }

            public bool HasColor { get; }

            public static IEnumerable<Segment> Parse(string value) => Parse(value, Color.NoColor);
            private static IEnumerable<Segment> Parse(string value, Color currentColor)
            {
                if (string.IsNullOrEmpty(value))
                    yield break;

                int index = 0;

                while (index < value.Length)
                    switch (value[index])
                    {
                        case '[': // Coloring
                        {
                            int end = Utils.StringOperations.FindEnd(value, index, '[', ']');
                            var block = value.Substring(index + 1, end - index - 1);
                            int colon = block.IndexOf(':');
                            if (colon > 0 && block[colon - 1] == '\\')
                                colon = -1;

                            if (colon == -1)
                                yield return new Segment($"[{block}]", currentColor);
                            else
                            {
                                var color = Color.Parse(block.Substring(0, colon));
                                string content = block.Substring(colon + 1);

                                foreach (var p in Parse(content, color))
                                    yield return p;
                            }
                            index += block.Length + 2;
                        }
                        break;

                        case '\\':
                            if (value.Length == index + 1)
                                index++;
                            else
                            {
                                yield return new Segment(value[index + 1].ToString(), currentColor);
                                index += 2;
                            }
                            break;

                        default: // Skip content
                            int nIndex = value.IndexOfAny(new char[] { '[', '\\' }, index);
                            if (nIndex < 0) nIndex = value.Length;
                            yield return new Segment(value.Substring(index, nIndex - index), currentColor);
                            index = nIndex;
                            break;
                    }
            }

            public override bool Equals(object? obj)
            {
                return Equals(obj as Segment);
            }
            public bool Equals(Segment? other)
            {
                return other != null &&
                       Content == other.Content &&
                       Color.Equals(other.Color) &&
                       HasColor == other.HasColor;
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(Content, Color, HasColor);
            }
        }
    }
}

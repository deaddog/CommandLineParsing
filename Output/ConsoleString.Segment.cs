using System.Collections.Generic;

namespace CommandLineParsing.Output
{
    public partial class ConsoleString
    {
        private class Segment
        {
            public readonly string Content;
            public readonly string Color;

            public readonly bool HasColor;

            public Segment(string content, string color)
            {
                Content = content;
                Color = color;
                HasColor = color != null;
            }

            public static IEnumerable<Segment> Parse(string value, bool maintainEscape)
            {
                var segments = parse(value, maintainEscape, null);

                var e = segments.GetEnumerator();
                if (!e.MoveNext())
                    yield break;

                var temp = e.Current;

                while (e.MoveNext())
                    if (temp.Color == e.Current.Color)
                        temp = new Segment(temp.Content + e.Current.Content, temp.Color);
                    else
                    {
                        yield return temp;
                        temp = e.Current;
                    }

                yield return temp;
            }
            private static IEnumerable<Segment> parse(string value, bool maintainEscape, string currentColor)
            {
                if (string.IsNullOrEmpty(value))
                    yield break;

                int index = 0;

                while (index < value.Length)
                    switch (value[index])
                    {
                        case '[': // Coloring
                            {
                                int end = findEnd(value, index, '[', ']');
                                var block = value.Substring(index + 1, end - index - 1);
                                int colon = block.IndexOf(':');
                                if (colon > 0 && block[colon - 1] == '\\')
                                    colon = -1;

                                if (colon == -1)
                                    yield return new Segment($"[{block}]", currentColor);
                                else
                                {
                                    var color = block.Substring(0, colon);
                                    string content = block.Substring(colon + 1);

                                    foreach (var p in parse(content, maintainEscape, color))
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
                                if (maintainEscape)
                                    yield return new Segment(value.Substring(index, 2), currentColor);
                                else
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
            
            private static int findEnd(string text, int index, char open, char close)
            {
                int count = 0;
                do
                {
                    if (text[index] == '\\') { index += 2; continue; }
                    if (text[index] == open) count++;
                    else if (text[index] == close) count--;
                    index++;
                } while (count > 0 && index < text.Length);
                if (count == 0) index--;

                return index;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;

namespace CommandLineParsing.Output
{
    /// <summary>
    /// Represents a string with embedded color-information.
    /// A string is implicitly converted into a <see cref="ConsoleString"/>.
    /// </summary>
    public partial class ConsoleString : IEnumerable<ConsoleStringSegment>
    {
        private readonly ConsoleStringSegment[] _content;
        private readonly Lazy<string> _text;
        private readonly Lazy<bool> _hasColors;

        /// <summary>
        /// Performs an implicit conversion from <see cref="System.String"/> to <see cref="ConsoleString"/>.
        /// With this a format can be specified simply as a string when used as a parameter.
        /// </summary>
        /// <param name="text">The text that is parsed to a <see cref="ConsoleString"/>. See <see cref="ConsoleString.Parse(string, bool)"/>.</param>
        /// <returns>
        /// A new <see cref="ConsoleString"/> that represents the format found in <paramref name="text"/>.
        /// </returns>
        public static implicit operator ConsoleString(string text)
        {
            return Parse(text, false);
        }

        /// <summary>
        /// Concatenates the two <see cref="ConsoleString"/>s. Similar to <c>string + string</c>.
        /// </summary>
        /// <param name="s1">The first string operand.</param>
        /// <param name="s2">The second string operand.</param>
        /// <returns>
        /// A new <see cref="ConsoleString"/> that is the result of concatenating the two strings, preserving color details from both.
        /// </returns>
        public static ConsoleString operator +(ConsoleString s1, ConsoleString s2)
        {
            if (s1._content.Length == 0)
                return s2;
            else if (s2._content.Length == 0)
                return s1;
            else if (s1._content[s1._content.Length - 1].Color == s2._content[0].Color)
            {
                var l = new List<ConsoleStringSegment>();

                l.AddRange(s1._content.Take(s1._content.Length - 1));
                l.Add(new ConsoleStringSegment(s1._content[s1._content.Length - 1].Content + s2._content[0].Content, s2._content[0].Color));
                l.AddRange(s2._content.Skip(1));

                return new ConsoleString(l);
            }
            else
                return new ConsoleString(s1._content.Concat(s2._content));
        }
        /// <summary>
        /// Concatenates a string and a <see cref="ConsoleString"/>. Similar to <c>string + string</c>.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="str">The <see cref="ConsoleString"/>.</param>
        /// <returns>
        /// A new <see cref="ConsoleString"/> that is the result of concatenating the two strings, preserving color details from both.
        /// </returns>
        public static ConsoleString operator +(string text, ConsoleString str)
        {
            return (ConsoleString)text + str;
        }
        /// <summary>
        /// Concatenates a <see cref="ConsoleString"/> and a string. Similar to <c>string + string</c>.
        /// </summary>
        /// <param name="str">The <see cref="ConsoleString"/>.</param>
        /// <param name="text">The text.</param>
        /// <returns>
        /// A new <see cref="ConsoleString"/> that is the result of concatenating the two strings, preserving color details from both.
        /// </returns>
        public static ConsoleString operator +(ConsoleString str, string text)
        {
            return str + (ConsoleString)text;
        }

        /// <summary>
        /// Determines if two <see cref="ConsoleString"/> are the same value, including color information.
        /// </summary>
        /// <param name="s1">The first <see cref="ConsoleString"/> to compare.</param>
        /// <param name="s2">The second <see cref="ConsoleString"/> to compare.</param>
        /// <returns>
        /// <c>true</c> if the value and colors of <paramref name="s1"/> are the same as the value and colors of <paramref name="s2"/>; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator ==(ConsoleString s1, ConsoleString s2)
        {
            if (ReferenceEquals(s1, s2))
                return true;
            else if (ReferenceEquals(s1, null))
                return false;
            else if (ReferenceEquals(s2, null))
                return false;
            else
                return s1.Equals(s2);
        }
        /// <summary>
        /// Determines if two <see cref="ConsoleString"/> are not the same value, including color information.
        /// </summary>
        /// <param name="s1">The first <see cref="ConsoleString"/> to compare.</param>
        /// <param name="s2">The second <see cref="ConsoleString"/> to compare.</param>
        /// <returns>
        /// <c>true</c> if the value and colors of <paramref name="s1"/> are not the same as the value and colors of <paramref name="s2"/>; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator !=(ConsoleString s1, ConsoleString s2)
        {
            return !(s1 == s2);
        }

        /// <summary>
        /// Gets a <see cref="ConsoleString"/> that represents the empty string.
        /// </summary>
        public static ConsoleString Empty => new ConsoleString();

        /// <summary>
        /// Parses a string using the color-encoding syntax.
        /// </summary>
        /// <param name="content">
        /// The content that should be contained by the <see cref="ConsoleString"/>.
        /// The string "[Color:Text]" will translate to a <see cref="ConsoleString"/> using Color as the foreground color.
        /// Color translations are handled by <see cref="ColorTable"/>.
        /// </param>
        /// <param name="maintainEscape">
        /// Determines if escaped characters should remain escaped when parsing <paramref name="content"/>.
        /// If <c>true</c> any escaped character (such as "\[") will remain in its escaped state, otherwise it will be converted into the unescaped version (such as "[").
        /// </param>
        /// <returns>A <see cref="ConsoleString"/> representing the result of the parsed string.</returns>
        public static ConsoleString Parse(string content, bool maintainEscape = false)
        {
            var segments = ParseToSegments(content, maintainEscape, null);
            segments = MergeSameColor(segments);

            return new ConsoleString(segments);
        }
        private static IEnumerable<ConsoleStringSegment> MergeSameColor(IEnumerable<ConsoleStringSegment> segments)
        {
            var e = segments.Where(x => x.Content.Length > 0).GetEnumerator();
            if (!e.MoveNext())
                yield break;

            var temp = e.Current;

            while (e.MoveNext())
                if (temp.Color == e.Current.Color)
                    temp = new ConsoleStringSegment(temp.Content + e.Current.Content, temp.Color);
                else
                {
                    yield return temp;
                    temp = e.Current;
                }

            yield return temp;
        }
        private static IEnumerable<ConsoleStringSegment> ParseToSegments(string value, bool maintainEscape, string currentColor)
        {
            if (string.IsNullOrEmpty(value))
                yield break;

            int index = 0;

            while (index < value.Length)
                switch (value[index])
                {
                    case '[': // Coloring
                        {
                            int end = FindEnd(value, index, '[', ']');
                            var block = value.Substring(index + 1, end - index - 1);
                            int colon = block.IndexOf(':');
                            if (colon > 0 && block[colon - 1] == '\\')
                                colon = -1;

                            if (colon == -1)
                                yield return new ConsoleStringSegment($"[{block}]", currentColor);
                            else
                            {
                                var color = block.Substring(0, colon);
                                string content = block.Substring(colon + 1);

                                foreach (var p in ParseToSegments(content, maintainEscape, color))
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
                                yield return new ConsoleStringSegment(value.Substring(index, 2), currentColor);
                            else
                                yield return new ConsoleStringSegment(value[index + 1].ToString(), currentColor);

                            index += 2;
                        }
                        break;

                    default: // Skip content
                        int nIndex = value.IndexOfAny(new char[] { '[', '\\' }, index);
                        if (nIndex < 0) nIndex = value.Length;
                        yield return new ConsoleStringSegment(value.Substring(index, nIndex - index), currentColor);
                        index = nIndex;
                        break;
                }
        }
        private static int FindEnd(string text, int index, char open, char close)
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

        /// <summary>
        /// Creates a new <see cref="ConsoleString"/> from a single segment.
        /// </summary>
        /// <param name="segment">The string segment.</param>
        /// <returns>A new <see cref="ConsoleString"/> defined by the single element <paramref name="segment"/>.</returns>
        public static ConsoleString Create(ConsoleStringSegment segment)
        {
            return new ConsoleString(new[] { segment });
        }
        /// <summary>
        /// Creates a new <see cref="ConsoleString"/> with direct content.
        /// </summary>
        /// <param name="content">The content of the <see cref="ConsoleString"/>. No parsing is applied.</param>
        /// <returns>A new <see cref="ConsoleString"/> defined by <paramref name="content"/>.</returns>
        public static ConsoleString FromContent(string content)
        {
            return Create(new ConsoleStringSegment(content));
        }
        /// <summary>
        /// Creates a new <see cref="ConsoleString"/> with direct content and color.
        /// </summary>
        /// <param name="content">The content of the <see cref="ConsoleString"/>. No parsing is applied.</param>
        /// <param name="color">The color that should be applied to <paramref name="content"/>.</param>
        /// <returns>A new <see cref="ConsoleString"/> defined by <paramref name="content"/> with color <paramref name="color"/>.</returns>
        public static ConsoleString FromContent(string content, string color)
        {
            return Create(new ConsoleStringSegment(content, color));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleString"/> class.
        /// </summary>
        /// <param name="segments">The segments that should make up the console string.</param>
        public ConsoleString(IEnumerable<ConsoleStringSegment> segments)
        {
            _content = MergeSameColor(segments).ToArray();
            _text = new Lazy<string>(() => string.Concat(_content.Select(x => x.Content)));
            _hasColors = new Lazy<bool>(() => _content.Any(x => x.HasColor));
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleString"/> class, representing an empty string.
        /// </summary>
        public ConsoleString()
            : this(new ConsoleStringSegment[0])
        {
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _content.GetEnumerator();
        }
        IEnumerator<ConsoleStringSegment> IEnumerable<ConsoleStringSegment>.GetEnumerator()
        {
            foreach (var s in _content)
                yield return s;
        }

        /// <summary>
        /// Returns a hash code for this <see cref="ConsoleString"/>.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            return _text.Value.GetHashCode();
        }
        /// <summary>
        /// Determines whether the specified <see cref="object"/>, is equal to this <see cref="ConsoleString"/>.
        /// </summary>
        /// <param name="obj">The <see cref="object" /> to compare with this <see cref="ConsoleString"/>.</param>
        /// <returns><c>true</c> if the specified <see cref="object" /> is equal to this <see cref="ConsoleString"/>; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null))
                return false;
            else if (obj is ConsoleString)
                return Equals(obj as ConsoleString);
            else
                return false;
        }
        /// <summary>
        /// Determines whether the specified <see cref="ConsoleString" />, is equal to this <see cref="ConsoleString"/>.
        /// </summary>
        /// <param name="obj">The <see cref="ConsoleString"/> to compare with this <see cref="ConsoleString"/>.</param>
        /// <returns><c>true</c> if the specified <see cref="ConsoleString" /> is equal to this <see cref="ConsoleString"/>; otherwise, <c>false</c>.</returns>
        public bool Equals(ConsoleString obj)
        {
            if (ReferenceEquals(obj, null))
                return false;
            else if (ReferenceEquals(obj, this))
                return true;
            else if (obj.GetHashCode() != this.GetHashCode())
                return false;
            else if (obj._content.Length != this._content.Length)
                return false;
            else
            {
                for (int i = 0; i < obj._content.Length; i++)
                    if (obj._content[i].Color != this._content[i].Color || obj._content[i].Content != this._content[i].Content)
                        return false;

                return true;
            }
        }

        /// <summary>
        /// Gets a value indicating whether any part of this console string has colors.
        /// </summary>
        /// <remarks>
        /// Colors can be removed by calling <see cref="ClearColors"/>.
        /// </remarks>
        public bool HasColors => _hasColors.Value;
        /// <summary>
        /// Gets the text content of this <see cref="ConsoleString"/>, ignoring the color information.
        /// </summary>
        public string Content => _text.Value;
        /// <summary>
        /// Gets the number of characters in the <see cref="ConsoleString"/>.
        /// </summary>
        public int Length => _text.Value.Length;

        /// <summary>
        /// Returns a new <see cref="ConsoleString"/> that has been stripped of coloring.
        /// </summary>
        /// <returns>A <see cref="ConsoleString"/> that has been stripped of coloring.</returns>
        public ConsoleString ClearColors()
        {
            return new ConsoleString(new ConsoleStringSegment[] { new ConsoleStringSegment(string.Concat(_content.Select(x => x.Content)), null) });
        }
        /// <summary>
        /// Returns a new <see cref="ConsoleString"/> where the currently parsed color-information is escaped.
        /// </summary>
        /// <returns>A new <see cref="ConsoleString"/> where the currently parsed color-information is escaped.</returns>
        public ConsoleString EscapeColors()
        {
            return new ConsoleString(new ConsoleStringSegment[] { new ConsoleStringSegment(string.Concat(_content.Select(x => $@"\[{x.Color}:{x.Content}\]")), null) });
        }

        /// <summary>
        /// Evaluates the contents of this <see cref="ConsoleString"/>.
        /// </summary>
        /// <param name="maintainEscape">
        /// Determines if escaped characters should remain escaped when parsing.
        /// If <c>true</c> any escaped character (such as "\[") will remain in its escaped state, otherwise it will be converted into the unescaped version (such as "[").
        /// </param>
        /// <returns>A new <see cref="ConsoleString"/> representing the parsed contents.</returns>
        public ConsoleString EvaluateSegments(bool maintainEscape = false)
        {
            return new ConsoleString(_content.SelectMany(x =>
            {
                var str = Parse(x.Content, maintainEscape);

                var segments = str._content;
                if (x.HasColor)
                    segments = segments.Select(s => new ConsoleStringSegment(s.Content, s.Color ?? x.Color)).ToArray();

                return segments;
            }));
        }
    }
}

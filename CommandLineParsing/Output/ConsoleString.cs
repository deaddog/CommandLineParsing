using System;
using System.Collections.Generic;
using System.Linq;

namespace CommandLineParsing.Output
{
    /// <summary>
    /// Represents a string with embedded color-information.
    /// A string is implicitly converted into a <see cref="ConsoleString"/>.
    /// </summary>
    public partial class ConsoleString
    {
        private readonly Segment[] content;
        private readonly Lazy<string> text;
        private readonly Lazy<bool> hasColors;

        internal IEnumerable<Segment> GetSegments()
        {
            foreach (var s in content)
                yield return s;
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="System.String"/> to <see cref="ConsoleString"/>.
        /// With this a format can be specified simply as a string when used as a parameter.
        /// </summary>
        /// <param name="text">The text that is parsed to a <see cref="ConsoleString"/>. See <see cref="ConsoleString.ConsoleString(string, bool)"/>.</param>
        /// <returns>
        /// A new <see cref="ConsoleString"/> that represents the format found in <paramref name="text"/>.
        /// </returns>
        public static implicit operator ConsoleString(string text)
        {
            return new ConsoleString(text, false);
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
            if (s1.content.Length == 0)
                return s2;
            else if (s2.content.Length == 0)
                return s1;
            else if (s1.content[s1.content.Length - 1].Color == s2.content[0].Color)
            {
                var l = new List<Segment>();

                l.AddRange(s1.content.Take(s1.content.Length - 1));
                l.Add(new Segment(s1.content[s1.content.Length - 1].Content + s2.content[0].Content, s2.content[0].Color));
                l.AddRange(s2.content.Skip(1));

                return new ConsoleString(l);
            }
            else
                return new ConsoleString(s1.content.Concat(s2.content));
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

        private ConsoleString(IEnumerable<Segment> segments)
        {
            content = segments.ToArray();
            text = new Lazy<string>(() => string.Concat(content.Select(x => x.Content)));
            hasColors = new Lazy<bool>(() => content.Any(x => x.HasColor));
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleString"/> class, representing an empty string.
        /// </summary>
        public ConsoleString()
            : this(new Segment[0])
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleString"/> class from a string.
        /// Any color-encoding will be parsed by the constructor.
        /// </summary>
        /// <param name="content">
        /// The content that should be contained by the <see cref="ConsoleString"/>.
        /// The string "[Color:Text]" will translate to a <see cref="ConsoleString"/> using Color as the foreground color.
        /// </param>
        /// <param name="maintainEscape">
        /// Determines if escaped characters should remain escaped when parsing <paramref name="content"/>.
        /// If <c>true</c> any escaped character (such as "\[") will remain in its escaped state, otherwise it will be converted into the unescaped version (such as "[").
        /// </param>
        public ConsoleString(string content, bool maintainEscape = false)
            : this(Segment.Parse(content, maintainEscape))
        {
        }

        /// <summary>
        /// Returns a hash code for this <see cref="ConsoleString"/>.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return text.Value.GetHashCode();
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
            else if (obj.content.Length != this.content.Length)
                return false;
            else
            {
                for (int i = 0; i < obj.content.Length; i++)
                    if (obj.content[i].Color != this.content[i].Color || obj.content[i].Content != this.content[i].Content)
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
        public bool HasColors => hasColors.Value;
        /// <summary>
        /// Gets the text content of this <see cref="ConsoleString"/>, ignoring the color information.
        /// </summary>
        public string Content => text.Value;
        /// <summary>
        /// Gets the number of characters in the <see cref="ConsoleString"/>.
        /// </summary>
        public int Length => text.Value.Length;

        /// <summary>
        /// Returns a new <see cref="ConsoleString"/> that has been stripped of coloring.
        /// </summary>
        /// <returns>A <see cref="ConsoleString"/> that has been stripped of coloring.</returns>
        public ConsoleString ClearColors()
        {
            return new ConsoleString(new Segment[] { new Segment(string.Concat(content.Select(x => x.Content)), null) });
        }
        /// <summary>
        /// Returns a new <see cref="ConsoleString"/> where the currently parsed color-information is escaped.
        /// </summary>
        /// <returns>A new <see cref="ConsoleString"/> where the currently parsed color-information is escaped.</returns>
        public ConsoleString EscapeColors()
        {
            return new ConsoleString(new Segment[] { new Segment(string.Concat(content.Select(x => $@"\[{x.Color}:{x.Content}\]")), null) });
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
            return new ConsoleString(content.SelectMany(x =>
            {
                var str = new ConsoleString(x.Content, maintainEscape);

                var segments = str.content;
                if (x.HasColor)
                    segments = segments.Select(s => new Segment(s.Content, s.Color ?? x.Color)).ToArray();

                return segments;
            }));
        }
    }
}

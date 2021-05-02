using ConsoleTools.Coloring;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace ConsoleTools
{
    public partial class ConsoleString : IEquatable<ConsoleString>, IEnumerable<ConsoleString.Segment>
    {
        private readonly IImmutableList<Segment> _segments;
        private readonly Lazy<string> _text;
        private readonly Lazy<bool> _hasColors;

        public static ConsoleString Empty { get; } = new ConsoleString(ImmutableList<Segment>.Empty);

        public static ConsoleString Parse(string content) => Create(Segment.Parse(content));

        public static ConsoleString Create(Segment segment) => new ConsoleString(ImmutableList.Create(segment));
        public static ConsoleString Create(IEnumerable<Segment> segments) => new ConsoleString(segments.ToImmutableList());
        public static ConsoleString Create(params Segment[] segments) => new ConsoleString(segments.ToImmutableList());

        public static ConsoleString FromContent(string content) => FromContent(content, Color.NoColor);
        public static ConsoleString FromContent(string content, Color color) => Create(new Segment(content, color));

        public ConsoleString(IImmutableList<Segment> segments)
        {
            // Merge sequential segments that use the same color
            for (int i = 1; i < segments.Count; i++)
                if (segments[i].Color == segments[i - 1].Color)
                    segments = segments.RemoveAt(i - 1).SetItem
                    (
                        index: i - 1,
                        value: new Segment
                        (
                            content: segments[i - 1].Content + segments[i].Content,
                            color: segments[i].Color
                        )
                    );

            _segments = segments;
            _text = new Lazy<string>(() => string.Concat(_segments.Select(x => x.Content)));
            _hasColors = new Lazy<bool>(() => _segments.Any(x => x.HasColor));
        }

        public static implicit operator ConsoleString(string text) => Parse(text);

        public static ConsoleString operator +(ConsoleString s1, ConsoleString s2) => new ConsoleString(s1._segments.AddRange(s2._segments));
        public static ConsoleString operator +(string text, ConsoleString str) => (ConsoleString)text + str;
        public static ConsoleString operator +(ConsoleString str, string text) => str + (ConsoleString)text;

        public static bool operator ==(ConsoleString s1, ConsoleString s2) => EqualityComparer<ConsoleString>.Default.Equals(s1, s2);
        public static bool operator !=(ConsoleString s1, ConsoleString s2) => !(s1 == s2);

        public ConsoleString this[Index index]
        {
            get
            {
                var offset = index.GetOffset(Length);

                for (int i = 0; i < _segments.Count; i++)
                {
                    if (_segments[i].Content.Length <= offset)
                        offset -= _segments[i].Content.Length;
                    else
                        return FromContent
                        (
                            content: _segments[i].Content.Substring(offset, 1),
                            color: _segments[i].Color
                        );
                }

                throw new ArgumentOutOfRangeException(nameof(index), $"Index and length must refer to a location within the {nameof(ConsoleString)}.");
            }
        }
        public ConsoleString this[Range range]
        {
            get
            {
                var (offset, length) = range.GetOffsetAndLength(Length);

                var segments = ImmutableList<Segment>.Empty;

                for (int index = 0; length > 0; index++)
                {
                    if (index >= _segments.Count)
                        throw new ArgumentOutOfRangeException(nameof(range), $"Index and length must refer to a location within the {nameof(ConsoleString)}.");

                    if (_segments[index].Content.Length <= offset)
                        offset -= _segments[index].Content.Length;
                    else
                    {
                        var segment = new Segment
                        (
                            content: _segments[index].Content.Substring
                            (
                                offset,
                                Math.Min(_segments[index].Content.Length, length) - offset
                            ),
                            color: _segments[index].Color
                        );

                        segments = segments.Add(segment);

                        offset = 0;
                        length -= segment.Content.Length;
                    }
                }

                return new ConsoleString(segments);
            }
        }

        public bool HasColors => _hasColors.Value;
        public string Content => _text.Value;
        public int Length => _text.Value.Length;

        public ConsoleString WithColor(Color color) => FromContent(Content, color);
        public ConsoleString WithoutColors() => FromContent(Content);

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _segments.GetEnumerator();
        }
        public IEnumerator<Segment> GetEnumerator()
        {
            foreach (var s in _segments)
                yield return s;
        }

        public override int GetHashCode()
        {
            return _text.Value.GetHashCode();
        }
        public override bool Equals(object? obj)
        {
            return obj is ConsoleString cs && Equals(cs);
        }
        public bool Equals(ConsoleString? other)
        {
            return !(other is null) &&
                   _segments.Count == other._segments.Count &&
                   _segments.Zip(other._segments, (a, b) => a.Equals(b)).All(x => x);
        }
    }
}

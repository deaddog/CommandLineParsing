using System.Collections.Generic;
using System.Linq;

namespace CommandLineParsing.Tests
{
    public class TestingConsoleString
    {
        private readonly ConsolePoint _position;
        private readonly List<TestingConsoleSegment> _segments;

        public TestingConsoleString(ConsolePoint position, IEnumerable<TestingConsoleSegment> segments)
        {
            _position = position;
            _segments = new List<TestingConsoleSegment>(segments);

            while (_segments.Count > 0 && _segments[0].Text.StartsWith(" "))
            {
                var text = _segments[0].Text.TrimStart(' ');
                _position.Left += _segments[0].Text.Length - text.Length;
                if (text.Length == 0)
                    _segments.RemoveAt(0);
                else
                    _segments[0] = new TestingConsoleSegment(text, _segments[0].Foreground, _segments[0].Background);
            }
        }

        public ConsolePoint Position => _position;

        public int SegmentCount => _segments.Count;
        public TestingConsoleSegment this[int index] => _segments[index];
        public IEnumerable<TestingConsoleSegment> GetSegments()
        {
            foreach (var s in _segments)
                yield return s;
        }

        public string Text => string.Join("", _segments.Select(x => x.Text));
    }
}

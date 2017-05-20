using System.Collections.Generic;

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
        }

        public ConsolePoint Position => _position;

        public int SegmentCount => _segments.Count;
        public TestingConsoleSegment this[int index] => _segments[index];
        public IEnumerable<TestingConsoleSegment> GetSegments()
        {
            foreach (var s in _segments)
                yield return s;
        }
    }
}

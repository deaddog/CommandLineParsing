using CommandLineParsing.Output;
using NUnit.Framework;

namespace CommandLineParsing.Tests.Output
{
    [TestFixture]
    public class ConsoleStringTests
    {
        [Test]
        public void ParseEmptyString()
        {
            Assert.AreEqual(ConsoleString.Empty, new ConsoleString());
            Assert.AreEqual(ConsoleString.Empty, ConsoleString.Parse(string.Empty, true));
            Assert.AreEqual(ConsoleString.Empty, ConsoleString.Parse(string.Empty, false));
            Assert.AreEqual(ConsoleString.Empty, new ConsoleString(new[] { new ConsoleStringSegment(string.Empty) }));
        }

        [Test]
        public void ParseStringFlatten()
        {
            Assert.AreEqual(ConsoleString.Parse("test1test2"), new ConsoleString(new[] { new ConsoleStringSegment("test1"), new ConsoleStringSegment("test2") }));
            Assert.AreEqual(ConsoleString.Parse("[red:test1test2]"), ConsoleString.Parse("[red:test1][red:test2]"));
            Assert.AreEqual(ConsoleString.Parse("[blue:test1][red:test2]test3"), ConsoleString.Parse("[blue:test1][red:test2]test3"));
            Assert.AreEqual(ConsoleString.Parse("[blue:test1][red:test2test3]test4"), ConsoleString.Parse("[blue:test1][red:test2][red:test3]test4"));
        }

        [Test]
        public void ParseNoColor()
        {
            Assert.AreEqual(ConsoleString.Empty, ConsoleString.Parse("[:]"));
            Assert.AreEqual(ConsoleString.Parse("test1"), ConsoleString.Parse("[:test1]"));
            Assert.AreEqual(ConsoleString.Parse("test1"), ConsoleString.Parse("[ :test1]"));
        }
    }
}

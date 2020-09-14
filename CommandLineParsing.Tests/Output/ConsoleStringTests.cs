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

        [Test]
        public void GetIndex()
        {
            Assert.AreEqual(ConsoleString.Parse("t"), ConsoleString.Parse("test1")[0]);
            Assert.AreEqual(ConsoleString.Parse("e"), ConsoleString.Parse("test1")[1]);
            Assert.AreEqual(ConsoleString.Parse("1"), ConsoleString.Parse("test1")[^1]);
            Assert.AreEqual(ConsoleString.Parse("s"), ConsoleString.Parse("test1")[^3]);

            Assert.AreEqual(ConsoleString.Parse("[blue:t]"), ConsoleString.Parse("[blue:test1]")[0]);
            Assert.AreEqual(ConsoleString.Parse("[blue:e]"), ConsoleString.Parse("[blue:test1]")[1]);
            Assert.AreEqual(ConsoleString.Parse("[blue:1]"), ConsoleString.Parse("[blue:test1]")[^1]);
            Assert.AreEqual(ConsoleString.Parse("[blue:s]"), ConsoleString.Parse("[blue:test1]")[^3]);

            Assert.AreEqual(ConsoleString.Parse("[blue:t]"), ConsoleString.Parse("[blue:test1]test2")[0]);
            Assert.AreEqual(ConsoleString.Parse("[blue:e]"), ConsoleString.Parse("[blue:test1]test2")[1]);
            Assert.AreEqual(ConsoleString.Parse("2"), ConsoleString.Parse("[blue:test1]test2")[^1]);
            Assert.AreEqual(ConsoleString.Parse("s"), ConsoleString.Parse("[blue:test1]test2")[^3]);

            Assert.AreEqual(ConsoleString.Parse("t"), ConsoleString.Parse("test1[red:test2]")[0]);
            Assert.AreEqual(ConsoleString.Parse("e"), ConsoleString.Parse("test1[red:test2]")[1]);
            Assert.AreEqual(ConsoleString.Parse("[red:2]"), ConsoleString.Parse("test1[red:test2]")[^1]);
            Assert.AreEqual(ConsoleString.Parse("[red:s]"), ConsoleString.Parse("test1[red:test2]")[^3]);
        }

        [Test]
        public void GetRange()
        {
            Assert.AreEqual(ConsoleString.Empty, ConsoleString.Empty[0..0]);
            Assert.AreEqual(ConsoleString.Empty, ConsoleString.Parse("test1")[0..0]);

            Assert.AreEqual(ConsoleString.Parse("test1"), ConsoleString.Parse("test1")[0..5]);
            Assert.AreEqual(ConsoleString.Parse("test1"), ConsoleString.Parse("test1test2")[0..5]);
            Assert.AreEqual(ConsoleString.Parse("test1test"), ConsoleString.Parse("test1test2")[0..^1]);

            Assert.AreEqual(ConsoleString.Parse("[blue:test1]"), ConsoleString.Parse("[blue:test1]test2")[0..5]);
            Assert.AreEqual(ConsoleString.Parse("test1"), ConsoleString.Parse("test1[red:test2]")[0..5]);
            Assert.AreEqual(ConsoleString.Parse("test2"), ConsoleString.Parse("[blue:test1]test2")[^5..^0]);
            Assert.AreEqual(ConsoleString.Parse("[red:test2]"), ConsoleString.Parse("test1[red:test2]")[^5..^0]);

            Assert.AreEqual(ConsoleString.Parse("[blue:test2]test3"), ConsoleString.Parse("[blue:test1test2]test3")[5..^0]);
        }
    }
}

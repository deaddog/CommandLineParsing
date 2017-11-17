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
    }
}

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
            Assert.AreEqual(new ConsoleString(), new ConsoleString(string.Empty, true));
            Assert.AreEqual(new ConsoleString(), new ConsoleString(string.Empty, false));
        }
    }
}

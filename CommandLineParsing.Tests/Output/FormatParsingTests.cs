using CommandLineParsing.Output.Formatting.Structure;
using NUnit.Framework;

namespace CommandLineParsing.Tests.Output
{
    [TestFixture]
    public class FormatParsingTests
    {
        [Test]
        public void ParseEmptyString()
        {
            Assert.AreEqual(new FormatText(string.Empty), FormatElement.Parse(string.Empty));
        }
    }
}

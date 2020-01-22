using CommandLineParsing.Output;
using NUnit.Framework;

namespace CommandLineParsing.Tests.Output
{
    [TestFixture]
    public class ColorTests
    {
        [Test]
        public void ParseEmptyString()
        {
            Assert.AreEqual(Color.NoColor, Color.Parse(""));
            Assert.AreEqual(Color.NoColor, Color.Parse("|"));
            Assert.AreEqual(Color.NoColor, Color.Parse("  |"));
            Assert.AreEqual(Color.NoColor, Color.Parse("|  "));
            Assert.AreEqual(Color.NoColor, Color.Parse("  |  "));
        }

        [Test]
        public void ParseForegroundOnlyString()
        {
            var red = Color.NoColor.WithForeground("red");

            Assert.AreEqual(red, Color.Parse("red"));
            Assert.AreEqual(red, Color.Parse("red|"));
            Assert.AreEqual(red, Color.Parse("  red"));
            Assert.AreEqual(red, Color.Parse("  red| "));
            Assert.AreEqual(red, Color.Parse("  red  "));
        }

        [Test]
        public void ParseBackgroundOnlyString()
        {
            var red = Color.NoColor.WithBackground("red");

            Assert.AreEqual(red, Color.Parse("|red"));
            Assert.AreEqual(red, Color.Parse(" |  red"));
        }
    }
}

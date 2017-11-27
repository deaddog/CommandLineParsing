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
            Assert.AreEqual(FormatNoContent.Element, FormatElement.Parse(string.Empty));
        }

        [Test]
        public void ParseBasicString()
        {
            Assert.AreEqual(new FormatText("hello world"), FormatElement.Parse("hello world"));
        }

        [Test]
        public void ParseColorString()
        {
            Assert.AreEqual(FormatNoContent.Element, FormatElement.Parse("[green:]"));
            Assert.AreEqual(new FormatText("hello world"), FormatElement.Parse("[:hello world]"));
            Assert.AreEqual(new FormatText("hello world"), FormatElement.Parse("[hello world]"));
            Assert.AreEqual(new FormatColor("green", new FormatText("hello world")), FormatElement.Parse("[green:hello world]"));
            Assert.AreEqual(new FormatColor("green", new FormatText("hello world")), FormatElement.Parse("[green:hello ][green:world]"));

            Assert.AreEqual(new FormatConcatenation(new FormatElement[] {
                    new FormatText("text1"),
                    new FormatColor("green", new FormatText("hello world"))
                }),
                FormatElement.Parse("text1[green:hello world]"));

            Assert.AreEqual(new FormatConcatenation(new FormatElement[] {
                    new FormatColor("green", new FormatText("hello world")),
                    new FormatText("text2")
                }),
                FormatElement.Parse("[green:hello world]text2"));

            Assert.AreEqual(new FormatConcatenation(new FormatElement[] {
                    new FormatText("text1"),
                    new FormatColor("green", new FormatText("hello world")),
                    new FormatText("text2")
                }),
                FormatElement.Parse("text1[green:hello world]text2"));
        }
    }
}

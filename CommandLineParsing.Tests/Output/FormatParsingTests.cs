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
        public void ParseEscapedString()
        {
            Assert.AreEqual(new FormatText("hello world"), FormatElement.Parse(@"hello \world"));
            Assert.AreEqual(new FormatText("hello $world"), FormatElement.Parse(@"hello \$world"));
            Assert.AreEqual(new FormatText("hello @world"), FormatElement.Parse(@"hello \@world"));
            Assert.AreEqual(new FormatText("hello ?world"), FormatElement.Parse(@"hello \?world"));
            Assert.AreEqual(new FormatText("hello [world]"), FormatElement.Parse(@"hello \[world]"));
            Assert.AreEqual(new FormatText("hello [world]"), FormatElement.Parse(@"hello \[world\]"));
            Assert.AreEqual(new FormatText("hello {world}"), FormatElement.Parse(@"hello \{world}"));
            Assert.AreEqual(new FormatText("hello {world}"), FormatElement.Parse(@"hello \{world\}"));
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

        [Test]
        public void ParseVariableString()
        {
            Assert.AreEqual(new FormatVariable("variable", FormatPaddings.None), FormatElement.Parse("$variable"));
            Assert.AreEqual(new FormatVariable("variable", FormatPaddings.PadLeft), FormatElement.Parse("$+variable"));
            Assert.AreEqual(new FormatVariable("variable", FormatPaddings.PadRight), FormatElement.Parse("$variable+"));
            Assert.AreEqual(new FormatVariable("variable", FormatPaddings.PadBoth), FormatElement.Parse("$+variable+"));

            Assert.AreEqual(new FormatVariable("variable1", FormatPaddings.None), FormatElement.Parse("$variable1"));
            Assert.AreEqual(new FormatText("$1variable"), FormatElement.Parse("$1variable"));

            Assert.AreEqual(new FormatConcatenation(new FormatElement[]
                {
                    new FormatText("text1"),
                    new FormatVariable("variable", FormatPaddings.None),
                    new FormatText("text2")
                }),
                FormatElement.Parse(@"text1$variable\text2"));
            Assert.AreEqual(new FormatConcatenation(new FormatElement[]
                {
                    new FormatText("text1"),
                    new FormatVariable("variable", FormatPaddings.PadLeft),
                    new FormatText("text2")
                }),
                FormatElement.Parse(@"text1$+variable\text2"));
            Assert.AreEqual(new FormatConcatenation(new FormatElement[]
                {
                    new FormatText("text1"),
                    new FormatVariable("variable", FormatPaddings.PadRight),
                    new FormatText("text2")
                }),
                FormatElement.Parse(@"text1$variable+\text2"));
            Assert.AreEqual(new FormatConcatenation(new FormatElement[]
                {
                    new FormatText("text1"),
                    new FormatVariable("variable", FormatPaddings.PadBoth),
                    new FormatText("text2")
                }),
                FormatElement.Parse(@"text1$+variable+\text2"));

            Assert.AreEqual(new FormatConcatenation(new FormatElement[]
                {
                    new FormatText("text1"),
                    new FormatVariable("variable", FormatPaddings.PadRight),
                    new FormatText("text2")
                }),
                FormatElement.Parse(@"text1$variable+text2"));
            Assert.AreEqual(new FormatConcatenation(new FormatElement[]
                {
                    new FormatText("text1"),
                    new FormatVariable("variable", FormatPaddings.PadBoth),
                    new FormatText("text2")
                }),
                FormatElement.Parse(@"text1$+variable+text2"));
        }
    }
}

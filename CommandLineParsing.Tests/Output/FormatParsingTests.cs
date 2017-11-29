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

            Assert.AreEqual(new FormatColor("green", new FormatText("hello world")), FormatElement.Parse("[green:hello world"));

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

        [Test]
        public void ParseConditionString()
        {
            Assert.AreEqual(new FormatCondition("cond", false, FormatNoContent.Element), FormatElement.Parse("?cond{}"));
            Assert.AreEqual(new FormatCondition("cond", true, FormatNoContent.Element), FormatElement.Parse("?!cond{}"));

            Assert.AreEqual(new FormatCondition("cond", false, new FormatText("text")), FormatElement.Parse("?cond{text}"));
            Assert.AreEqual(new FormatCondition("cond", true, new FormatText("text")), FormatElement.Parse("?!cond{text}"));

            Assert.AreEqual(new FormatCondition("cond", false, FormatNoContent.Element), FormatElement.Parse("?cond{"));
            Assert.AreEqual(new FormatText("?1cond{}"), FormatElement.Parse("?1cond{}"));
            Assert.AreEqual(new FormatText("?1cond{text}"), FormatElement.Parse("?1cond{text}"));

            Assert.AreEqual(new FormatCondition("cond1", false, FormatNoContent.Element), FormatElement.Parse("?cond1{}"));
            Assert.AreEqual(new FormatCondition("cond1", true, new FormatText("text")), FormatElement.Parse("?!cond1{text}"));
        }

        [Test]
        public void ParseFunctionString()
        {
            Assert.AreEqual(new FormatFunction("func", new[] { FormatNoContent.Element }), FormatElement.Parse("@func{}"));
            Assert.AreEqual(new FormatFunction("func1", new[] { FormatNoContent.Element }), FormatElement.Parse("@func1{}"));

            Assert.AreEqual(new FormatText("@1func{}"), FormatElement.Parse("@1func{}"));
            Assert.AreEqual(new FormatFunction("func", new[] { FormatNoContent.Element }), FormatElement.Parse("@func{"));

            Assert.AreEqual(new FormatFunction("func", new[] { new FormatText("arg1") }), FormatElement.Parse("@func{arg1}"));
            Assert.AreEqual(new FormatFunction("func", new[] { new FormatText("arg1"), new FormatText("arg2") }), FormatElement.Parse("@func{arg1,arg2}"));
        }
    }
}

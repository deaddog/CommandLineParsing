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
            Assert.AreEqual(FormatNoContentElement.Element, FormatElement.Parse(string.Empty));
        }

        [Test]
        public void ParseBasicString()
        {
            Assert.AreEqual(new FormatTextElement("hello world"), FormatElement.Parse("hello world"));
        }

        [Test]
        public void ParseEscapedString()
        {
            Assert.AreEqual(new FormatTextElement("hello world"), FormatElement.Parse(@"hello \world"));
            Assert.AreEqual(new FormatTextElement("hello $world"), FormatElement.Parse(@"hello \$world"));
            Assert.AreEqual(new FormatTextElement("hello @world"), FormatElement.Parse(@"hello \@world"));
            Assert.AreEqual(new FormatTextElement("hello ?world"), FormatElement.Parse(@"hello \?world"));
            Assert.AreEqual(new FormatTextElement("hello [world]"), FormatElement.Parse(@"hello \[world]"));
            Assert.AreEqual(new FormatTextElement("hello [world]"), FormatElement.Parse(@"hello \[world\]"));
            Assert.AreEqual(new FormatTextElement("hello {world}"), FormatElement.Parse(@"hello \{world}"));
            Assert.AreEqual(new FormatTextElement("hello {world}"), FormatElement.Parse(@"hello \{world\}"));
        }

        [Test]
        public void ParseColorString()
        {
            Assert.AreEqual(FormatNoContentElement.Element, FormatElement.Parse("[green:]"));
            Assert.AreEqual(new FormatTextElement("hello world"), FormatElement.Parse("[:hello world]"));
            Assert.AreEqual(new FormatTextElement("hello world"), FormatElement.Parse("[hello world]"));
            Assert.AreEqual(new FormatColorElement("green", new FormatTextElement("hello world")), FormatElement.Parse("[green:hello world]"));
            Assert.AreEqual(new FormatColorElement("green", new FormatTextElement("hello world")), FormatElement.Parse("[green:hello ][green:world]"));

            Assert.AreEqual(new FormatColorElement("green", new FormatTextElement("hello world")), FormatElement.Parse("[green:hello world"));

            Assert.AreEqual(new FormatConcatenationElement(new FormatElement[] {
                    new FormatTextElement("text1"),
                    new FormatColorElement("green", new FormatTextElement("hello world"))
                }),
                FormatElement.Parse("text1[green:hello world]"));

            Assert.AreEqual(new FormatConcatenationElement(new FormatElement[] {
                    new FormatColorElement("green", new FormatTextElement("hello world")),
                    new FormatTextElement("text2")
                }),
                FormatElement.Parse("[green:hello world]text2"));

            Assert.AreEqual(new FormatConcatenationElement(new FormatElement[] {
                    new FormatTextElement("text1"),
                    new FormatColorElement("green", new FormatTextElement("hello world")),
                    new FormatTextElement("text2")
                }),
                FormatElement.Parse("text1[green:hello world]text2"));
        }

        [Test]
        public void ParseVariableString()
        {
            Assert.AreEqual(new FormatVariableElement("variable", FormatVariablePaddings.None), FormatElement.Parse("$variable"));
            Assert.AreEqual(new FormatVariableElement("variable", FormatVariablePaddings.PadLeft), FormatElement.Parse("$+variable"));
            Assert.AreEqual(new FormatVariableElement("variable", FormatVariablePaddings.PadRight), FormatElement.Parse("$variable+"));
            Assert.AreEqual(new FormatVariableElement("variable", FormatVariablePaddings.PadBoth), FormatElement.Parse("$+variable+"));

            Assert.AreEqual(new FormatVariableElement("variable1", FormatVariablePaddings.None), FormatElement.Parse("$variable1"));
            Assert.AreEqual(new FormatTextElement("$1variable"), FormatElement.Parse("$1variable"));

            Assert.AreEqual(new FormatConcatenationElement(new FormatElement[]
                {
                    new FormatTextElement("text1"),
                    new FormatVariableElement("variable", FormatVariablePaddings.None),
                    new FormatTextElement("text2")
                }),
                FormatElement.Parse(@"text1$variable\text2"));
            Assert.AreEqual(new FormatConcatenationElement(new FormatElement[]
                {
                    new FormatTextElement("text1"),
                    new FormatVariableElement("variable", FormatVariablePaddings.PadLeft),
                    new FormatTextElement("text2")
                }),
                FormatElement.Parse(@"text1$+variable\text2"));
            Assert.AreEqual(new FormatConcatenationElement(new FormatElement[]
                {
                    new FormatTextElement("text1"),
                    new FormatVariableElement("variable", FormatVariablePaddings.PadRight),
                    new FormatTextElement("text2")
                }),
                FormatElement.Parse(@"text1$variable+\text2"));
            Assert.AreEqual(new FormatConcatenationElement(new FormatElement[]
                {
                    new FormatTextElement("text1"),
                    new FormatVariableElement("variable", FormatVariablePaddings.PadBoth),
                    new FormatTextElement("text2")
                }),
                FormatElement.Parse(@"text1$+variable+\text2"));

            Assert.AreEqual(new FormatConcatenationElement(new FormatElement[]
                {
                    new FormatTextElement("text1"),
                    new FormatVariableElement("variable", FormatVariablePaddings.PadRight),
                    new FormatTextElement("text2")
                }),
                FormatElement.Parse(@"text1$variable+text2"));
            Assert.AreEqual(new FormatConcatenationElement(new FormatElement[]
                {
                    new FormatTextElement("text1"),
                    new FormatVariableElement("variable", FormatVariablePaddings.PadBoth),
                    new FormatTextElement("text2")
                }),
                FormatElement.Parse(@"text1$+variable+text2"));
        }

        [Test]
        public void ParseConditionString()
        {
            Assert.AreEqual(new FormatConditionElement("cond", false, FormatNoContentElement.Element), FormatElement.Parse("?cond{}"));
            Assert.AreEqual(new FormatConditionElement("cond", true, FormatNoContentElement.Element), FormatElement.Parse("?!cond{}"));

            Assert.AreEqual(new FormatConditionElement("cond", false, new FormatTextElement("text")), FormatElement.Parse("?cond{text}"));
            Assert.AreEqual(new FormatConditionElement("cond", true, new FormatTextElement("text")), FormatElement.Parse("?!cond{text}"));

            Assert.AreEqual(new FormatConditionElement("cond", false, FormatNoContentElement.Element), FormatElement.Parse("?cond{"));
            Assert.AreEqual(new FormatTextElement("?1cond{}"), FormatElement.Parse("?1cond{}"));
            Assert.AreEqual(new FormatTextElement("?1cond{text}"), FormatElement.Parse("?1cond{text}"));

            Assert.AreEqual(new FormatConditionElement("cond1", false, FormatNoContentElement.Element), FormatElement.Parse("?cond1{}"));
            Assert.AreEqual(new FormatConditionElement("cond1", true, new FormatTextElement("text")), FormatElement.Parse("?!cond1{text}"));
        }

        [Test]
        public void ParseFunctionString()
        {
            Assert.AreEqual(new FormatFunctionElement("func", new[] { FormatNoContentElement.Element }), FormatElement.Parse("@func{}"));
            Assert.AreEqual(new FormatFunctionElement("func1", new[] { FormatNoContentElement.Element }), FormatElement.Parse("@func1{}"));

            Assert.AreEqual(new FormatTextElement("@1func{}"), FormatElement.Parse("@1func{}"));
            Assert.AreEqual(new FormatFunctionElement("func", new[] { FormatNoContentElement.Element }), FormatElement.Parse("@func{"));

            Assert.AreEqual(new FormatFunctionElement("func", new[] { new FormatTextElement("arg1") }), FormatElement.Parse("@func{arg1}"));
            Assert.AreEqual(new FormatFunctionElement("func", new[] { new FormatTextElement("arg1"), new FormatTextElement("arg2") }), FormatElement.Parse("@func{arg1,arg2}"));
        }
    }
}

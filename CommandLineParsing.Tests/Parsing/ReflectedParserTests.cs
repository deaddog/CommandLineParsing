using CommandLineParsing.Parsing;
using NUnit.Framework;
using System;

namespace CommandLineParsing.Tests.Parsing
{
    [TestFixture]
    public class ReflectedParserTests : ConsoleTestBase
    {
        private ReflectedParserSettings _settings, _settingsIgnoreCase;

        #region Parsing Types

        private class NoParser
        {
        }
        private class HasMessageParser
        {
            public string Text { get; }

            public HasMessageParser(string text)
            {
                Text = text;
            }

            public static Message TryParse(string text, out HasMessageParser result)
            {
                result = new HasMessageParser(text);
                return Message.NoError;
            }
        }
        private class HasParser
        {
            public string Text { get; }

            public HasParser(string text)
            {
                Text = text;
            }

            public static bool TryParse(string text, out HasParser result)
            {
                result = new HasParser(text);
                return true;
            }
        }

        #endregion

        [SetUp]
        public void InitializeSettings()
        {
            _settings = new ReflectedParserSettings
            (
                enumIgnoreCase: false,
                noValueMessage: new Message("MULTI"),
                multipleValuesMessage: new Message("NO"),
                typeErrorMessage: x => new Message($"TYPE[{x}]"),
                useParserMessage: false
            );
            _settingsIgnoreCase = new ReflectedParserSettings
            (
                enumIgnoreCase: true,
                noValueMessage: new Message("MULTI"),
                multipleValuesMessage: new Message("NO"),
                typeErrorMessage: x => new Message($"TYPE[{x}]"),
                useParserMessage: false
            );
        }

        private static Message<T> Parse<T>(ReflectedParserSettings settings, string[] args) => new ReflectedParser<T>(settings).Parse(args);
        private static Message<T> Parse<T>(ReflectedParserSettings settings, string input) => new ReflectedParser<T>(settings).Parse(input);

        [Test]
        public void ParseBasicTypes()
        {
            System.Globalization.CultureInfo.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

            Assert.AreEqual(5d, Parse<double>(_settings, "5.0").Value);
            Assert.AreEqual(5f, Parse<float>(_settings, "5.0").Value);
            Assert.AreEqual(14, Parse<int>(_settings, "14").Value);
            Assert.AreEqual("hello", Parse<string>(_settings, "hello").Value);
            Assert.AreEqual(new DateTime(2017, 11, 13), Parse<DateTime>(_settings, "2017-11-13").Value);
        }

        [Test]
        public void ParseEnumCaseTypes()
        {
            Assert.IsTrue(Parse<ConsoleColor>(_settings, "red").IsError);
            Assert.IsTrue(Parse<ConsoleColor>(_settings, "rEd").IsError);
            Assert.AreEqual(ConsoleColor.Red, Parse<ConsoleColor>(_settings, "Red").Value);
            Assert.IsTrue(Parse<ConsoleColor>(_settings, "reds").IsError);
        }
        [Test]
        public void ParseEnumNoCaseTypes()
        {
            Assert.AreEqual(ConsoleColor.Red, Parse<ConsoleColor>(_settingsIgnoreCase, "red").Value);
            Assert.AreEqual(ConsoleColor.Red, Parse<ConsoleColor>(_settingsIgnoreCase, "rEd").Value);
            Assert.AreEqual(ConsoleColor.Red, Parse<ConsoleColor>(_settingsIgnoreCase, "Red").Value);
            Assert.IsTrue(Parse<ConsoleColor>(_settingsIgnoreCase, "reds").IsError);
        }

        [Test]
        public void ParseArray()
        {
            Assert.AreEqual(14, Parse<int>(_settings, new string[] { "14" }).Value);
            Assert.AreEqual(new[] { 14 }, Parse<int[]>(_settings, new string[] { "14" }).Value);
            Assert.AreEqual(new[] { 14, 18 }, Parse<int[]>(_settings, new string[] { "14", "18" }).Value);
        }

        [Test]
        public void MissingParserExceptionThrow()
        {
            Assert.Catch<MissingParserException>(() => Parse<NoParser>(_settings, "test"));
        }
        [Test]
        public void MessageParserDefined()
        {
            Assert.AreEqual("test", Parse<HasMessageParser>(_settings, "test").Value.Text);
        }
        [Test]
        public void ParserDefined()
        {
            Assert.AreEqual("test", Parse<HasParser>(_settings, "test").Value.Text);
        }

        [Test]
        public void ParseNullables()
        {
            Assert.AreEqual(null, Parse<int?>(_settings, (string)null).Value);
            Assert.AreEqual(null, Parse<int?>(_settings, "").Value);
            Assert.AreEqual(null, Parse<int?>(_settings, "    ").Value);
            Assert.AreEqual(23, Parse<int?>(_settings, "23").Value);
            Assert.AreEqual(new int?[] { 14, 18, null, 2 }, Parse<int?[]>(_settings, new string[] { "14", "18", null, "2" }).Value);
        }
    }
}

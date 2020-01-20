using CommandLineParsing.Parsing;
using CommandLineParsing.Tests.Setup;
using NUnit.Framework;
using System;

namespace CommandLineParsing.Tests.Parsing
{
    [TestFixture]
    public class ParserLookupTests
    {
        private TestingConsole _console;
        private ParserSettings _settings, _settingsIgnoreCase;

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

            public static Message MessageTryParse(string text, out HasMessageParser result)
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
        public void CreateConsole()
        {
            _console = new TestingConsole();
            ColorConsole.ActiveConsole = _console;

            _settings = new ParserSettings
            {
                EnumIgnoreCase = false,
                MultipleValuesMessage = "MULTI",
                NoValueMessage = "NO",
                TypeErrorMessage = x => $"TYPE[{x}]",
                UseParserMessage = false
            };
            _settingsIgnoreCase = new ParserSettings
            {
                EnumIgnoreCase = true,
                MultipleValuesMessage = "MULTI",
                NoValueMessage = "NO",
                TypeErrorMessage = x => $"TYPE[{x}]",
                UseParserMessage = false
            };
        }

        [Test]
        public void ParseBasicTypes()
        {
            System.Globalization.CultureInfo.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

            Assert.AreEqual(5d, ParserLookup.Parse<double>(_settings, "5.0"));
            Assert.AreEqual(5f, ParserLookup.Parse<float>(_settings, "5.0"));
            Assert.AreEqual(14, ParserLookup.Parse<int>(_settings, "14"));
            Assert.AreEqual("hello", ParserLookup.Parse<string>(_settings, "hello"));
            Assert.AreEqual(new DateTime(2017, 11, 13), ParserLookup.Parse<DateTime>(_settings, "2017-11-13"));
        }

        [Test]
        public void ParseEnumCaseTypes()
        {
            Assert.Catch<ParsingFailedException>(() => ParserLookup.Parse<ConsoleColor>(_settings, "red"));
            Assert.Catch<ParsingFailedException>(() => ParserLookup.Parse<ConsoleColor>(_settings, "rEd"));
            Assert.AreEqual(ConsoleColor.Red, ParserLookup.Parse<ConsoleColor>(_settings, "Red"));
            Assert.Catch<ParsingFailedException>(() => ParserLookup.Parse<ConsoleColor>(_settings, "reds"));
        }
        [Test]
        public void ParseEnumNoCaseTypes()
        {
            Assert.AreEqual(ConsoleColor.Red, ParserLookup.Parse<ConsoleColor>(_settingsIgnoreCase, "red"));
            Assert.AreEqual(ConsoleColor.Red, ParserLookup.Parse<ConsoleColor>(_settingsIgnoreCase, "rEd"));
            Assert.AreEqual(ConsoleColor.Red, ParserLookup.Parse<ConsoleColor>(_settingsIgnoreCase, "Red"));
            Assert.Catch<ParsingFailedException>(() => ParserLookup.Parse<ConsoleColor>(_settingsIgnoreCase, "reds"));
        }

        [Test]
        public void ParseArray()
        {
            Assert.AreEqual(14, ParserLookup.Parse<int>(_settings, new string[] { "14" }));
            Assert.AreEqual(new[] { 14 }, ParserLookup.Parse<int[]>(_settings, new string[] { "14" }));
            Assert.AreEqual(new[] { 14, 18 }, ParserLookup.Parse<int[]>(_settings, new string[] { "14", "18" }));
        }

        [Test]
        public void MissingParserExceptionThrow()
        {
            Assert.Catch<MissingParserException>(() => ParserLookup.Parse<NoParser>(_settings, "test"));
        }
        [Test]
        public void MessageParserDefined()
        {
            Assert.AreEqual("test", ParserLookup.Parse<HasMessageParser>(_settings, "test")?.Text);
        }
        [Test]
        public void ParserDefined()
        {
            Assert.AreEqual("test", ParserLookup.Parse<HasParser>(_settings, "test")?.Text);
        }

        [Test]
        public void ParseNullables()
        {
            Assert.AreEqual(null, ParserLookup.Parse<int?>(_settings, (string)null));
            Assert.AreEqual(null, ParserLookup.Parse<int?>(_settings, ""));
            Assert.AreEqual(null, ParserLookup.Parse<int?>(_settings, "    "));
            Assert.AreEqual(23, ParserLookup.Parse<int?>(_settings, "23"));
            Assert.AreEqual(new int?[] { 14, 18, null, 2 }, ParserLookup.Parse<int?[]>(_settings, new string[] { "14", "18", null, "2" }));
        }
    }
}

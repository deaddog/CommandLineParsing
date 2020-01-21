using CommandLineParsing.Tests.Setup;
using NUnit.Framework;
using System;
using System.Linq;

namespace CommandLineParsing.Tests
{
    public class ConsoleAssertions
    {
        private readonly TestingConsole _console;

        public ConsoleAssertions(TestingConsole console)
        {
            _console = console ?? throw new ArgumentNullException(nameof(console));
        }

        private TestingConsoleString[] GetState(AssertStrings strings)
        {
            return strings switch
            {
                AssertStrings.Buffer => _console.BufferStrings,
                AssertStrings.Window => _console.WindowStrings,
                _ => throw new ArgumentOutOfRangeException(nameof(strings))
            };
        }

        public void LineCount(int expectedCount, AssertStrings strings = AssertStrings.Buffer)
        {
            var state = GetState(strings);

            Assert.AreEqual(expectedCount, state.Length, "Number of lines in console ({0}) did not match expected ({1}).", state.Length, expectedCount);
        }

        public void Line(int lineIndex, string text, int startIndex = 0, AssertStrings strings = AssertStrings.Buffer)
        {
            var state = GetState(strings);
            var str = state.FirstOrDefault(x => x.Position.Top == lineIndex);

            if (str == null)
                Assert.Fail("The console did not contain line index {0}.", lineIndex);
            else
            {
                var diff = text.Length - text.TrimStart(' ').Length;
                if (diff != 0)
                {
                    text = text.Substring(diff);
                    startIndex += diff;
                }

                Assert.AreEqual(startIndex, str.Position.Left);
                Assert.AreEqual(text, str.Text);
            }
        }
    }
}

using CommandLineParsing.Output;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CommandLineParsing.Tests
{
    public class ConsoleAssertions
    {
        private readonly TestComponents.TestingConsole _console;

        public ConsoleAssertions(TestComponents.TestingConsole console)
        {
            _console = console ?? throw new ArgumentNullException(nameof(console));
        }

        private TestComponents.ConsoleString[] GetState(AssertStrings strings)
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

            Assert.AreEqual(expectedCount, state.Length, "Number of lines in console {2} ({0}) did not match expected ({1}).", state.Length, expectedCount, strings.ToString().ToLower());
        }

        public void Line(int lineIndex, ConsoleString expected, AssertStrings strings = AssertStrings.Buffer)
        {
            var state = GetState(strings);
            var str = state.FirstOrDefault(x => x.Position.Top == lineIndex);

            if (str == null)
                Assert.Fail("The console {1} did not contain line index {0}.", lineIndex, strings.ToString().ToLower());
            else
            {
                Assert.AreEqual(0, str.Position.Left);
                Assert.AreEqual(expected, str);
            }
        }
        public void Lines(IEnumerable<ConsoleString> expected, AssertStrings strings = AssertStrings.Buffer)
        {
            var lines = expected.ToArray();
            LineCount(lines.Length, strings);

            for (int l = 0; l < lines.Length; l++)
                Line(l, lines[l], strings);
        }

        public void BufferAndWindowLines(params ConsoleString[] expected)
        {
            BufferLines(expected);
            WindowLines(expected);
        }
        public void BufferLines(params ConsoleString[] expected)
        {
            Lines(expected, AssertStrings.Buffer);
        }
        public void WindowLines(params ConsoleString[] expected)
        {
            Lines(expected, AssertStrings.Window);
        }
    }
}

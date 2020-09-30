using CommandLineParsing.Input.Reading;
using NUnit.Framework;
using System;

namespace CommandLineParsing.Tests.Input
{
    [TestFixture]
    public class ReadLineTests : ConsoleTestBase
    {
        [Test]
        public void NoCleanup()
        {
            Console.Input.Enqueue("12");
            Console.Input.Enqueue(ConsoleKey.Enter);

            var value = ReadlineConfiguration.Create<int>()
                    .WithPrompt("My prompt: ")
                    .WithCleanup(cleanup: ReadlineCleanup.None)
                    .Read(Console);

            Assert.AreEqual(12, value);
            Assert.AreEqual(new ConsolePoint(0, 1), Console.GetCursorPosition());

            AssertConsole.BufferAndWindowLines("My prompt: 12");
        }

        [Test]
        public void PromptCleanup()
        {
            Console.Input.Enqueue("12");
            Console.Input.Enqueue(ConsoleKey.Enter);

            var value = ReadlineConfiguration.Create<int>()
                    .WithPrompt("My prompt: ")
                    .WithCleanup(cleanup: ReadlineCleanup.RemovePrompt)
                    .Read(Console);

            Assert.AreEqual(12, value);
            Assert.AreEqual(new ConsolePoint(0, 1), Console.GetCursorPosition());

            AssertConsole.BufferAndWindowLines("12");
        }

        [Test]
        public void FullCleanup()
        {
            Console.Input.Enqueue("12");
            Console.Input.Enqueue(ConsoleKey.Enter);

            var value = ReadlineConfiguration.Create<int>()
                    .WithPrompt("My prompt: ")
                    .WithCleanup(cleanup: ReadlineCleanup.RemoveAll)
                    .Read(Console);

            Assert.AreEqual(12, value);
            Assert.AreEqual(new ConsolePoint(0, 0), Console.GetCursorPosition());

            AssertConsole.BufferAndWindowLines();
        }

        [Test]
        public void BarelyOverflow()
        {
            var width = Console.WindowWidth;
            var word = "abcde";
            var line = string.Join("", System.Linq.Enumerable.Repeat(word, width / word.Length + 1)).Substring(0, width);

            Console.Input.Enqueue(line);
            Console.Input.Enqueue(ConsoleKey.Enter);

            var value = ReadlineConfiguration.Create<string>()
                    .Read(Console);

            AssertConsole.BufferAndWindowLines
            (
                line
            );
        }

        [Test]
        public void Overflow()
        {
            var width = Console.WindowWidth;
            var word = "abcde";
            var line = string.Join("", System.Linq.Enumerable.Repeat(word, width / word.Length + 1)).Substring(0, width);

            Console.Input.Enqueue(line + "123");
            Console.Input.Enqueue(ConsoleKey.Enter);

            var value = ReadlineConfiguration.Create<string>()
                    .Read(Console);

            AssertConsole.BufferAndWindowLines
            (
                line,
                "123"
            );
        }
    }
}

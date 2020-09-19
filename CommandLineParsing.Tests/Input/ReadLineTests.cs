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
                    .WithCleanup(cleanup: ReadLineCleanup.None)
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
                    .WithCleanup(cleanup: ReadLineCleanup.RemovePrompt)
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
                    .WithCleanup(cleanup: ReadLineCleanup.RemoveAll)
                    .Read(Console);

            Assert.AreEqual(12, value);
            Assert.AreEqual(new ConsolePoint(0, 0), Console.GetCursorPosition());

            AssertConsole.BufferAndWindowLines();
        }
    }
}

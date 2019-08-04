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

            var value = Console.ReadLine<int>("My prompt: ", cleanup: ReadLineCleanup.None);

            Assert.AreEqual(12, value);
            Assert.AreEqual(new ConsolePoint(0, 1), Console.GetCursorPosition());

            AssertConsole.BufferAndWindowLines("My prompt: 12");
        }

        [Test]
        public void PromptCleanup()
        {
            Console.Input.Enqueue("12");
            Console.Input.Enqueue(ConsoleKey.Enter);

            var value = Console.ReadLine<int>("My prompt: ", cleanup: ReadLineCleanup.RemovePrompt);

            Assert.AreEqual(12, value);
            Assert.AreEqual(new ConsolePoint(0, 1), Console.GetCursorPosition());

            AssertConsole.BufferAndWindowLines("12");
        }

        [Test]
        public void FullCleanup()
        {
            Console.Input.Enqueue("12");
            Console.Input.Enqueue(ConsoleKey.Enter);

            var value = Console.ReadLine<int>("My prompt: ", cleanup: ReadLineCleanup.RemoveAll);

            Assert.AreEqual(12, value);
            Assert.AreEqual(new ConsolePoint(0, 0), Console.GetCursorPosition());

            AssertConsole.BufferAndWindowLines();
        }
    }
}

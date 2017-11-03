using CommandLineParsing.Input;
using NUnit.Framework;
using System;

namespace CommandLineParsing.Tests.Input
{
    [TestFixture]
    public class MenuDisplayTests
    {
        private TestingConsole console;

        [SetUp]
        public void CreateConsole()
        {
            console = new TestingConsole();
            ColorConsole.ActiveConsole = console;
        }

        [Test]
        public void TopBottomPrefixAdding()
        {
            Console.CursorVisible = false;
            var display = new MenuDisplay<MenuOption<string>>(10);

            display.PrefixesTop.SetKeys(new char[] { '0', '1' });
            display.PrefixesBottom.SetKeys(new char[] { 'a', 'b' });
            
            Assert.AreEqual(0, console.BufferStrings.Length);

            display.Options.Add(new MenuOption<string>("first", "first"));
            Assert.AreEqual(1, console.BufferStrings.Length);
            console.BufferStrings.AssertLine(0, "  a: first");

            display.Options.Add(new MenuOption<string>("second", "second"));
            Assert.AreEqual(2, console.BufferStrings.Length);
            console.BufferStrings.AssertLine(0, "  b: first");
            console.BufferStrings.AssertLine(1, "  a: second");

            display.Options.Add(new MenuOption<string>("third", "third"));
            Assert.AreEqual(3, console.BufferStrings.Length);
            console.BufferStrings.AssertLine(0, "  0: first");
            console.BufferStrings.AssertLine(1, "  b: second");
            console.BufferStrings.AssertLine(2, "  a: third");

            display.Options.Add(new MenuOption<string>("fourth", "fourth"));
            Assert.AreEqual(4, console.BufferStrings.Length);
            console.BufferStrings.AssertLine(0, "  0: first");
            console.BufferStrings.AssertLine(1, "  1: second");
            console.BufferStrings.AssertLine(2, "  b: third");
            console.BufferStrings.AssertLine(3, "  a: fourth");

            display.Options.Add(new MenuOption<string>("fifth", "fifth"));
            Assert.AreEqual(5, console.BufferStrings.Length);
            console.BufferStrings.AssertLine(0, "  0: first");
            console.BufferStrings.AssertLine(1, "  1: second");
            console.BufferStrings.AssertLine(2, "     third");
            console.BufferStrings.AssertLine(3, "  b: fourth");
            console.BufferStrings.AssertLine(4, "  a: fifth");
        }

        [Test]
        public void TopBottomPrefixRemoving()
        {
            Console.CursorVisible = false;
            var display = new MenuDisplay<MenuOption<string>>(10);

            display.PrefixesTop.SetKeys(new char[] { '0', '1' });
            display.PrefixesBottom.SetKeys(new char[] { 'a', 'b' });

            display.Options.Add(new MenuOption<string>("first", "first"));
            display.Options.Add(new MenuOption<string>("second", "second"));
            display.Options.Add(new MenuOption<string>("third", "third"));
            display.Options.Add(new MenuOption<string>("fourth", "fourth"));
            display.Options.Add(new MenuOption<string>("fifth", "fifth"));

            Assert.AreEqual(5, console.BufferStrings.Length);
            console.BufferStrings.AssertLine(0, "  0: first");
            console.BufferStrings.AssertLine(1, "  1: second");
            console.BufferStrings.AssertLine(2, "     third");
            console.BufferStrings.AssertLine(3, "  b: fourth");
            console.BufferStrings.AssertLine(4, "  a: fifth");

            display.Options.RemoveAt(4);
            Assert.AreEqual(4, console.BufferStrings.Length);
            console.BufferStrings.AssertLine(0, "  0: first");
            console.BufferStrings.AssertLine(1, "  1: second");
            console.BufferStrings.AssertLine(2, "  b: third");
            console.BufferStrings.AssertLine(3, "  a: fourth");

            display.Options.RemoveAt(3);
            Assert.AreEqual(3, console.BufferStrings.Length);
            console.BufferStrings.AssertLine(0, "  0: first");
            console.BufferStrings.AssertLine(1, "  b: second");
            console.BufferStrings.AssertLine(2, "  a: third");

            display.Options.RemoveAt(2);
            Assert.AreEqual(2, console.BufferStrings.Length);
            console.BufferStrings.AssertLine(0, "  b: first");
            console.BufferStrings.AssertLine(1, "  a: second");

            display.Options.RemoveAt(1);
            Assert.AreEqual(1, console.BufferStrings.Length);
            console.BufferStrings.AssertLine(0, "  a: first");
            
            display.Options.RemoveAt(0);
            Assert.AreEqual(0, console.BufferStrings.Length);
        }

        [Test]
        public void ScrollbackCleanUp()
        {
            console.WindowHeight = 5;

            ColorConsole.WriteLine("Line 1");
            ColorConsole.WriteLine("Line 2");

            var display = new MenuDisplay<MenuOption<string>>(5);
            display.Cleanup = InputCleanup.Clean;
            display.Options.Add(new MenuOption<string>("first", "first"));
            display.Options.Add(new MenuOption<string>("second", "second"));
            display.Options.Add(new MenuOption<string>("third", "third"));
            display.Options.Add(new MenuOption<string>("fourth", "fourth"));
            display.Options.Add(new MenuOption<string>("fifth", "fifth"));
            display.Dispose();

            Assert.AreEqual(2, console.WindowStrings.Length);
            console.BufferStrings.AssertLine(0, "Line 1");
            console.BufferStrings.AssertLine(1, "Line 2");
        }

        [Test]
        public void ReducedDisplayLines()
        {
            var dis = new MenuDisplay<MenuOption<string>>(2);
            dis.Options.Add(new MenuOption<string>("first", "first"));
            dis.Options.Add(new MenuOption<string>("second", "second"));
            dis.Options.Add(new MenuOption<string>("third", "third"));

            Assert.AreEqual(2, console.BufferStrings.Length);
            console.BufferStrings.AssertLine(0, "  first");
            console.BufferStrings.AssertLine(1, "  second");

            dis.HandleKey(new ConsoleKeyInfo('\0', ConsoleKey.UpArrow, false, false, false));

            Assert.AreEqual(2, console.BufferStrings.Length);
            console.BufferStrings.AssertLine(0, "  second");
            console.BufferStrings.AssertLine(1, "> third");

            dis.Options.RemoveAt(0);

            Assert.AreEqual(2, console.BufferStrings.Length);
            console.BufferStrings.AssertLine(0, "  second");
            console.BufferStrings.AssertLine(1, "> third");

            dis.Options.RemoveAt(0);

            Assert.AreEqual(1, console.BufferStrings.Length);
            console.BufferStrings.AssertLine(0, "> third");
        }

        [Test]
        public void ChangeToLongerPrompt()
        {
            var dis = new MenuDisplay<MenuOption<string>>(3);
            dis.Options.Add(new MenuOption<string>("first", "first"));
            dis.Options.Add(new MenuOption<string>("second", "second"));
            dis.Options.Add(new MenuOption<string>("third", "third"));

            dis.HandleKey(new ConsoleKeyInfo('\0', ConsoleKey.DownArrow, false, false, false));

            Assert.AreEqual(3, console.BufferStrings.Length);
            console.BufferStrings.AssertLine(0, "> first");
            console.BufferStrings.AssertLine(1, "  second");
            console.BufferStrings.AssertLine(2, "  third");

            dis.Prompt = ">> ";

            Assert.AreEqual(3, console.BufferStrings.Length);
            console.BufferStrings.AssertLine(0, ">> first");
            console.BufferStrings.AssertLine(1, "   second");
            console.BufferStrings.AssertLine(2, "   third");
        }

        [Test]
        public void ChangeToShorterPrompt()
        {
            var dis = new MenuDisplay<MenuOption<string>>(3);
            dis.Options.Add(new MenuOption<string>("first", "first"));
            dis.Options.Add(new MenuOption<string>("second", "second"));
            dis.Options.Add(new MenuOption<string>("third", "third"));

            dis.HandleKey(new ConsoleKeyInfo('\0', ConsoleKey.DownArrow, false, false, false));

            Assert.AreEqual(3, console.BufferStrings.Length);
            console.BufferStrings.AssertLine(0, "> first");
            console.BufferStrings.AssertLine(1, "  second");
            console.BufferStrings.AssertLine(2, "  third");

            dis.Prompt = ">";

            Assert.AreEqual(3, console.BufferStrings.Length);
            console.BufferStrings.AssertLine(0, ">first");
            console.BufferStrings.AssertLine(1, " second");
            console.BufferStrings.AssertLine(2, " third");
        }
    }
}

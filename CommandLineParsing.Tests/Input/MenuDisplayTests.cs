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
    }
}

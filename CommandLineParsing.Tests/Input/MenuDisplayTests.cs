using CommandLineParsing.Input;
using CommandLineParsing.Tests.Setup;
using NUnit.Framework;
using System;

namespace CommandLineParsing.Tests.Input
{
    [TestFixture]
    public class MenuDisplayTests : ConsoleTestBase
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
            var display = new MenuDisplay<MenuOption<string>>(10);

            display.PrefixesTop.SetKeys(new char[] { '0', '1' });
            display.PrefixesBottom.SetKeys(new char[] { 'a', 'b' });

            AssertConsole.LineCount(0);

            display.Options.Add(new MenuOption<string>("first", "first"));
            AssertConsole.LineCount(1);
            AssertConsole.Line(0, "  a: first");

            display.Options.Add(new MenuOption<string>("second", "second"));
            AssertConsole.LineCount(2);
            AssertConsole.Line(0, "  b: first");
            AssertConsole.Line(1, "  a: second");

            display.Options.Add(new MenuOption<string>("third", "third"));
            AssertConsole.LineCount(3);
            AssertConsole.Line(0, "  0: first");
            AssertConsole.Line(1, "  b: second");
            AssertConsole.Line(2, "  a: third");

            display.Options.Add(new MenuOption<string>("fourth", "fourth"));
            AssertConsole.LineCount(4);
            AssertConsole.Line(0, "  0: first");
            AssertConsole.Line(1, "  1: second");
            AssertConsole.Line(2, "  b: third");
            AssertConsole.Line(3, "  a: fourth");

            display.Options.Add(new MenuOption<string>("fifth", "fifth"));
            AssertConsole.LineCount(5);
            AssertConsole.Line(0, "  0: first");
            AssertConsole.Line(1, "  1: second");
            AssertConsole.Line(2, "     third");
            AssertConsole.Line(3, "  b: fourth");
            AssertConsole.Line(4, "  a: fifth");
        }

        [Test]
        public void TopBottomPrefixRemoving()
        {
            var display = new MenuDisplay<MenuOption<string>>(10);

            display.PrefixesTop.SetKeys(new char[] { '0', '1' });
            display.PrefixesBottom.SetKeys(new char[] { 'a', 'b' });

            display.Options.Add(new MenuOption<string>("first", "first"));
            display.Options.Add(new MenuOption<string>("second", "second"));
            display.Options.Add(new MenuOption<string>("third", "third"));
            display.Options.Add(new MenuOption<string>("fourth", "fourth"));
            display.Options.Add(new MenuOption<string>("fifth", "fifth"));

            AssertConsole.LineCount(5);
            AssertConsole.Line(0, "  0: first");
            AssertConsole.Line(1, "  1: second");
            AssertConsole.Line(2, "     third");
            AssertConsole.Line(3, "  b: fourth");
            AssertConsole.Line(4, "  a: fifth");

            display.Options.RemoveAt(4);
            AssertConsole.LineCount(4);
            AssertConsole.Line(0, "  0: first");
            AssertConsole.Line(1, "  1: second");
            AssertConsole.Line(2, "  b: third");
            AssertConsole.Line(3, "  a: fourth");

            display.Options.RemoveAt(3);
            AssertConsole.LineCount(3);
            AssertConsole.Line(0, "  0: first");
            AssertConsole.Line(1, "  b: second");
            AssertConsole.Line(2, "  a: third");

            display.Options.RemoveAt(2);
            AssertConsole.LineCount(2);
            AssertConsole.Line(0, "  b: first");
            AssertConsole.Line(1, "  a: second");

            display.Options.RemoveAt(1);
            AssertConsole.LineCount(1);
            AssertConsole.Line(0, "  a: first");

            display.Options.RemoveAt(0);
            AssertConsole.LineCount(0);
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

            AssertConsole.LineCount(2, AssertStrings.Window);
            AssertConsole.Line(0, "Line 1");
            AssertConsole.Line(1, "Line 2");
        }

        [Test]
        public void ReducedDisplayLines()
        {
            var dis = new MenuDisplay<MenuOption<string>>(2);
            dis.Options.Add(new MenuOption<string>("first", "first"));
            dis.Options.Add(new MenuOption<string>("second", "second"));
            dis.Options.Add(new MenuOption<string>("third", "third"));

            AssertConsole.LineCount(2);
            AssertConsole.Line(0, "  first");
            AssertConsole.Line(1, "  second");

            dis.HandleKey(new ConsoleKeyInfo('\0', ConsoleKey.UpArrow, false, false, false));

            AssertConsole.LineCount(2);
            AssertConsole.Line(0, "  second");
            AssertConsole.Line(1, "> third");

            dis.Options.RemoveAt(0);

            AssertConsole.LineCount(2);
            AssertConsole.Line(0, "  second");
            AssertConsole.Line(1, "> third");

            dis.Options.RemoveAt(0);

            AssertConsole.LineCount(1);
            AssertConsole.Line(0, "> third");
        }

        [Test]
        public void ChangeToLongerPrompt()
        {
            var dis = new MenuDisplay<MenuOption<string>>(3);
            dis.Options.Add(new MenuOption<string>("first", "first"));
            dis.Options.Add(new MenuOption<string>("second", "second"));
            dis.Options.Add(new MenuOption<string>("third", "third"));

            dis.HandleKey(new ConsoleKeyInfo('\0', ConsoleKey.DownArrow, false, false, false));

            AssertConsole.LineCount(3);
            AssertConsole.Line(0, "> first");
            AssertConsole.Line(1, "  second");
            AssertConsole.Line(2, "  third");

            dis.Prompt = ">> ";

            AssertConsole.LineCount(3);
            AssertConsole.Line(0, ">> first");
            AssertConsole.Line(1, "   second");
            AssertConsole.Line(2, "   third");
        }

        [Test]
        public void ChangeToShorterPrompt()
        {
            var dis = new MenuDisplay<MenuOption<string>>(3);
            dis.Options.Add(new MenuOption<string>("first", "first"));
            dis.Options.Add(new MenuOption<string>("second", "second"));
            dis.Options.Add(new MenuOption<string>("third", "third"));

            dis.HandleKey(new ConsoleKeyInfo('\0', ConsoleKey.DownArrow, false, false, false));

            AssertConsole.LineCount(3);
            AssertConsole.Line(0, "> first");
            AssertConsole.Line(1, "  second");
            AssertConsole.Line(2, "  third");

            dis.Prompt = ">";

            AssertConsole.LineCount(3);
            AssertConsole.Line(0, ">first");
            AssertConsole.Line(1, " second");
            AssertConsole.Line(2, " third");
        }
    }
}

using CommandLineParsing.Input;
using NUnit.Framework;
using System;

namespace CommandLineParsing.Tests.Input
{
    [TestFixture]
    public class MenuDisplayTests : ConsoleTestBase
    {
        [Test]
        public void TopBottomPrefixAdding()
        {
            var display = new MenuDisplay<MenuOption<string>>(10);

            display.PrefixesTop.SetKeys(new char[] { '0', '1' });
            display.PrefixesBottom.SetKeys(new char[] { 'a', 'b' });

            AssertConsole.LineCount(0);

            display.Options.Add(new MenuOption<string>("first", "first"));
            AssertConsole.BufferAndWindowLines
            (
                "  a: first"
            );

            display.Options.Add(new MenuOption<string>("second", "second"));
            AssertConsole.BufferAndWindowLines
            (
                "  b: first",
                "  a: second"
            );

            display.Options.Add(new MenuOption<string>("third", "third"));
            AssertConsole.BufferAndWindowLines
            (
                "  0: first",
                "  b: second",
                "  a: third"
            );

            display.Options.Add(new MenuOption<string>("fourth", "fourth"));
            AssertConsole.BufferAndWindowLines
            (
                "  0: first",
                "  1: second",
                "  b: third",
                "  a: fourth"
            );

            display.Options.Add(new MenuOption<string>("fifth", "fifth"));
            AssertConsole.BufferAndWindowLines
            (
                "  0: first",
                "  1: second",
                "     third",
                "  b: fourth",
                "  a: fifth"
            );
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

            AssertConsole.BufferAndWindowLines
            (
                "  0: first",
                "  1: second",
                "     third",
                "  b: fourth",
                "  a: fifth"
            );

            display.Options.RemoveAt(4);
            AssertConsole.BufferAndWindowLines
            (
                "  0: first",
                "  1: second",
                "  b: third",
                "  a: fourth"
            );

            display.Options.RemoveAt(3);
            AssertConsole.BufferAndWindowLines
            (
                "  0: first",
                "  b: second",
                "  a: third"
            );

            display.Options.RemoveAt(2);
            AssertConsole.BufferAndWindowLines
            (
                "  b: first",
                "  a: second"
            );

            display.Options.RemoveAt(1);
            AssertConsole.BufferAndWindowLines
            (
                "  a: first"
            );

            display.Options.RemoveAt(0);
            AssertConsole.BufferAndWindowLines();
        }

        [Test]
        public void ScrollbackCleanUp()
        {
            Console.WindowHeight = 5;

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

            AssertConsole.BufferAndWindowLines
            (
                "Line 1",
                "Line 2"
            );
        }

        [Test]
        public void ReducedDisplayLines()
        {
            var dis = new MenuDisplay<MenuOption<string>>(2);
            dis.Options.Add(new MenuOption<string>("first", "first"));
            dis.Options.Add(new MenuOption<string>("second", "second"));
            dis.Options.Add(new MenuOption<string>("third", "third"));

            AssertConsole.BufferAndWindowLines
            (
                "  first",
                "  second"
            );

            dis.HandleKey(new ConsoleKeyInfo('\0', ConsoleKey.UpArrow, false, false, false));

            AssertConsole.BufferAndWindowLines
            (
                "  second",
                "> third"
            );

            dis.Options.RemoveAt(0);

            AssertConsole.BufferAndWindowLines
            (
                "  second",
                "> third"
            );

            dis.Options.RemoveAt(0);

            AssertConsole.BufferAndWindowLines
            (
                "> third"
            );
        }

        [Test]
        public void ChangeToLongerPrompt()
        {
            var dis = new MenuDisplay<MenuOption<string>>(3);
            dis.Options.Add(new MenuOption<string>("first", "first"));
            dis.Options.Add(new MenuOption<string>("second", "second"));
            dis.Options.Add(new MenuOption<string>("third", "third"));

            dis.HandleKey(new ConsoleKeyInfo('\0', ConsoleKey.DownArrow, false, false, false));

            AssertConsole.BufferAndWindowLines
            (
                "> first",
                "  second",
                "  third"
            );

            dis.Prompt = ">> ";

            AssertConsole.BufferAndWindowLines
            (
                ">> first",
                "   second",
                "   third"
            );
        }

        [Test]
        public void ChangeToShorterPrompt()
        {
            var dis = new MenuDisplay<MenuOption<string>>(3);
            dis.Options.Add(new MenuOption<string>("first", "first"));
            dis.Options.Add(new MenuOption<string>("second", "second"));
            dis.Options.Add(new MenuOption<string>("third", "third"));

            dis.HandleKey(new ConsoleKeyInfo('\0', ConsoleKey.DownArrow, false, false, false));

            AssertConsole.BufferAndWindowLines
            (
                "> first",
                "  second",
                "  third"
            );

            dis.Prompt = ">";

            AssertConsole.BufferAndWindowLines
            (
                ">first",
                " second",
                " third"
            );
        }
    }
}

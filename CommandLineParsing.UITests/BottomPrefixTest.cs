using CommandLineParsing.Input;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandLineParsing.UITests
{
    public class BottomPrefixTest : UITest
    {
        public override void Execute()
        {
            Console.WriteLine("Prefixes should start from bottom (a, b, c) and then from top (0, 1, 2).");
            Console.WriteLine("Use Y and N to determine if the current state is correct:");

            var options = new Queue<string>(new string[] { "First", "Second", "Third", "Fourth", "Fifth", "Sixth", "Seventh", "Eight" });

            Console.CursorVisible = false;
            var display = new MenuDisplay<MenuOption<string>>(10);

            display.PrefixesTop.SetKeys(new char[] { '0', '1', '2' });
            display.PrefixesBottom.SetKeys(new char[] { 'a', 'b', 'c' });

            while (options.Count > 0)
            {
                var o = options.Dequeue() + " option";
                display.Options.Add(new MenuOption<string>(o, o));

                Assert.IsTrue(GetYesNo(display), $"Menu displayed did not look correct after {display.Options.Count} items");
            }

            display.Cleanup = InputCleanup.None;
            display.Dispose();
        }

        static bool GetYesNo(IConsoleInput input)
        {
            var position = ColorConsole.CursorPosition;
            var key = Console.ReadKey(true);

            while (key.Key != ConsoleKey.Y && key.Key != ConsoleKey.N)
            {
                if (key.Key == ConsoleKey.UpArrow || key.Key == ConsoleKey.DownArrow)
                    input.HandleKey(key);

                key = Console.ReadKey(true);
            }

            return key.Key == ConsoleKey.Y;
        }
    }
}

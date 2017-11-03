using CommandLineParsing.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CommandLineParsing.UITests
{
    class Program
    {
        static readonly SharedConsole _console = new SharedConsole();

        static void Main(string[] args)
        {
            // Setting the active console to the shared one, use _console.BufferStrings and _console.WindowStrings to inspect the state.
            ColorConsole.ActiveConsole = _console;

            // Do stuff using the console, by interacting with ColorConsole.
        }


        static string SelectCategoryIdFromMenu(IEnumerable<string> names, string initialSearch = null)
        {
            var allOptions = new List<MenuOption<string>>();
            allOptions.AddRange(names.Select(x => new MenuOption<string>(x, x)));

            using (var menu = new MenuDisplay<MenuOption<string>>(new ConsoleSize(0, 2), Console.WindowHeight - 2))
            using (var reader = new ConsoleReader("Select a category: "))
            {
                reader.Cleanup = InputCleanup.Clean;
                menu.Cleanup = InputCleanup.Clean;
                //menu.PrefixesTop.SetKeys(MenuLabeling.Letters);
                menu.Options.Add(new MenuOption<string>("[cyan:Something...]", null));
                foreach (var o in allOptions)
                    menu.Options.Add(o);

                reader.TextChanged += (r, old) =>
                {
                    var cmp = r.Text.ToLowerInvariant().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    menu.UpdateOptions = false;

                    int j = 1;
                    for (int i = 0; i < allOptions.Count; i++)
                    {
                        var itemText = allOptions[i].Text.Content.ToLowerInvariant();
                        var include = cmp.All(x => itemText.Contains(x));
                        if (j >= menu.Options.Count)
                        {
                            if (include)
                            {
                                menu.Options.Add(allOptions[i]);
                                j++;
                            }
                        }
                        else if (allOptions[i] == menu.Options[j])
                        {
                            if (!include)
                                menu.Options.RemoveAt(j);
                            else
                                j++;
                        }
                        else
                        {
                            if (include)
                                menu.Options.Insert(j++, allOptions[i]);
                        }
                    }

                    menu.UpdateOptions = true;
                };

                menu.SelectedIndex = 0;

                ConsoleKeyInfo info;
                do
                {
                    info = Console.ReadKey(true);
                    switch (info.Key)
                    {
                        case ConsoleKey.UpArrow:
                        case ConsoleKey.DownArrow:
                        case ConsoleKey.PageUp:
                        case ConsoleKey.PageDown:
                            menu.HandleKey(info);
                            break;

                        case ConsoleKey.F2:
                            menu.Prompt = "[red: >>>> ]";
                            break;
                        case ConsoleKey.F9:
                            menu.UpdateOptions = !menu.UpdateOptions;
                            break;

                        case ConsoleKey.Enter:
                            if (menu.SelectedIndex >= 0)
                                return menu.Options[menu.SelectedIndex].Value;
                            break;

                        case ConsoleKey.Delete:
                            menu.HandleKey(info);
                            break;

                        default:
                            reader.HandleKey(info);
                            break;
                    }
                } while (true);
            }
        }
    }
}

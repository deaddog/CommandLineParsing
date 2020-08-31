using CommandLineParsing.Input;
using CommandLineParsing.Output;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CommandLineParsing
{
    /// <summary>
    /// Provides additional methods for specialized menu types.
    /// </summary>
    public static class MenuExtension
    {
        /// <summary>
        /// Displays a menu where a single element can be selected from the collection.
        /// </summary>
        /// <typeparam name="T">The type of the elements in <paramref name="collection"/>.</typeparam>
        /// <param name="collection">The collection of element from which the menu should be created.</param>
        /// <param name="keySelector">A function that gets the <see cref="ConsoleString"/> that should be displayed for an item in the collection.</param>
        /// <param name="labeling">The type of labeling (option prefix) that should be applied when displaying the menu.</param>
        /// <param name="cleanup">The cleanup applied after displaying the menu.</param>
        /// <param name="cancelKey">If not <c>null</c>, this string is displayed as a "cancel option" in the bottom of the menu.
        /// <paramref name="cancelValue"/> will be returned if this option is selected.</param>
        /// <param name="cancelValue">The value returned if <paramref name="cancelKey"/> is not <c>null</c>.</param>
        /// <returns>The element that was selected using the displayed menu.</returns>
        public static T MenuSelect<T>(this IConsole console, IEnumerable<T> collection, Func<T, ConsoleString> keySelector = null, MenuLabeling labeling = MenuLabeling.NumbersAndLetters, MenuCleanup cleanup = MenuCleanup.None, ConsoleString cancelKey = null, T cancelValue = default(T))
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));
            if (!collection.Any())
                throw new ArgumentOutOfRangeException(nameof(collection), $"{nameof(MenuSelect)} can not on be executed on non-empty collections.");

            if (keySelector == null)
                keySelector = x => x.ToString();

            MenuOption<T> result = null;

            console.CursorVisible = false;

            using (var display = new MenuDisplay<MenuOption<T>>(console))
            {
                display.Cleanup = cleanup == MenuCleanup.None ? InputCleanup.None : InputCleanup.Clean;
                display.PrefixesTop.SetKeys(labeling);

                foreach (var item in collection)
                    display.Options.Add(new MenuOption<T>(keySelector(item), item));

                if (cancelKey != null || (cancelValue != null && !cancelValue.Equals(default(T))))
                {
                    if (labeling != MenuLabeling.None)
                        display.PrefixesBottom.SetKeys(new char[] { '0' });
                    if (cancelKey == null) cancelKey = keySelector(cancelValue);
                    display.Options.Add(new MenuOption<T>(cancelKey, cancelValue));
                }

                display.SelectedIndex = 0;

                bool done = false;
                ConsoleKeyInfo info;
                do
                {
                    info = console.ReadKey(true);
                    switch (info.Key)
                    {
                        case ConsoleKey.DownArrow:
                        case ConsoleKey.UpArrow:
                            display.HandleKey(info);
                            break;

                        case ConsoleKey.Enter:
                            result = display.Options[display.SelectedIndex];
                            done = true;
                            break;

                        default:
                            var index = display.IndexFromPrefix(info.KeyChar);
                            if (index >= 0)
                            {
                                display.HandleKey(info);
                                result = display.Options[index];
                                done = true;
                            }
                            break;
                    }
                } while (!done);

                if (cleanup == MenuCleanup.None)
                    console.SetCursorPosition(display.Origin + new ConsoleSize(0, display.Options.Count));
            }

            if (cleanup == MenuCleanup.RemoveMenuShowChoice)
                console.WriteLine(result.Text);

            console.CursorVisible = true;

            return result.Value;
        }
        /// <summary>
        /// Displays a menu where a single element can be selected from the collection.
        /// Displays the key part of each element in the menu and returns the selected value part.
        /// </summary>
        /// <typeparam name="TKey">The type of the Key part of elements in <paramref name="collection"/>.</typeparam>
        /// <typeparam name="TValue">The type of the Value part of elements in <paramref name="collection"/>.</typeparam>
        /// <param name="collection">The collection of elements from which the menu should be created.</param>
        /// <param name="labeling">The type of labeling (option prefix) that should be applied when displaying the menu.</param>
        /// <param name="cleanup">The cleanup applied after displaying the menu.</param>
        /// <param name="cancelKey">If not <c>null</c>, this string is displayed as a "cancel option" in the bottom of the menu.
        /// <paramref name="cancelValue"/> will be returned if this option is selected.</param>
        /// <param name="cancelValue">The value returned if <paramref name="cancelKey"/> is not <c>null</c>.</param>
        /// <returns>The element that was selected using the displayed menu.</returns>
        public static TValue MenuSelect<TKey, TValue>(IConsole console, IEnumerable<KeyValuePair<TKey, TValue>> collection, MenuLabeling labeling = MenuLabeling.NumbersAndLetters, MenuCleanup cleanup = MenuCleanup.None, ConsoleString cancelKey = null, TValue cancelValue = default(TValue))
        {
            return MenuSelect(console, collection, x => x.Key.ToString(), labeling, cleanup, cancelKey, new KeyValuePair<TKey, TValue>(default(TKey), cancelValue)).Value;
        }

        /// <summary>
        /// Displays a menu where a set of elements can be selected from the collection.
        /// </summary>
        /// <typeparam name="T">The type of the elements in <paramref name="collection"/>.</typeparam>
        /// <param name="isSelectionValid">A function that determines if the current selected collection is a valid selection. If the function returns <c>true</c> the done option is enabled.</param>
        /// <param name="collection">The collection of element from which the menu should be created.</param>
        /// <param name="onKeySelector">A function that gets the <see cref="ConsoleString"/> that should be displayed for an item in the collection, when the option is selected.</param>
        /// <param name="offKeySelector">A function that gets the <see cref="ConsoleString"/> that should be displayed for an item in the collection, when the option is not selected.</param>
        /// <param name="selected">A function that returns a boolean value indicating if an element should be pre-selected when the menu is displayed.</param>
        /// <param name="labeling">The type of labeling (option prefix) that should be applied when displaying the menu.</param>
        /// <param name="cleanup">The cleanup applied after displaying the menu.</param>
        /// <param name="doneText">The <see cref="ConsoleString"/> that is displayed as the bottommost option in the menu. Selecting this option will cause the function to return.</param>
        /// <returns>The elements that were selected using the displayed menu.</returns>
        public static T[] MenuSelectMultiple<T>(this IConsole console, IEnumerable<T> collection, Func<IEnumerable<T>, bool> isSelectionValid = null, Func<T, ConsoleString> onKeySelector = null, Func<T, ConsoleString> offKeySelector = null, Func<T, bool> selected = null, MenuLabeling labeling = MenuLabeling.NumbersAndLetters, MenuCleanup cleanup = MenuCleanup.None, ConsoleString doneText = null)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));
            var items = collection.ToArray();
            if (!items.Any())
                throw new ArgumentOutOfRangeException(nameof(collection), $"{nameof(MenuSelect)} can not on be executed on non-empty collections.");

            if (isSelectionValid == null)
                isSelectionValid = x => true;

            if (onKeySelector == null)
                onKeySelector = x => x.ToString();
            if (offKeySelector == null)
                offKeySelector = x =>
                {
                    var text = onKeySelector(x);
                    if (text.HasColors)
                        return text.ClearColors();
                    else
                        return $"[DarkGray:{text.Content}]";
                };
            if (selected == null)
                selected = x => false;

            List<MenuOnOffOption<T>> result = null;

            console.CursorVisible = false;

            using (var display = new MenuDisplay<MenuOnOffOption<T>>(console))
            {
                display.Cleanup = cleanup == MenuCleanup.None ? InputCleanup.None : InputCleanup.Clean;
                display.PrefixesTop.SetKeys(labeling);
                display.PrefixesBottom.SetKeys(new char[] { '0' });

                foreach (var item in items)
                    display.Options.Add(new MenuOnOffOption<T>(onKeySelector(item), offKeySelector(item), selected(item), item));

                if (doneText == null)
                    doneText = "Done";
                var doneOption = new MenuOnOffOption<T>(doneText, $"[DarkGray:{doneText.Content}]", true, default(T));
                display.Options.Add(doneOption);

                doneOption.On = isSelectionValid(display.Options.Where(x => x != doneOption && x.On).Select(x => x.Value));

                display.SelectedIndex = 0;

                bool done = false;
                ConsoleKeyInfo info;
                do
                {
                    info = console.ReadKey(true);
                    switch (info.Key)
                    {
                        case ConsoleKey.DownArrow:
                        case ConsoleKey.UpArrow:
                            display.HandleKey(info);
                            break;

                        case ConsoleKey.Enter:
                        default:
                            var index = info.Key == ConsoleKey.Enter ? display.SelectedIndex : display.IndexFromPrefix(info.KeyChar);

                            if (index >= 0)
                            {
                                var option = display.Options[index];
                                if (option == doneOption && doneOption.On)
                                    done = true;
                                else if (option != doneOption)
                                {
                                    option.On = !option.On;
                                    doneOption.On = isSelectionValid(display.Options.Where(x => x != doneOption && x.On).Select(x => x.Value));
                                }
                            }
                            break;
                    }
                } while (!done);

                result = new List<MenuOnOffOption<T>>(display.Options.Where(x => x != doneOption && x.On));

                if (cleanup == MenuCleanup.None)
                    console.SetCursorPosition(display.Origin + new ConsoleSize(0, display.Options.Count));
            }

            if (cleanup == MenuCleanup.RemoveMenuShowChoice)
                foreach (var r in result)
                    console.WriteLine(r.Text);

            console.CursorVisible = true;

            return result.Select(x => x.Value).ToArray();
        }
        /// <summary>
        /// Displays a menu where a set of elements can be selected from the collection.
        /// Displays the key part of each element in the menu and returns the selected value part.
        /// </summary>
        /// <typeparam name="TKey">The type of the Key part of elements in <paramref name="collection"/>.</typeparam>
        /// <typeparam name="TValue">The type of the Value part of elements in <paramref name="collection"/>.</typeparam>
        /// <param name="isSelectionValid">A function that determines if the current selected collection is a valid selection. If the function returns <c>true</c> the done option is enabled.</param>
        /// <param name="collection">The collection of elements from which the menu should be created.</param>
        /// <param name="selected">A function that returns a boolean value indicating if an element should be pre-selected when the menu is displayed.</param>
        /// <param name="labeling">The type of labeling (option prefix) that should be applied when displaying the menu.</param>
        /// <param name="cleanup">The cleanup applied after displaying the menu.</param>
        /// <param name="doneText">The <see cref="ConsoleString"/> that is displayed as the bottommost option in the menu. Selecting this option will cause the function to return.</param>
        /// <returns>The elements that were selected using the displayed menu.</returns>
        public static TValue[] MenuSelectMultiple<TKey, TValue>(this IConsole console, IEnumerable<KeyValuePair<TKey, TValue>> collection, Func<IEnumerable<TValue>, bool> isSelectionValid = null, Func<KeyValuePair<TKey, TValue>, bool> selected = null, MenuLabeling labeling = MenuLabeling.NumbersAndLetters, MenuCleanup cleanup = MenuCleanup.None, ConsoleString doneText = null)
        {
            return MenuSelectMultiple(console, collection, selection => isSelectionValid(selection.Select(x => x.Value)), null, null, selected, labeling, cleanup, doneText).Select(x => x.Value).ToArray();
        }
    }
}

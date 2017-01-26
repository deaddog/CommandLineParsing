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
        /// Displays a <see cref="Menu{T}"/> where an element from the collection can be selected.
        /// </summary>
        /// <typeparam name="T">The type of element in <paramref name="collection"/>.</typeparam>
        /// <param name="collection">The collection of element from which the <see cref="Menu{T}"/> should be created.</param>
        /// <param name="settings">A <see cref="MenuSettings"/> that expresses the settings used when displaying the menu, or <c>null</c> to use the default settings.</param>
        /// <param name="cancel">If not <c>null</c>, this string is displayed as a "cancel option" in the menu.
        /// The default value for <typeparamref name="T"/> will be returned if this option is selected.</param>
        /// <returns>The element that was selected using the displayed <see cref="Menu{T}"/>.</returns>
        public static T MenuSelect<T>(this IEnumerable<T> collection, MenuSettings settings, string cancel = null)
        {
            return MenuSelect(collection, settings, x => x.ToString(), cancel, default(T));
        }
        /// <summary>
        /// Displays a <see cref="Menu{T}"/> where an element from the collection can be selected.
        /// </summary>
        /// <typeparam name="T">The type of element in <paramref name="collection"/>.</typeparam>
        /// <param name="collection">The collection of element from which the <see cref="Menu{T}"/> should be created.</param>
        /// <param name="settings">A <see cref="MenuSettings"/> that expresses the settings used when displaying the menu, or <c>null</c> to use the default settings.</param>
        /// <param name="cancelKey">If not <c>null</c>, this string is displayed as a "cancel option" in the menu.</param>
        /// <param name="cancelValue">The value associated with the cancel option. If <paramref name="cancelKey"/> is <c>null</c> this value is ignored.</param>
        /// <returns>The element that was selected using the displayed <see cref="Menu{T}"/>.</returns>
        public static T MenuSelect<T>(this IEnumerable<T> collection, MenuSettings settings, string cancelKey, T cancelValue)
        {
            return MenuSelect(collection, settings, x => x.ToString(), cancelKey, cancelValue);
        }

        /// <summary>
        /// Displays a <see cref="Menu{T}"/> where an element from the collection can be selected.
        /// </summary>
        /// <typeparam name="T">The type of element in <paramref name="collection"/>.</typeparam>
        /// <param name="collection">The collection of element from which the <see cref="Menu{T}"/> should be created.</param>
        /// <param name="settings">A <see cref="MenuSettings"/> that expresses the settings used when displaying the menu, or <c>null</c> to use the default settings.</param>
        /// <param name="keySelector">A function that gets the <see cref="String"/> that should be displayed for an item in the collection</param>
        /// <param name="cancel">If not <c>null</c>, this string is displayed as a "cancel option" in the menu.
        /// The default value for <typeparamref name="T"/> will be returned if this option is selected.</param>
        /// <returns>The element that was selected using the displayed <see cref="Menu{T}"/>.</returns>
        public static T MenuSelect<T>(this IEnumerable<T> collection, MenuSettings settings, Func<T, string> keySelector, string cancel = null)
        {
            return MenuSelect(collection, settings, keySelector, cancel, default(T));
        }
        /// <summary>
        /// Displays a <see cref="Menu{T}"/> where an element from the collection can be selected.
        /// </summary>
        /// <typeparam name="T">The type of element in <paramref name="collection"/>.</typeparam>
        /// <param name="collection">The collection of element from which the <see cref="Menu{T}"/> should be created.</param>
        /// <param name="settings">A <see cref="MenuSettings"/> that expresses the settings used when displaying the menu, or <c>null</c> to use the default settings.</param>
        /// <param name="keySelector">A function that gets the <see cref="String"/> that should be displayed for an item in the collection</param>
        /// <param name="cancelKey">If not <c>null</c>, this string is displayed as a "cancel option" in the menu.</param>
        /// <param name="cancelValue">The value associated with the cancel option. If <paramref name="cancelKey"/> is <c>null</c> this value is ignored.</param>
        /// <returns>The element that was selected using the displayed <see cref="Menu{T}"/>.</returns>
        public static T MenuSelect<T>(this IEnumerable<T> collection, MenuSettings settings, Func<T, string> keySelector, string cancelKey, T cancelValue)
        {
            Menu<T> menu = new Menu<T>();

            foreach (var item in collection)
                menu.Add(keySelector(item), item);

            if (cancelKey != null)
                menu.SetCancel(cancelKey, cancelValue);

            return menu.ShowAndSelect(settings).Value;
        }

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
        public static T MenuSelect<T>(this IEnumerable<T> collection, Func<T, ConsoleString> keySelector = null, MenuLabeling labeling = MenuLabeling.NumbersAndLetters, MenuCleanup cleanup = MenuCleanup.None, ConsoleString cancelKey = null, T cancelValue = default(T))
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));
            if (!collection.Any())
                throw new ArgumentOutOfRangeException(nameof(collection), $"{nameof(MenuSelect)} can not on be executed on non-empty collections.");

            if (keySelector == null)
                keySelector = x => x.ToString();

            MenuOption<T> result = null;

            Console.CursorVisible = false;

            using (var display = new MenuDisplay<MenuOption<T>>())
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
                    info = Console.ReadKey(true);
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
                    ColorConsole.CursorPosition = display.Origin + new ConsoleSize(0, display.Options.Count);
            }

            if (cleanup == MenuCleanup.RemoveMenuShowChoice)
                ColorConsole.WriteLine(result.Text);

            Console.CursorVisible = true;

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
        public static TValue MenuSelect<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> collection, MenuLabeling labeling = MenuLabeling.NumbersAndLetters, MenuCleanup cleanup = MenuCleanup.None, ConsoleString cancelKey = null, TValue cancelValue = default(TValue))
        {
            return MenuSelect(collection, x => x.Key.ToString(), labeling, cleanup, cancelKey, new KeyValuePair<TKey, TValue>(default(TKey), cancelValue)).Value;
        }

        /// <summary>
        /// Displays a <see cref="Menu{T}"/> where an element from the collection can be selected.
        /// Displays the key part of each element in the menu and returns the selected value part.
        /// </summary>
        /// <typeparam name="TKey">The type of the Key part of elements in <paramref name="collection"/>.</typeparam>
        /// <typeparam name="TValue">The type of the Value part of elements in <paramref name="collection"/>.</typeparam>
        /// <param name="collection">The collection of element from which the <see cref="Menu{T}"/> should be created.</param>
        /// <param name="settings">A <see cref="MenuSettings"/> that expresses the settings used when displaying the menu, or <c>null</c> to use the default settings.</param>
        /// <param name="cancel">If not <c>null</c>, this string is displayed as a "cancel option" in the menu.
        /// The default value for <typeparamref name="TValue"/> will be returned if this option is selected.</param>
        /// <returns>The element that was selected using the displayed <see cref="Menu{T}"/>.</returns>
        public static TValue MenuSelect<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> collection, MenuSettings settings, string cancel = null)
        {
            return MenuSelect(collection, settings, cancel, default(TValue));
        }
        /// <summary>
        /// Displays a <see cref="Menu{T}"/> where an element from the collection can be selected.
        /// Displays the key part of each element in the menu and returns the selected value part.
        /// </summary>
        /// <typeparam name="TKey">The type of the Key part of elements in <paramref name="collection"/>.</typeparam>
        /// <typeparam name="TValue">The type of the Value part of elements in <paramref name="collection"/>.</typeparam>
        /// <param name="collection">The collection of element from which the <see cref="Menu{T}"/> should be created.</param>
        /// <param name="settings">A <see cref="MenuSettings"/> that expresses the settings used when displaying the menu, or <c>null</c> to use the default settings.</param>
        /// <param name="cancelKey">If not <c>null</c>, this string is displayed as a "cancel option" in the menu.</param>
        /// <param name="cancelValue">The value associated with the cancel option. If <paramref name="cancelKey"/> is <c>null</c> this value is ignored.</param>
        /// <returns>The element that was selected using the displayed <see cref="Menu{T}"/>.</returns>
        public static TValue MenuSelect<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> collection, MenuSettings settings, string cancelKey, TValue cancelValue)
        {
            return MenuSelect(collection, settings, x => x.Key.ToString(), cancelKey, new KeyValuePair<TKey, TValue>(default(TKey), cancelValue)).Value;
        }

        private class OnOffOption<T> : MenuOption<T>
        {
            private readonly ConsoleString _onText, _offText;
            private bool _on;

            public OnOffOption(ConsoleString onText, ConsoleString offText, bool on, T value)
                : base(on ? onText : offText, value)
            {
                _onText = onText;
                _offText = offText;
                _on = on;
            }

            public bool On
            {
                get { return _on; }
                set
                {
                    if (_on == value)
                        return;

                    _on = value;
                    Text = _on ? _onText : _offText;
                }
            }
        }

        /// <summary>
        /// Displays a <see cref="SelectionMenu{T}"/> where a set of elements from the collection can be selected.
        /// </summary>
        /// <typeparam name="T">The type of element in <paramref name="collection"/>.</typeparam>
        /// <param name="collection">The collection of element from which the <see cref="SelectionMenu{T}"/> should be created.</param>
        /// <param name="settings">A <see cref="MenuSettings"/> that expresses the settings used when displaying the menu, or <c>null</c> to use the default settings.</param>
        /// <param name="selected">A function that returns <c>true</c> if an element should initially be selected when the menu is displayed.</param>
        /// <returns>An array containing the elements that were selected using the displayed <see cref="SelectionMenu{T}"/>.</returns>
        public static T[] MenuSelectMultiple<T>(this IEnumerable<T> collection, MenuSettings settings, Func<T, bool> selected = null)
        {
            return MenuSelectMultiple(collection, settings, x => x.ToString(), x => null, selected);
        }
        /// <summary>
        /// Displays a <see cref="SelectionMenu{T}"/> where a set of elements from the collection can be selected.
        /// </summary>
        /// <typeparam name="T">The type of element in <paramref name="collection"/>.</typeparam>
        /// <param name="collection">The collection of element from which the <see cref="SelectionMenu{T}"/> should be created.</param>
        /// <param name="settings">A <see cref="MenuSettings"/> that expresses the settings used when displaying the menu, or <c>null</c> to use the default settings.</param>
        /// <param name="onKeySelector">A function that gets the <see cref="String"/> that should be displayed for an item when it is selected.</param>
        /// <param name="offKeySelector">A function that gets the <see cref="String"/> that should be displayed for an item when it is not selected.</param>
        /// <param name="selected">A function that returns <c>true</c> if an element should initially be selected when the menu is displayed.</param>
        /// <returns>An array containing the elements that were selected using the displayed <see cref="SelectionMenu{T}"/>.</returns>
        public static T[] MenuSelectMultiple<T>(this IEnumerable<T> collection, MenuSettings settings, Func<T, string> onKeySelector, Func<T, string> offKeySelector, Func<T, bool> selected = null)
        {
            SelectionMenu<T> menu = new SelectionMenu<T>();

            foreach (var item in collection)
                menu.Add(onKeySelector(item), offKeySelector(item), item, selected == null ? false : selected(item));

            var res = menu.ShowAndSelect(settings);
            T[] arr = new T[res.Length];
            for (int i = 0; i < res.Length; i++) arr[i] = res[i].Value;
            return arr;
        }

        /// <summary>
        /// Displays a menu where a set of elements can be selected from the collection.
        /// </summary>
        /// <typeparam name="T">The type of the elements in <paramref name="collection"/>.</typeparam>
        /// <param name="minimum">The minimum number of elements that must be selected for the function to return.</param>
        /// <param name="maximum">The maximum number of elements that must be selected for the function to return.</param>
        /// <param name="collection">The collection of element from which the menu should be created.</param>
        /// <param name="onKeySelector">A function that gets the <see cref="ConsoleString"/> that should be displayed for an item in the collection, when the option is selected.</param>
        /// <param name="offKeySelector">A function that gets the <see cref="ConsoleString"/> that should be displayed for an item in the collection, when the option is not selected.</param>
        /// <param name="selected">A function that returns a boolean value indicating if an element should be pre-selected when the menu is displayed.</param>
        /// <param name="labeling">The type of labeling (option prefix) that should be applied when displaying the menu.</param>
        /// <param name="cleanup">The cleanup applied after displaying the menu.</param>
        /// <param name="doneText">The <see cref="ConsoleString"/> that is displayed as the bottommost option in the menu. Selecting this option will cause the function to return.</param>
        /// <returns>The elements that were selected using the displayed menu.</returns>
        public static T[] MenuSelectMultiple<T>(this IEnumerable<T> collection, uint minimum = uint.MinValue, uint maximum = uint.MaxValue, Func<T, ConsoleString> onKeySelector = null, Func<T, ConsoleString> offKeySelector = null, Func<T, bool> selected = null, MenuLabeling labeling = MenuLabeling.NumbersAndLetters, MenuCleanup cleanup = MenuCleanup.None, ConsoleString doneText = null)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));
            var items = collection.ToArray();
            if (!items.Any())
                throw new ArgumentOutOfRangeException(nameof(collection), $"{nameof(MenuSelect)} can not on be executed on non-empty collections.");

            if (maximum < minimum)
                throw new ArgumentOutOfRangeException(nameof(maximum), "The maximum selectable number of elements cannot be less then the minimum.");
            if (minimum > items.Count())
                throw new ArgumentOutOfRangeException(nameof(minimum), "The minimum selectable number of elements cannot be greater than the number of elements.");

            if (onKeySelector == null)
                onKeySelector = x => x.ToString();
            if (offKeySelector == null)
                offKeySelector = x =>
                {
                    var text = onKeySelector(x);
                    if (text.HasColors)
                        return text.ClearColors();
                    else
                        return new ConsoleString($"[DarkGray:{text.Content}]");
                };
            if (selected == null)
                selected = x => false;

            List<OnOffOption<T>> result = null;

            Console.CursorVisible = false;

            using (var display = new MenuDisplay<OnOffOption<T>>())
            {
                display.Cleanup = cleanup == MenuCleanup.None ? InputCleanup.None : InputCleanup.Clean;
                display.PrefixesTop.SetKeys(labeling);
                display.PrefixesBottom.SetKeys(new char[] { '0' });

                foreach (var item in items)
                    display.Options.Add(new OnOffOption<T>(onKeySelector(item), offKeySelector(item), selected(item), item));

                if (doneText == null)
                    doneText = "Done";
                var doneOption = new OnOffOption<T>(doneText, $"[DarkGray:{doneText.Content}]", true, default(T));
                display.Options.Add(doneOption);

                Func<bool> CheckCanExit = () =>
                {
                    var onCount = display.Options.Count(x => x != doneOption && x.On);
                    return onCount >= minimum && onCount <= maximum;
                };
                doneOption.On = CheckCanExit();

                display.SelectedIndex = 0;

                bool done = false;
                ConsoleKeyInfo info;
                do
                {
                    info = Console.ReadKey(true);
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
                                    doneOption.On = CheckCanExit();
                                }
                            }
                            break;
                    }
                } while (!done);

                result = new List<OnOffOption<T>>(display.Options.Where(x => x != doneOption && x.On));

                if (cleanup == MenuCleanup.None)
                    ColorConsole.CursorPosition = display.Origin + new ConsoleSize(0, display.Options.Count);
            }

            if (cleanup == MenuCleanup.RemoveMenuShowChoice)
                foreach (var r in result)
                    ColorConsole.WriteLine(r.Text);

            Console.CursorVisible = true;

            return result.Select(x => x.Value).ToArray();
        }
        /// <summary>
        /// Displays a menu where a set of elements can be selected from the collection.
        /// Displays the key part of each element in the menu and returns the selected value part.
        /// </summary>
        /// <typeparam name="TKey">The type of the Key part of elements in <paramref name="collection"/>.</typeparam>
        /// <typeparam name="TValue">The type of the Value part of elements in <paramref name="collection"/>.</typeparam>
        /// <param name="minimum">The minimum number of elements that must be selected for the function to return.</param>
        /// <param name="maximum">The maximum number of elements that must be selected for the function to return.</param>
        /// <param name="collection">The collection of elements from which the menu should be created.</param>
        /// <param name="selected">A function that returns a boolean value indicating if an element should be pre-selected when the menu is displayed.</param>
        /// <param name="labeling">The type of labeling (option prefix) that should be applied when displaying the menu.</param>
        /// <param name="cleanup">The cleanup applied after displaying the menu.</param>
        /// <param name="doneText">The <see cref="ConsoleString"/> that is displayed as the bottommost option in the menu. Selecting this option will cause the function to return.</param>
        /// <returns>The elements that were selected using the displayed menu.</returns>
        public static TValue[] MenuSelectMultiple<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> collection, uint minimum = uint.MinValue, uint maximum = uint.MaxValue, Func<KeyValuePair<TKey, TValue>, bool> selected = null, MenuLabeling labeling = MenuLabeling.NumbersAndLetters, MenuCleanup cleanup = MenuCleanup.None, ConsoleString doneText = null)
        {
            return MenuSelectMultiple(collection, minimum, maximum, null, null, selected, labeling, cleanup, doneText).Select(x => x.Value).ToArray();
        }

        /// <summary>
        /// Adds a new option to the menu, which returns a constant value.
        /// </summary>
        /// <param name="menu">The menu to which the option is added.</param>
        /// <param name="text">The text displayed for the new option.</param>
        /// <param name="value">The value returned by the new option.</param>
        public static void Add<T>(this Menu<Func<T>> menu, string text, T value)
        {
            menu.Add(text, () => value);
        }
        /// <summary>
        /// Sets the cancel option for the menu.
        /// </summary>
        /// <param name="menu">The menu for which the cancel option is set.</param>
        /// <param name="text">The text displayed for the cancel option.</param>
        /// <param name="value">The value of type <typeparamref name="T"/> that should be returned if the cancel option is selected.</param>
        public static void SetCancel<T>(this Menu<Func<T>> menu, string text, T value)
        {
            menu.SetCancel(text, () => value);
        }
    }
}

using System;
using System.Collections.Generic;

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
        /// <typeparam name="T">The type of element in <typeparamref name="collection"/>.</typeparam>
        /// <param name="collection">The collection of element from which the <see cref="Menu{T}"/> should be created.</param>
        /// <param name="settings">A <see cref="MenuSettings"/> that expresses the settings used when displaying the menu, or <c>null</c> to use the default settings.</param>
        /// <returns>The element that was selected using the displayed <see cref="Menu{T}"/>.</returns>
        public static T MenuSelect<T>(this IEnumerable<T> collection, MenuSettings settings)
        {
            return MenuSelect(collection, settings, x => x.ToString());
        }
        /// <summary>
        /// Displays a <see cref="Menu{T}"/> where an element from the collection can be selected.
        /// </summary>
        /// <typeparam name="T">The type of element in <typeparamref name="collection"/>.</typeparam>
        /// <param name="collection">The collection of element from which the <see cref="Menu{T}"/> should be created.</param>
        /// <param name="settings">A <see cref="MenuSettings"/> that expresses the settings used when displaying the menu, or <c>null</c> to use the default settings.</param>
        /// <param name="keySelector">A function that gets the <see cref="String"/> that should be displayed for an item in the collection</param>
        /// <returns>The element that was selected using the displayed <see cref="Menu{T}"/>.</returns>
        public static T MenuSelect<T>(this IEnumerable<T> collection, MenuSettings settings, Func<T, string> keySelector)
        {
            Menu<T> menu = new Menu<T>();

            foreach (var item in collection)
                menu.Add(keySelector(item), item);

            return menu.ShowAndSelect(settings).Value;
        }

        /// <summary>
        /// Displays a <see cref="Menu{T}"/> where an element from the collection can be selected.
        /// Displays the key part of each element in the menu and returns the selected value part.
        /// </summary>
        /// <typeparam name="TKey">The type of the Key part of elements in <paramref name="collection"/>.</typeparam>
        /// <typeparam name="TValue">The type of the Value part of elements in <paramref name="collection"/>.</typeparam>
        /// <param name="collection">The collection of element from which the <see cref="Menu{T}"/> should be created.</param>
        /// <param name="settings">A <see cref="MenuSettings"/> that expresses the settings used when displaying the menu, or <c>null</c> to use the default settings.</param>
        /// <returns>The element that was selected using the displayed <see cref="Menu{T}"/>.</returns>
        public static TValue MenuSelect<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> collection, MenuSettings settings)
        {
            return MenuSelect(collection, settings, x => x.Key.ToString()).Value;
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

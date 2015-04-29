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
        /// Shows the menu and waits for an option to be selected.
        /// When an option has been selected, its corresponding delegate is executed.
        /// If <paramref name="repeat"/> is <c>true</c> this process is repeated until the cancel option is selected.
        /// </summary>
        /// <param name="menu">The menu that should be shown.</param>
        /// <param name="settings">A <see cref="MenuSettings"/> that expresses the settings used when displaying the menu, or <c>null</c> to use the default settings.</param>
        /// <param name="repeat">A boolean indicating whether the menu should be displayed repeatedly until the cancel option is selected.</param>
        public static void Show(this Menu<Action> menu, MenuSettings settings, bool repeat = false)
        {
            if (!menu.CanCancel && repeat)
                throw new InvalidOperationException("A menu cannot auto-repeat without a cancel option.");

            Menu<Action>.MenuOption selected;
            do
            {
                selected = menu.ShowAndSelect(settings);
                selected.Value();
            } while (repeat && !selected.IsCancel);
        }
        /// <summary>
        /// Shows the menu and waits for an option to be selected.
        /// When an option has been selected, its corresponding delegate is executed.
        /// </summary>
        /// <param name="menu">The menu that should be shown.</param>
        /// <param name="settings">A <see cref="MenuSettings"/> that expresses the settings used when displaying the menu, or <c>null</c> to use the default settings.</param>
        /// <returns>
        /// The value returned by the delegate called (dependant on the option selected).
        /// </returns>
        public static T Show<T>(this Menu<Func<T>> menu, MenuSettings settings)
        {
            return menu.ShowAndSelect(settings).Value();
        }
        /// <summary>
        /// Shows the menu and waits for an option to be selected.
        /// When an option has been selected, its corresponding delegate is executed.
        /// </summary>
        /// <param name="menu">The menu that should be shown.</param>
        /// <param name="settings">A <see cref="MenuSettings"/> that expresses the settings used when displaying the menu, or <c>null</c> to use the default settings.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> that contains the selected elements (one for each time the menu is displayed).</returns>
        public static IEnumerable<T> ShowRange<T>(this Menu<Func<T>> menu, MenuSettings settings)
        {
            Menu<Func<T>>.MenuOption selected;
            do
            {
                selected = menu.ShowAndSelect(settings);
                yield return selected.Value();
            } while (!selected.IsCancel);
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

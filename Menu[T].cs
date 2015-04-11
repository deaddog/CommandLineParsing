using System;
using System.Collections.Generic;

namespace CommandLineParsing
{
    /// <summary>
    /// Represents a menu where each option is associated with a <see cref="Func{T}"/> delegate.
    /// </summary>
    /// <typeparam name="T">The type of elements returned by the menu.</typeparam>
    public class Menu<T> : MenuBase<Func<T>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Menu"/> class.
        /// </summary>
        /// <param name="labels">Defines the type of labeling used when displaying this menu.</param>
        public Menu(MenuLabeling labels)
            : base(labels)
        {
        }

        /// <summary>
        /// Adds a new option to the menu, which returns a constant value.
        /// </summary>
        /// <param name="text">The text displayed for the new option.</param>
        /// <param name="value">The value returned by the new option.</param>
        /// <param name="color">The color of the text displayed for the new option.</param>
        public void Add(string text, T value)
        {
            this.Add(text, () => value);
        }

        /// <summary>
        /// Sets the cancel option for the menu.
        /// The default value of <typeparamref name="T"/> is returned if the cancel option is selected.
        /// </summary>
        /// <param name="text">The text displayed for the cancel option.</param>
        /// <param name="color">The color of the text displayed for the cancel option.</param>
        public void SetCancel(string text)
        {
            base.SetCancel(text, () => default(T));
        }
        /// <summary>
        /// Sets the cancel option for the menu.
        /// </summary>
        /// <param name="text">The text displayed for the cancel option.</param>
        /// <param name="value">The value of type <typeparamref name="T"/> that should be returned if the cancel option is selected.</param>
        /// <param name="color">The color of the text displayed for the cancel option.</param>
        public void SetCancel(string text, T value)
        {
            base.SetCancel(text, () => value);
        }

        /// <summary>
        /// Shows the menu and waits for an option to be selected.
        /// When an option has been selected, its corresponding delegate is executed.
        /// </summary>
        /// <returns>The value returned by the delegate called (dependant on the option selected).</returns>
        public T Show()
        {
            MenuOption selected = ShowAndSelect();
            return selected.Action();
        }

        /// <summary>
        /// Shows the menu and waits for an option to be selected.
        /// When an option has been selected, its corresponding delegate is executed.
        /// </summary>
        /// <param name="repeat">A boolean indicating whether the menu should be displayed repeatedly until the cancel option is selected.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> that contains the selected elements (one for each time the menu is displayed).</returns>
        public IEnumerable<T> Show(bool repeat)
        {
            if (!this.CanCancel && repeat)
                throw new InvalidOperationException("A menu cannot auto-repeat without a cancel option.");

            MenuOption selected;
            do
            {
                selected = ShowAndSelect();
                yield return selected.Action();
            } while (repeat && !selected.IsCancel);
        }
    }
}

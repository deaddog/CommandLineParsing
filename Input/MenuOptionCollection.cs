using System.Collections.Generic;

namespace CommandLineParsing.Input
{
    /// <summary>
    /// Defines a collection of <see cref="MenuOption{T}"/> that is displayed by a <see cref="MenuDisplay{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the values managed by the <see cref="MenuOptionCollection{T}"/>.</typeparam>
    public class MenuOptionCollection<T>
    {
        private readonly MenuDisplay<T> _display;
        private readonly List<MenuOption<T>> _options;

        internal MenuOptionCollection(MenuDisplay<T> display)
        {
            _display = display;
            _options = new List<MenuOption<T>>();
        }

        /// <summary>
        /// Gets the number of options in the collection.
        /// </summary>
        public int Count => _options.Count;

        /// <summary>
        /// Gets the <see cref="MenuOption{T}"/> at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>The <see cref="MenuOption{T}"/> at the specified index.</returns>
        public MenuOption<T> this[int index] => _options[index];

        /// <summary>
        /// Gets the index of a specific <see cref="MenuOption{T}"/> in the collection.
        /// </summary>
        /// <param name="option">The option to look for.</param>
        /// <returns>The index of <paramref name="option"/>, or <c>-1</c> if it doesn't exist.</returns>
        public int IndexOf(MenuOption<T> option)
        {
            return _options.IndexOf(option);
        }
    }
}

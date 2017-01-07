using System;
using System.Collections;
using System.Collections.Generic;

namespace CommandLineParsing.Input
{
    /// <summary>
    /// Defines a collection of <see cref="MenuOption{T}"/> that is displayed by a <see cref="MenuDisplay{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the values managed by the <see cref="MenuOptionCollection{T}"/>.</typeparam>
    public class MenuOptionCollection<T> : IList<MenuOption<T>>
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
        /// Gets or sets the <see cref="MenuOption{T}"/> at the specified index.
        /// </summary>
        /// <param name="index">The option index.</param>
        /// <returns>The <see cref="MenuOption{T}"/> at the specified index.</returns>
        public MenuOption<T> this[int index]
        {
            get { return _options[index]; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));

                var old = _options[index];
                old.TextChanged -= _display.UpdateOption;

                _options[index] = value;
                value.TextChanged += _display.UpdateOption;

                _display.UpdateOption(index, old.Text, value.Text);
            }
        }

        /// <summary>
        /// Gets the index of a specific <see cref="MenuOption{T}"/> in the collection.
        /// </summary>
        /// <param name="option">The option to look for.</param>
        /// <returns>The index of <paramref name="option"/>, or <c>-1</c> if it doesn't exist.</returns>
        public int IndexOf(MenuOption<T> option)
        {
            if (option == null)
                throw new ArgumentNullException(nameof(option));

            return _options.IndexOf(option);
        }

        /// <summary>
        /// Determines whether an option is part of this collection.
        /// </summary>
        /// <param name="option">The option.</param>
        /// <returns><c>true</c> if <paramref name="option"/> is found in the collection; otherwise, <c>false</c>.</returns>
        public bool Contains(MenuOption<T> option)
        {
            if (option == null)
                throw new ArgumentNullException(nameof(option));

            return _options.Contains(option);
        }

        /// <summary>
        /// Adds an option to the menu display.
        /// </summary>
        /// <param name="option">The option to add.</param>
        public void Add(MenuOption<T> option)
        {
            if (option == null)
                throw new ArgumentNullException(nameof(option));

            _options.Add(option);
            option.TextChanged += _display.UpdateOption;

            _display.UpdateOption(_options.Count - 1, "", option.Text);

            if (_display.SelectedIndex == -1)
                _display.SelectedIndex = 0;
        }
        /// <summary>
        /// Inserts an option in the menu display at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="option">The option to add.</param>
        public void Insert(int index, MenuOption<T> option)
        {
            if (option == null)
                throw new ArgumentNullException(nameof(option));

            if (index == _options.Count)
            {
                Add(option);
                return;
            }

            _options.Insert(index, option);
            option.TextChanged += _display.UpdateOption;

            for (int i = index; i < _options.Count - 1; i++)
                _display.UpdateOption(i, _options[i + 1].Text, _options[i].Text);
            _display.UpdateOption(_options.Count - 1, "", _options[_options.Count - 1].Text);

            if (_display.SelectedIndex == -1)
                _display.SelectedIndex = 0;
        }

        /// <summary>
        /// Removes the specified option from the menu display.
        /// </summary>
        /// <param name="option">The option to remove.</param>
        /// <returns>
        /// <c>true</c> if options is successfully removed; otherwise, <c>false</c>.
        /// This method also returns <c>false</c> if the option was not found in the collection.</returns>
        public bool Remove(MenuOption<T> option)
        {
            if (option == null)
                throw new ArgumentNullException(nameof(option));

            int index = _options.IndexOf(option);

            if (index == -1)
                return false;
            else
            {
                RemoveAt(index);
                return true;
            }
        }
        /// <summary>
        /// Removes the option at the specified index.
        /// </summary>
        /// <param name="index">The index to remove from.</param>
        public void RemoveAt(int index)
        {
            var last = _options[index];
            last.TextChanged -= _display.UpdateOption;
            _options.RemoveAt(index);

            for (int i = index; i < _options.Count; i++)
            {
                _display.UpdateOption(i, last.Text, _options[i].Text);
                last = _options[i];
            }

            _display.UpdateOption(_options.Count, last.Text, null);
        }

        /// <summary>
        /// Removes all options from the menu display.
        /// </summary>
        public void Clear()
        {
            while (_options.Count > 0)
                RemoveAt(_options.Count - 1);
        }

        bool ICollection<MenuOption<T>>.IsReadOnly => false;
        void ICollection<MenuOption<T>>.CopyTo(MenuOption<T>[] array, int arrayIndex)
        {
            _options.CopyTo(array, arrayIndex);
        }

        IEnumerator<MenuOption<T>> IEnumerable<MenuOption<T>>.GetEnumerator()
        {
            foreach (var o in _options)
                yield return o;
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (var o in _options)
                yield return o;
        }
    }
}

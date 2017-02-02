﻿using CommandLineParsing.Output;
using System;
using System.Collections;
using System.Collections.Generic;

namespace CommandLineParsing.Input
{
    /// <summary>
    /// Defines a collection of <see cref="IMenuOption"/> that is displayed by a <see cref="MenuDisplay{TOption}"/>.
    /// </summary>
    /// <typeparam name="TOption">The type of the options managed by the <see cref="MenuOptionCollection{TOption}"/>.</typeparam>
    public class MenuOptionCollection<TOption> : IList<TOption> where TOption : class, IMenuOption
    {
        private readonly MenuDisplay<TOption> _display;
        private readonly List<TOption> _options;

        internal MenuOptionCollection(MenuDisplay<TOption> display)
        {
            _display = display;
            _options = new List<TOption>();
        }

        private void OptionUpdateHelper(IMenuOption option)
        {
            _display.UpdateOption(IndexOf(option), option.Text);
        }

        /// <summary>
        /// Gets the number of options in the collection.
        /// </summary>
        public int Count => _options.Count;

        /// <summary>
        /// Gets or sets the <see cref="IMenuOption"/> at the specified index.
        /// </summary>
        /// <param name="index">The option index.</param>
        /// <returns>The <see cref="IMenuOption"/> at the specified index.</returns>
        public TOption this[int index]
        {
            get { return _options[index]; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));

                var old = _options[index];
                old.TextChanged -= OptionUpdateHelper;

                _options[index] = value;
                value.TextChanged += OptionUpdateHelper;

                _display.UpdateOption(index, value.Text);
            }
        }

        /// <summary>
        /// Gets the index of a specific <see cref="IMenuOption"/> in the collection.
        /// </summary>
        /// <param name="option">The option to look for.</param>
        /// <returns>The index of <paramref name="option"/>, or <c>-1</c> if it doesn't exist.</returns>
        public int IndexOf(TOption option)
        {
            if (option == null)
                throw new ArgumentNullException(nameof(option));

            return _options.IndexOf(option);
        }
        internal int IndexOf(IMenuOption option)
        {
            if (option == null)
                throw new ArgumentNullException(nameof(option));

            var cast = option as TOption;

            if (cast == null)
                return -1;
            else
                return IndexOf(cast);
        }

        /// <summary>
        /// Determines whether an option is part of this collection.
        /// </summary>
        /// <param name="option">The option.</param>
        /// <returns><c>true</c> if <paramref name="option"/> is found in the collection; otherwise, <c>false</c>.</returns>
        public bool Contains(TOption option)
        {
            if (option == null)
                throw new ArgumentNullException(nameof(option));

            return _options.Contains(option);
        }

        /// <summary>
        /// Adds an option to the menu display.
        /// </summary>
        /// <param name="option">The option to add.</param>
        public void Add(TOption option)
        {
            if (option == null)
                throw new ArgumentNullException(nameof(option));

            _options.Add(option);
            option.TextChanged += OptionUpdateHelper;

            _display.UpdateOption(_options.Count - 1, option.Text);

        }
        /// <summary>
        /// Inserts an option in the menu display at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="option">The option to add.</param>
        public void Insert(int index, TOption option)
        {
            if (option == null)
                throw new ArgumentNullException(nameof(option));

            if (index == _options.Count)
            {
                Add(option);
                return;
            }

            _options.Insert(index, option);
            option.TextChanged += OptionUpdateHelper;

            for (int i = index; i < _options.Count - 1; i++)
                _display.UpdateOption(i, _options[i].Text);
            _display.UpdateOption(_options.Count - 1, _options[_options.Count - 1].Text);

            if (index <= _display.SelectedIndex)
                _display.SelectedIndex++;
        }

        /// <summary>
        /// Removes the specified option from the menu display.
        /// </summary>
        /// <param name="option">The option to remove.</param>
        /// <returns>
        /// <c>true</c> if options is successfully removed; otherwise, <c>false</c>.
        /// This method also returns <c>false</c> if the option was not found in the collection.</returns>
        public bool Remove(TOption option)
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
            last.TextChanged -= OptionUpdateHelper;
            _options.RemoveAt(index);

            for (int i = index; i < _options.Count; i++)
            {
                _display.UpdateOption(i, _options[i].Text);
                last = _options[i];
            }

            _display.UpdateOption(_options.Count, null);

            if (index < _display.SelectedIndex || _options.Count == _display.SelectedIndex)
                _display.SelectedIndex--;
        }

        /// <summary>
        /// Removes all options from the menu display.
        /// </summary>
        public void Clear()
        {
            while (_options.Count > 0)
                RemoveAt(_options.Count - 1);
        }

        bool ICollection<TOption>.IsReadOnly => false;
        void ICollection<TOption>.CopyTo(TOption[] array, int arrayIndex)
        {
            _options.CopyTo(array, arrayIndex);
        }

        IEnumerator<TOption> IEnumerable<TOption>.GetEnumerator()
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

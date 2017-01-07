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
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Gets the index of a specific <see cref="MenuOption{T}"/> in the collection.
        /// </summary>
        /// <param name="option">The option to look for.</param>
        /// <returns>The index of <paramref name="option"/>, or <c>-1</c> if it doesn't exist.</returns>
        public int IndexOf(MenuOption<T> option)
        {
            return _options.IndexOf(option);
        }

        public bool Contains(MenuOption<T> item)
        {
            throw new NotImplementedException();
        }

        public void Add(MenuOption<T> item)
        {
            throw new NotImplementedException();
        }
        public void Insert(int index, MenuOption<T> item)
        {
            throw new NotImplementedException();
        }

        public bool Remove(MenuOption<T> item)
        {
            throw new NotImplementedException();
        }
        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
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

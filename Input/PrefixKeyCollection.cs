using System;
using System.Collections.Generic;
using System.Linq;

namespace CommandLineParsing.Input
{
    /// <summary>
    /// Defines a collection of characters that are used as prefixes for options in a menu.
    /// </summary>
    public class PrefixKeyCollection
    {
        private readonly List<char> _keys;

        /// <summary>
        /// Initializes a new instance of the <see cref="PrefixKeyCollection"/> class.
        /// </summary>
        public PrefixKeyCollection()
        {
            _keys = new List<char>();
        }

        /// <summary>
        /// Occurs when the set of prefix keys has changed.
        /// </summary>
        public event Action PrefixSetChanged;

        /// <summary>
        /// Gets the number of prefixes in the collection.
        /// </summary>
        public int Count => _keys.Count;

        /// <summary>
        /// Gets the index of a <see cref="char"/> in the collection, or <c>-1</c> if the <see cref="char"/> is not part of the collection.
        /// </summary>
        /// <param name="prefixChar">The prefix character to look for.</param>
        /// <returns>The index of <paramref name="prefixChar"/> in the collection.</returns>
        public int IndexFromPrefix(char prefixChar)
        {
            var low = char.ToLowerInvariant(prefixChar);
            return _keys.FindIndex(x => char.ToLowerInvariant(x) == low);
        }
        /// <summary>
        /// Gets the <see cref="char"/> at a specific index in the collection, or <c>null</c> if no <see cref="char"/> is defined for the index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>The prefix character at <paramref name="index"/>.</returns>
        public char? PrefixFromIndex(int index)
        {
            if (index >= 0 && index < _keys.Count)
                return _keys[index];
            else
                return null;
        }
        
        /// <summary>
        /// Sets the prefix keys used by the collection.
        /// </summary>
        /// <param name="prefixChars">The prefix chars.</param>
        public void SetKeys(IEnumerable<char> prefixChars)
        {
            _keys.Clear();
            _keys.AddRange(prefixChars);

            PrefixSetChanged?.Invoke();
        }
    }
}

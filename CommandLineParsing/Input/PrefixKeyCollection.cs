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
        /// <summary>
        /// Sets the prefix keys used by the collection from one of the <see cref="MenuLabeling"/> values.
        /// </summary>
        /// <param name="label">The set of prefix keys to use.</param>
        public void SetKeys(MenuLabeling label)
        {
            switch (label)
            {
                case MenuLabeling.None: SetKeys(new char[0]); break;
                case MenuLabeling.Numbers: SetKeys(GetRange('1', '9')); break;
                case MenuLabeling.Letters: SetKeys(GetRange('a', 'z')); break;
                case MenuLabeling.LettersUpper: SetKeys(GetRange('A', 'Z')); break;
                case MenuLabeling.NumbersAndLetters: SetKeys(GetRange('1', '9').Concat(GetRange('a', 'z'))); break;
                case MenuLabeling.NumbersAndLettersUpper: SetKeys(GetRange('1', '9').Concat(GetRange('A', 'Z'))); break;
            }
        }
        private IEnumerable<char> GetRange(char from, char to)
        {
            if (from > to)
            {
                var temp = from;
                from = to;
                to = temp;
            }

            while (from < to)
                yield return from++;

            yield return to;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;

namespace CommandLineParsing
{
    /// <summary>
    /// Provides a collection of methods for calculating the edit distance between two strings (see http://en.wikipedia.org/wiki/Edit_distance).
    /// </summary>
    public static class EditDistance
    {
        /// <summary>
        /// Orders the strings in <paramref name="collection"/> according to their edit distance to <paramref name="origin"/>.
        /// </summary>
        /// <param name="collection">The strings to which edit distance is calculated.</param>
        /// <param name="origin">The origin string from which edit distance is calculated.</param>
        /// <param name="add">The weight of an 'add' operation (inserting a character).</param>
        /// <param name="remove">The weight of a 'remove' operation (deleting a character).</param>
        /// <param name="replace">The weight of  a 'replace' operation (replacing a character, maintaining order).</param>
        /// <param name="swap">The weight of a 'swap' operation (swapping two neighbouring characters) or <c>null</c> to disable swap operations.</param>
        /// <returns>An ordered collection of strings and their edit distance to <paramref name="origin"/>, ordered by the edit distance (ascending).</returns>
        public static IEnumerable<Tuple<string, uint>> OrderByDistance(IEnumerable<string> collection, string origin, uint add, uint remove, uint replace, uint? swap = null)
        {
            return OrderByDistance(collection, origin, GetEditDistanceMethod(add, remove, replace, swap));
        }

        /// <summary>
        /// Orders the strings in <paramref name="collection"/> according to their edit distance to <paramref name="origin"/>.
        /// </summary>
        /// <param name="collection">The strings to which edit distance is calculated.</param>
        /// <param name="origin">The origin string from which edit distance is calculated.</param>
        /// <param name="editdistance">The method used to calculate the edit distance. This can be retrieved using the <see cref="GetEditDistanceMethod(uint, uint, uint, uint?)"/> method.</param>
        /// <returns>An ordered collection of strings and their edit distance to <paramref name="origin"/>, ordered by the edit distance (ascending).</returns>
        public static IEnumerable<Tuple<string, uint>> OrderByDistance(IEnumerable<string> collection, string origin, Func<string, string, uint> editdistance)
        {
            Tuple<string, uint>[] dist = collection.Select(x => Tuple.Create(x, editdistance(origin, x))).ToArray();
            Array.Sort(dist, (x, y) => x.Item2.CompareTo(y.Item2));

            foreach (var t in dist)
                yield return t;
        }

        /// <summary>
        /// Gets a method for calculating edit distance between strings using a set of weights.
        /// </summary>
        /// <param name="add">The weight of an 'add' operation (inserting a character).</param>
        /// <param name="remove">The weight of a 'remove' operation (deleting a character).</param>
        /// <param name="replace">The weight of  a 'replace' operation (replacing a character, maintaining order).</param>
        /// <param name="swap">The weight of a 'swap' operation (swapping two neighbouring characters) or <c>null</c> to disable swap operations.</param>
        /// <returns>A method that will calculate the edit distance between strings using the set of weights.</returns>
        public static Func<string, string, uint> GetEditDistanceMethod(uint add, uint remove, uint replace, uint? swap = null)
        {
            return (string a, string b) => GetEditDistance(a, b, add, remove, replace, swap);
        }

        /// <summary>
        /// Calculates the edit distance between <paramref name="from" /> and <paramref name="to" />.
        /// </summary>
        /// <param name="from">The origin string from which edit distance is calculated.</param>
        /// <param name="to">The target string to which edit distance is calculated.</param>
        /// <param name="add">The weight of an 'add' operation (inserting a character).</param>
        /// <param name="remove">The weight of a 'remove' operation (deleting a character).</param>
        /// <param name="replace">The weight of  a 'replace' operation (replacing a character, maintaining order).</param>
        /// <param name="swap">The weight of a 'swap' operation (swapping two neighbouring characters) or <c>null</c> to disable swap operations.</param>
        /// <returns>
        /// The edit distance between the two strings (see http://en.wikipedia.org/wiki/Edit_distance).
        /// </returns>
        public static uint GetEditDistance(string from, string to, uint add, uint remove, uint replace, uint? swap = null)
        {
            if (swap.HasValue)
                return editDistance(from, to, add, remove, replace, swap.Value, 1);

            uint[,] dist = new uint[from.Length + 1, to.Length + 1];
            for (uint i = 1; i <= from.Length; i++) dist[i, 0] = i * remove;
            for (uint j = 1; j <= to.Length; j++) dist[0, j] = j * add;
            for (int i = 1; i <= from.Length; i++)
                for (int j = 1; j <= to.Length; j++)
                    if (from[i - 1] == to[j - 1])
                        dist[i, j] = Math.Min(dist[i - 1, j - 1], Math.Min(dist[i - 1, j] + remove, dist[i, j - 1] + add));
                    else
                        dist[i, j] = Math.Min(dist[i - 1, j - 1] + replace, Math.Min(dist[i - 1, j] + remove, dist[i, j - 1] + add));

            return dist[from.Length, to.Length];
        }

        private static uint editDistance(string origin, string str, uint add, uint remove, uint replace, uint swap, int nextswap)
        {
            if (nextswap > origin.Length - 1)
                return GetEditDistance(origin, str, add, remove, replace);
            else
                return Math.Min(
                    editDistance(origin, str, add, remove, replace, swap, nextswap + 1),
                    editDistance(swapAt(origin, nextswap), str, add, remove, replace, swap, nextswap + 2) + swap);
        }

        private static string swapAt(string str, int lastindex)
        {
            return str.Substring(0, lastindex - 1) + str[lastindex] + str[lastindex - 1] + str.Substring(lastindex + 1);
        }
    }
}

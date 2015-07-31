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
        /// Orders the strings in <paramref name="strings"/> according to their edit distance to <paramref name="origin"/>.
        /// </summary>
        /// <param name="strings">The strings to which edit distance is calculated.</param>
        /// <param name="origin">The origin string from which edit distance is calculated.</param>
        /// <returns>An ordered collection of strings and their edit distance to <paramref name="origin"/>, ordered by the edit distance (ascending).</returns>
        public static IEnumerable<Tuple<string, int>> OrderByDistance(this IEnumerable<string> strings, string origin)
        {
            Tuple<string, int>[] dist = strings.Select(x => Tuple.Create(x, GetEditDistance(origin, x))).ToArray();
            Array.Sort(dist, (x, y) => x.Item2.CompareTo(y.Item2));

            foreach (var t in dist)
                yield return t;
        }

        /// <summary>
        /// Calculates the edit distance between <paramref name="from"/> and <paramref name="to"/>.
        /// </summary>
        /// <param name="from">The origin string from which edit distance is calculated.</param>
        /// <param name="to">The target string to which edit distance is calculated.</param>
        /// <returns>The edit distance between the two strings (see http://en.wikipedia.org/wiki/Edit_distance). </returns>
        public static int GetEditDistance(string from, string to)
        {
            int[,] dist = new int[from.Length + 1, to.Length + 1];
            for (int i = 1; i <= from.Length; i++) dist[i, 0] = i;
            for (int j = 1; j <= to.Length; j++) dist[0, j] = j;
            for (int i = 1; i <= from.Length; i++)
                for (int j = 1; j <= to.Length; j++)
                    if (from[i - 1] == to[j - 1])
                        dist[i, j] = Math.Min(dist[i - 1, j - 1], Math.Min(dist[i - 1, j] + 1, dist[i, j - 1] + 1));
                    else
                        dist[i, j] = 1 + Math.Min(dist[i - 1, j - 1], Math.Min(dist[i - 1, j], dist[i, j - 1]));

            return dist[from.Length, to.Length];
        }
    }
}

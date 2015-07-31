using System;
using System.Collections.Generic;
using System.Linq;

namespace CommandLineParsing
{
    /// <summary>
    /// Provides a collection of methods for calculating the edit distance between two strings (see http://en.wikipedia.org/wiki/Edit_distance).
    /// </summary>
    public static class EditDistanceExtension
    {
        /// <summary>
        /// Orders the strings in <paramref name="strings"/> according to their edit distance to <paramref name="origin"/>.
        /// </summary>
        /// <param name="strings">The strings to which edit distance is calculated.</param>
        /// <param name="origin">The origin string from which edit distance is calculated.</param>
        /// <returns>An ordered collection of strings and their edit distance to <paramref name="origin"/>, ordered by the edit distance (ascending).</returns>
        public static IEnumerable<Tuple<string, int>> OrderByDistance(this IEnumerable<string> strings, string origin)
        {
            Tuple<string, int>[] dist = strings.Select(x => Tuple.Create(x, EditDistanceTo(origin, x))).ToArray();
            Array.Sort(dist, (x, y) => x.Item2.CompareTo(y.Item2));

            foreach (var t in dist)
                yield return t;
        }

        /// <summary>
        /// Calculates the edit distance from <paramref name="origin"/> to <paramref name="str"/>.
        /// </summary>
        /// <param name="origin">The origin string from which edit distance is calculated.</param>
        /// <param name="str">The target string to which edit distance is calculated.</param>
        /// <returns>The edit distance between the two strings (see http://en.wikipedia.org/wiki/Edit_distance). </returns>
        public static int EditDistanceTo(this string origin, string str)
        {
            int[,] dist = new int[origin.Length + 1, str.Length + 1];
            for (int i = 1; i <= origin.Length; i++) dist[i, 0] = i;
            for (int j = 1; j <= str.Length; j++) dist[0, j] = j;
            for (int i = 1; i <= origin.Length; i++)
                for (int j = 1; j <= str.Length; j++)
                    if (origin[i - 1] == str[j - 1])
                        dist[i, j] = Math.Min(dist[i - 1, j - 1], Math.Min(dist[i - 1, j] + 1, dist[i, j - 1] + 1));
                    else
                        dist[i, j] = 1 + Math.Min(dist[i - 1, j - 1], Math.Min(dist[i - 1, j], dist[i, j - 1]));

            return dist[origin.Length, str.Length];
        }
    }
}

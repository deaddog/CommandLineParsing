using System;
using System.Collections.Generic;
using System.Linq;

namespace CommandLineParsing
{
    public static class EditDistanceExtension
    {
        public static IEnumerable<Tuple<string, int>> OrderByDistance(this IEnumerable<string> strings, string origin)
        {
            Tuple<string, int>[] dist = strings.Select(x => Tuple.Create(x, EditDistanceTo(origin, x))).ToArray();
            Array.Sort(dist, (x, y) => x.Item2.CompareTo(y.Item2));

            foreach (var t in dist)
                yield return t;
        }

        public static int EditDistanceTo(this string origin, string str)
        {
            int[,] dist = new int[origin.Length + 1, str.Length + 1];
            for (int i = 0; i <= origin.Length; i++) dist[i, 0] = i;
            for (int j = 1; j <= str.Length; j++) dist[0, j] = j;
            for (int i = 1; i <= origin.Length; i++)
                for (int j = 1; j <= str.Length; j++)
                    if (origin[i - 1] == str[j - 1])
                        dist[i, j] = Math.Min(dist[i - 1, j - 1], Math.Min(dist[i - 1, j] + 1, dist[i, j - 1] + 1));
                    else
                        dist[i, j] = 1 + Math.Min(dist[i - 1, j - 1], Math.Min(dist[i - 1, j], dist[i, j - 1]));

            int v = dist[origin.Length, str.Length];
            return v;
        }
    }
}

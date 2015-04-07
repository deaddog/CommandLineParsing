using System.Collections;
using System.Text;

namespace CommandLineParsing
{
    /// <summary>
    /// Provides methods that simplifies object-to-string conversions as extension methods.
    /// </summary>
    public static class ToStringExtension
    {
        /// <summary>
        /// Returns a <see cref="string"/> that is a listing of the elements in <paramref name="collection"/>.
        /// This can be used to create strings such as "<c>item1</c>, <c>item2</c>, <c>item3</c> and <c>item4</c>".
        /// </summary>
        /// <param name="collection">The collection that should be converted into a string.</param>
        /// <param name="separator">The <see cref="string"/> that should be used to separate all elements except the last two.</param>
        /// <param name="lastseparator">The <see cref="string"/> that should be used to separate the last two elements.</param>
        /// <returns>
        /// A <see cref="string"/> that is a listing of the elements in <paramref name="collection"/>.
        /// </returns>
        public static string ToString(this IEnumerable collection, string separator, string lastseparator)
        {
            var enumerator = collection.GetEnumerator();
            if (!enumerator.MoveNext())
                return string.Empty;

            StringBuilder sb = new StringBuilder();
            sb.Append(enumerator.Current);

            if (!enumerator.MoveNext())
                return sb.ToString();

            object prev = enumerator.Current;

            while (enumerator.MoveNext())
            {
                sb.Append(separator);
                sb.Append(prev);
                prev = enumerator.Current;
            }

            sb.Append(lastseparator);
            sb.Append(prev);

            return sb.ToString();
        }
    }
}

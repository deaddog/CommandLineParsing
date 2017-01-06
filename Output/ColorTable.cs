using System;
using System.Collections.Generic;

namespace CommandLineParsing.Output
{
    /// <summary>
    /// Provides a collection of <see cref="string"/>-><see cref="ConsoleColor"/> relations.
    /// </summary>
    public class ColorTable
    {
        private Dictionary<string, ConsoleColor> colors;

        internal ColorTable()
        {
            colors = new Dictionary<string, ConsoleColor>();

            foreach (var c in Enum.GetValues(typeof(ConsoleColor)))
                colors.Add(c.ToString().ToLowerInvariant(), (ConsoleColor)c);
        }

        /// <summary>
        /// Gets or sets the <see cref="System.Nullable{ConsoleColor}"/> with the specified name.
        /// A value of <c>null</c> (in both getter and setter) is equivalent of no color.
        /// </summary>
        /// <param name="name">The name associated with the <see cref="ConsoleColor"/>.
        /// This name does not have to pre-exist in the <see cref="ConsoleColor"/> enum.
        /// The name is case insensitive, meaning that "Red" and "red" will refer to the same color, if any.</param>
        /// <returns>The <see cref="ConsoleColor"/> associated with <paramref name="name"/> or <c>null</c>, if no color is associated with <paramref name="name"/>.</returns>
        public ConsoleColor? this[string name]
        {
            get
            {
                if (name == null)
                    throw new ArgumentNullException(nameof(name));
                else
                    name = name.Trim().ToLowerInvariant();

                if (name.Length == 0)
                    return null;

                ConsoleColor c;
                if (!colors.TryGetValue(name, out c))
                    return null;
                else
                    return c;
            }
            set
            {
                if (name == null)
                    throw new ArgumentNullException(nameof(name));
                else
                    name = name.Trim().ToLowerInvariant();

                if (name.Length == 0)
                    throw new ArgumentException("Color name must be non-empty.", nameof(name));

                if (value.HasValue)
                    colors[name] = value.Value;
                else
                    colors.Remove(name.ToLowerInvariant());
            }
        }
    }
}

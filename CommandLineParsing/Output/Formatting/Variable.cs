using System;
using System.Collections.Immutable;

namespace CommandLineParsing.Output.Formatting
{
    /// <summary>
    /// Defines the configuration of a variable when used in a format.
    /// </summary>
    /// <typeparam name="T">The type of elements the variable represents.</typeparam>
    public class Variable<T>
    {
        /// <summary>
        /// Initializes a new variable.
        /// </summary>
        /// <param name="name">The name of the variable, as used in a format.</param>
        /// <param name="selector">A function that gets the variables content when used in a format.</param>
        /// <param name="paddedLength">The variables padded length, or <c>null</c> is padding should not be enabled.</param>
        /// <param name="dynamicColors">Describes dynamic color translation for the variable.</param>
        public Variable(string name, Func<T, string> selector, int? paddedLength, IImmutableDictionary<string, Func<T, Color>> dynamicColors)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Variable must have a name.", nameof(name));
            if (!name.Equals(name.Trim(), StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("Variable must cannot be padded.", nameof(name));

            if (paddedLength <= 0)
                throw new ArgumentOutOfRangeException(nameof(paddedLength));

            Name = name ?? throw new ArgumentNullException(nameof(name));
            PaddedLength = paddedLength;

            Selector = selector ?? throw new ArgumentNullException(nameof(selector));
            DynamicColors = dynamicColors ?? throw new ArgumentNullException(nameof(dynamicColors));
        }

        /// <summary>
        /// Gets the name of the variable.
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// Gets the number of characters this variable will be padded to, or <c>null</c> if padding doesn't apply.
        /// </summary>
        public int? PaddedLength { get; }

        /// <summary>
        /// Gets the functions that gets the variables content from an item.
        /// </summary>
        public Func<T, string> Selector { get; }
        /// <summary>
        /// Gets a dictionary of dynamic colors.
        /// The key will work as the color name in a format, and the functions represent the method that defines the actual color.
        /// </summary>
        public IImmutableDictionary<string, Func<T, Color>> DynamicColors { get; }
    }
}

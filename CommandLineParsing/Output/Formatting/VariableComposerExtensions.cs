using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace CommandLineParsing.Output.Formatting
{
    /// <summary>
    /// Extension methods for <see cref="IVariableComposer{T}"/> to simplify the fluent config.
    /// </summary>
    public static class VariableComposerExtensions
    {
        /// <summary>
        /// Adds a variable to the composer.
        /// </summary>
        /// <typeparam name="T">The type of elements the resulting formatter will support.</typeparam>
        /// <param name="composer">The <see cref="IFormatterComposer{T}"/> being extended.</param>
        /// <param name="name">The name of the variable.</param>
        /// <param name="selector">A function that gets the variables content when used in a format.</param>
        /// <returns>A <see cref="IVariableComposer{T}"/> for further configuration.</returns>
        public static IVariableComposer<T> With<T>(this IFormatterComposer<T> composer, string name, Func<T, string> selector)
        {
            return composer.With(new Variable<T>
            (
                name: name,
                selector: selector,
                paddedLength: null,
                dynamicColors: ImmutableDictionary<string, Func<T, Color>>.Empty
            ));
        }
        /// <summary>
        /// Adds a variable to the composer.
        /// </summary>
        /// <typeparam name="T">The type of elements the resulting formatter will support.</typeparam>
        /// <typeparam name="TValue">The type of value returned by the selector.</typeparam>
        /// <param name="composer">The <see cref="IFormatterComposer{T}"/> being extended.</param>
        /// <param name="name">The name of the variable.</param>
        /// <param name="selector">A function that gets the variables content when used in a format. The objects <see cref="Object.ToString"/> method will be used for console output.</param>
        /// <returns>A <see cref="IVariableComposer{T}"/> for further configuration.</returns>
        public static IVariableComposer<T> With<T, TValue>(this IFormatterComposer<T> composer, string name, Func<T, TValue> selector)
        {
            return With(composer, name, x => selector(x).ToString());
        }

        /// <summary>
        /// Sets the variables padded length.
        /// </summary>
        /// <typeparam name="T">The type of elements the resulting formatter will support.</typeparam>
        /// <param name="composer">The <see cref="IVariableComposer{T}"/> being extended.</param>
        /// <param name="length">The variables padded length.</param>
        /// <returns>A <see cref="IVariableComposer{T}"/> for further configuration.</returns>
        public static IVariableComposer<T> WithPaddedLength<T>(this IVariableComposer<T> composer, int length)
        {
            return composer.With(new Variable<T>
            (
                name: composer.Variable.Name,
                selector: composer.Variable.Selector,
                paddedLength: length,
                dynamicColors: composer.Variable.DynamicColors
            ));
        }
        /// <summary>
        /// Sets the variables padded length, as the max length in <paramref name="collection"/>.
        /// </summary>
        /// <typeparam name="T">The type of elements the resulting formatter will support.</typeparam>
        /// <param name="composer">The <see cref="IVariableComposer{T}"/> being extended.</param>
        /// <param name="collection">A collection of items, where padding will match the element in the collection with the longest string representation.</param>
        /// <returns>A <see cref="IVariableComposer{T}"/> for further configuration.</returns>
        public static IVariableComposer<T> WithPaddedLengthFrom<T>(this IVariableComposer<T> composer, IEnumerable<T> collection)
        {
            return composer.WithPaddedLength(collection.Select(x => composer.Variable.Selector(x).Length).Concat(new[] { 0 }).Max());
        }

        /// <summary>
        /// Sets handling of a dynamic color.
        /// </summary>
        /// <typeparam name="T">The type of elements the resulting formatter will support.</typeparam>
        /// <param name="composer">The <see cref="IVariableComposer{T}"/> being extended.</param>
        /// <param name="color">The dynamic color name that this rule should apply to.</param>
        /// <param name="colorSelector">A function that determies the actual color to print when this dynamic color is used. Color strings are parsed using <see cref="Color.Parse(string)"/>.</param>
        /// <returns>A <see cref="IVariableComposer{T}"/> for further configuration.</returns>
        public static IVariableComposer<T> WithDynamicColor<T>(this IVariableComposer<T> composer, string color, Func<T, string> colorSelector)
        {
            return WithDynamicColor(composer, color, x => Color.Parse(colorSelector(x)));
        }
        /// <summary>
        /// Sets handling of a dynamic color.
        /// </summary>
        /// <typeparam name="T">The type of elements the resulting formatter will support.</typeparam>
        /// <param name="composer">The <see cref="IVariableComposer{T}"/> being extended.</param>
        /// <param name="color">The dynamic color name that this rule should apply to.</param>
        /// <param name="colorSelector">A function that determies the actual color to print when this dynamic color is used.</param>
        /// <returns>A <see cref="IVariableComposer{T}"/> for further configuration.</returns>
        public static IVariableComposer<T> WithDynamicColor<T>(this IVariableComposer<T> composer, string color, Func<T, Color> colorSelector)
        {
            return composer.With(new Variable<T>
            (
                name: composer.Variable.Name,
                selector: composer.Variable.Selector,
                paddedLength: composer.Variable.PaddedLength,
                dynamicColors: composer.Variable.DynamicColors.SetItem(color, colorSelector)
            ));
        }
        /// <summary>
        /// Sets handling of a dynamic color with the name "auto".
        /// </summary>
        /// <typeparam name="T">The type of elements the resulting formatter will support.</typeparam>
        /// <param name="composer">The <see cref="IVariableComposer{T}"/> being extended.</param>
        /// <param name="colorSelector">A function that determies the actual color to print when "auto" is used. Color strings are parsed using <see cref="Color.Parse(string)"/>.</param>
        /// <returns>A <see cref="IVariableComposer{T}"/> for further configuration.</returns>
        public static IVariableComposer<T> WithAutoColor<T>(this IVariableComposer<T> composer, Func<T, string> colorSelector)
        {
            return WithAutoColor(composer, x => Color.Parse(colorSelector(x)));
        }
        /// <summary>
        /// Sets handling of a dynamic color with the name "auto".
        /// </summary>
        /// <typeparam name="T">The type of elements the resulting formatter will support.</typeparam>
        /// <param name="composer">The <see cref="IVariableComposer{T}"/> being extended.</param>
        /// <param name="colorSelector">A function that determies the actual color to print when "auto" is used.</param>
        /// <returns>A <see cref="IVariableComposer{T}"/> for further configuration.</returns>
        public static IVariableComposer<T> WithAutoColor<T>(this IVariableComposer<T> composer, Func<T, Color> colorSelector)
        {
            return composer.WithDynamicColor("auto", colorSelector);
        }
    }
}

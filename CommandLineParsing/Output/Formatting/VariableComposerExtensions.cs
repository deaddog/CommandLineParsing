using System;
using System.Collections.Immutable;

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
                dynamicColors: ImmutableDictionary<string, Func<T, string>>.Empty
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
    }
}

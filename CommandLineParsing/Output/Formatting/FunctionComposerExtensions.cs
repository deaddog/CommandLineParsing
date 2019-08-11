using CommandLineParsing.Output.Formatting.Structure;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CommandLineParsing.Output.Formatting
{
    /// <summary>
    /// Extension methods for <see cref="IFunctionComposer{T}"/> to simplify the fluent config.
    /// </summary>
    public static class FunctionComposerExtensions
    {
        /// <summary>
        /// Adds a function to the composer.
        /// </summary>
        /// <typeparam name="T">The type of elements the resulting formatter will support.</typeparam>
        /// <param name="composer">The <see cref="IFormatterComposer{T}"/> being extended.</param>
        /// <param name="name">The name of the function.</param>
        /// <param name="evaluator">The method used to evaluate the function.</param>
        /// <returns>A <see cref="IFunctionComposer{T}"/> for further configuration.</returns>
        public static IFunctionComposer<T> WithFunction<T>(this IFormatterComposer<T> composer, string name, FunctionEvaluator<T> evaluator)
        {
            return composer.With(new Function<T>(name, evaluator));
        }
        /// <summary>
        /// Adds a function to the composer, ignoring all arguments.
        /// </summary>
        /// <typeparam name="T">The type of elements the resulting formatter will support.</typeparam>
        /// <param name="composer">The <see cref="IFormatterComposer{T}"/> being extended.</param>
        /// <param name="name">The name of the function.</param>
        /// <param name="evaluator">The method used to evaluate the function.</param>
        /// <returns>A <see cref="IFunctionComposer{T}"/> for further configuration.</returns>
        public static IFunctionComposer<T> WithFunction<T>(this IFormatterComposer<T> composer, string name, Func<T, ConsoleString> evaluator)
        {
            return composer.With(new Function<T>(name, (i, args) => evaluator(i)));
        }

        /// <summary>
        /// Adds a function to the composer that will list items in a collection.
        /// Formatting will be applied to each element and separator.
        /// Handling of end-user arguments in a format:
        /// The first argument for the function is the format of each element.
        /// The second argument is the default separator.
        /// The N last arguments are used as separators for the N last elements.
        /// </summary>
        /// <typeparam name="T">The type of elements the resulting formatter will support.</typeparam>
        /// <typeparam name="TItem">The type of the elements in the collection returned by <paramref name="itemsSelector"/>.</typeparam>
        /// <param name="composer">The <see cref="IFunctionComposer{T}"/> being extended.</param>
        /// <param name="name">The name of the function.</param>
        /// <param name="itemsSelector">A function that returns a list of items to be formatted.</param>
        /// <param name="itemComposer">A <see cref="IFunctionComposer{TItem}"/> that is used to format the elements and separators.</param>
        /// <returns>A <see cref="IFunctionComposer{T}"/> for further configuration.</returns>
        public static IFunctionComposer<T> WithListFunction<T, TItem>(this IFormatterComposer<T> composer, string name, Func<T, IEnumerable<TItem>> itemsSelector, IFormatterComposer<TItem> itemComposer)
        {
            return WithListFunction(composer, name, itemsSelector, itemComposer.GetFormatter());
        }
        /// <summary>
        /// Adds a function to the composer that will list items in a collection.
        /// Formatting will be applied to each element and separator.
        /// Handling of end-user arguments in a format:
        /// The first argument for the function is the format of each element.
        /// The second argument is the default separator.
        /// The N last arguments are used as separators for the N last elements.
        /// </summary>
        /// <typeparam name="T">The type of elements the resulting formatter will support.</typeparam>
        /// <typeparam name="TItem">The type of the elements in the collection returned by <paramref name="itemsSelector"/>.</typeparam>
        /// <param name="composer">The <see cref="IFunctionComposer{T}"/> being extended.</param>
        /// <param name="name">The name of the function.</param>
        /// <param name="itemsSelector">A function that returns a list of items to be formatted.</param>
        /// <param name="itemFormatter">A <see cref="IFormatter{TItem}"/> that is used to format the elements and separators.</param>
        /// <returns>A <see cref="IFunctionComposer{T}"/> for further configuration.</returns>
        public static IFunctionComposer<T> WithListFunction<T, TItem>(this IFormatterComposer<T> composer, string name, Func<T, IEnumerable<TItem>> itemsSelector, IFormatter<TItem> itemFormatter)
        {
            return composer.With(new Function<T>
            (
                name: name,
                evaluator: (i, args) =>
                {
                    if (args.Count == 0)
                        return ConsoleString.Empty;

                    if (args.Count == 1) args = args.Add(FormatNoContentElement.Element);

                    return itemsSelector(i)
                        .Reverse()
                        .Select((item, index) =>
                        {
                            var newIndex = Math.Max(args.Count - index, 1);
                            var separator = newIndex == args.Count ? FormatNoContentElement.Element : args[newIndex];

                            return itemFormatter.Format(args[0] + separator, item);
                        })
                        .Aggregate(ConsoleString.Empty, (a, b) => b + a);
                }
            ));
        }
    }
}

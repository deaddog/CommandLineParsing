using System;

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
    }
}

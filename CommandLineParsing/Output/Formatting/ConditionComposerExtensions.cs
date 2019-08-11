using System;

namespace CommandLineParsing.Output.Formatting
{
    /// <summary>
    /// Extension methods for <see cref="IConditionComposer{T}"/> to simplify the fluent config.
    /// </summary>
    public static class ConditionComposerExtensions
    {
        /// <summary>
        /// Adds a condition to the composer.
        /// </summary>
        /// <typeparam name="T">The type of elements the resulting formatter will support.</typeparam>
        /// <param name="composer">The <see cref="IFormatterComposer{T}"/> being extended.</param>
        /// <param name="name">The name of the condition.</param>
        /// <param name="predicate">A predicate function that evaluates if the condition is met.</param>
        /// <returns>A <see cref="IConditionComposer{T}"/> for further configuration.</returns>
        public static IConditionComposer<T> WithPredicate<T>(this IFormatterComposer<T> composer, string name, Predicate<T> predicate)
        {
            return composer.With(new Condition<T>(name, predicate));
        }
    }
}

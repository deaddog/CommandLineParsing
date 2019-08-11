using System;

namespace CommandLineParsing.Output.Formatting
{
    /// <summary>
    /// Defines the configuration of a condition when used in a format.
    /// </summary>
    /// <typeparam name="T">The type of elements the condition uses for evaluation.</typeparam>
    public class Condition<T>
    {
        /// <summary>
        /// Initializes a new condition.
        /// </summary>
        /// <param name="name">The name of the condition, as used in a format.</param>
        /// <param name="predicate">A predicate function that evaluates if the condition is met.</param>
        public Condition(string name, Predicate<T> predicate)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));
        }

        /// <summary>
        /// Gets the name of the condition.
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// Gets the predicate function that evaluates if the condition is met.
        /// </summary>
        public Predicate<T> Predicate { get; }
    }
}

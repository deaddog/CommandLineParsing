using System;

namespace CommandLineParsing.Output.Formatting
{
    /// <summary>
    /// Defines the configuration of a function when used in a format.
    /// </summary>
    /// <typeparam name="T">The type of elements the function can be applied to.</typeparam>
    public class Function<T>
    {
        /// <summary>
        /// Initializes a new function overload.
        /// </summary>
        /// <param name="name">The name of the function, as used in a format.</param>
        /// <param name="evaluator">The method used for evaluation of the function.</param>
        public Function(string name, FunctionEvaluator<T> evaluator)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Evaluator = evaluator ?? throw new ArgumentNullException(nameof(evaluator));
        }

        /// <summary>
        /// Gets the name of the function.
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// Gets the method used for evaluating the function.
        /// </summary>
        public FunctionEvaluator<T> Evaluator { get; }
    }
}

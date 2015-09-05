using System;

namespace CommandLineParsing
{
    /// <summary>
    /// Provides additional methods for the <see cref="Parameter"/> class.
    /// </summary>
    public static class ParameterExtension
    {
        /// <summary>
        /// Sets the parser used by the <see cref="Parameter{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of the values managed by the parameter.</typeparam>
        /// <param name="parameter">The parameter.</param>
        /// <param name="parser">The new parser.</param>
        public static void SetParser<T>(this Parameter<T[]> parameter, TryParse<T> parser)
        {
            (parameter as ArrayParameter<T>)?.setParser(parser);
        }
    }
}

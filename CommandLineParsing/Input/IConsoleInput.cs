using System;

namespace CommandLineParsing.Input
{
    /// <summary>
    /// Provides methods for handling input in console applications.
    /// </summary>
    public interface IConsoleInput
    {
        /// <summary>
        /// Handles the specified key by updating internal/visual state.
        /// </summary>
        /// <param name="key">The key to process.</param>
        void HandleKey(ConsoleKeyInfo key);

        /// <summary>
        /// Gets the location where the input is displayed (upper-left corner).
        /// </summary>
        ConsolePoint Origin { get; }
    }
}

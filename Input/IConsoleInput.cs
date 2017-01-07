using System;

namespace CommandLineParsing.Input
{
    /// <summary>
    /// Provides a method for handling keyboard input in console applications.
    /// </summary>
    public interface IConsoleInput
    {
        /// <summary>
        /// Handles the specified key by updating internal/visual state.
        /// </summary>
        /// <param name="key">The key to process.</param>
        void HandleKey(ConsoleKeyInfo key);
    }
}

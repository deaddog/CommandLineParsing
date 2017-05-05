using System;

namespace CommandLineParsing.Consoles
{
    /// <summary>
    /// Provides an implementation of <see cref="IConsole"/> that forwards method calls to <see cref="Console"/>
    /// </summary>
    public class SystemConsole : IConsole
    {
        private static SystemConsole _singleton = new SystemConsole();

        /// <summary>
        /// Gets the singleton instance of <see cref="SystemConsole"/>, used to interact with <see cref="Console"/>.
        /// </summary>
        public static SystemConsole Instance => _singleton;

        private SystemConsole() { }
    }
}

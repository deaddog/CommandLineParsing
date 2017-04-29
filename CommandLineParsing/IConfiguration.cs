using System.Collections.Generic;

namespace CommandLineParsing
{
    /// <summary>
    /// Provides access to configuration options in the form of key-value pairs.
    /// </summary>
    public interface IConfiguration
    {
        /// <summary>
        /// Gets the <see cref="string"/> value associated with a key.
        /// </summary>
        /// <value>
        /// The <see cref="System.String"/>.
        /// </value>
        /// <param name="key">The key (name.name) that the value is associated with.</param>
        /// <returns>The <see cref="string"/> value that corresponds to <paramref name="key"/>.</returns>
        string this[string key] { get; }
        /// <summary>
        /// Determines whether the specified key exists in the configuration.
        /// </summary>
        /// <param name="key">The key that is looked for in the configuration.</param>
        /// <returns><c>true</c>, if <paramref name="key"/> is defined in the configuration; otherwise, <c>false</c>.</returns>
        bool HasKey(string key);
        /// <summary>
        /// Gets all the key/value pairs in the configuration.
        /// </summary>
        /// <returns>A collection of all key/value pairs in the configuration.</returns>
        IEnumerable<KeyValuePair<string, string>> GetAll();
    }
}

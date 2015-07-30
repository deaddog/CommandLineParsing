using System;
using System.Collections.Generic;

namespace CommandLineParsing
{
    /// <summary>
    /// Provides acces to multiple <see cref="IConfiguration"/> configurations through one element.
    /// </summary>
    public class ConfigurationGroup : IConfiguration
    {
        private LinkedList<IConfiguration> configurations;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationGroup"/> class.
        /// </summary>
        public ConfigurationGroup()
        {
            this.configurations = new LinkedList<IConfiguration>();
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationGroup"/> class.
        /// </summary>
        /// <param name="configurations">The configurations that are copied to the new <see cref="ConfigurationGroup"/>.
        /// Values in the first element will override any in the latter ones when they have equal keys.</param>
        public ConfigurationGroup(params IConfiguration[] configurations)
        {
            if (configurations == null)
                throw new ArgumentNullException(nameof(configurations));

            this.configurations = new LinkedList<IConfiguration>(configurations);
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationGroup"/> class.
        /// </summary>
        /// <param name="configurations">The configurations that are copied to the new <see cref="ConfigurationGroup"/>.
        /// Values in the first element will override any in the latter ones when they have equal keys.</param>
        public ConfigurationGroup(IEnumerable<IConfiguration> configurations)
        {
            if (configurations == null)
                throw new ArgumentNullException(nameof(configurations));

            this.configurations = new LinkedList<IConfiguration>(configurations);
        }

        /// <summary>
        /// Gets the <see cref="string"/> value with the specified key.
        /// </summary>
        /// <param name="key">The key (name.name) that the value is associated with.</param>
        /// <returns>The <see cref="string"/> value that corresponds to <paramref name="key"/> from the first configuration where <paramref name="key"/> is defined.</returns>
        public string this[string key]
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Determines whether the specified key exists in the configuration.
        /// </summary>
        /// <param name="key">The key that is looked for in the configuration.</param>
        /// <returns><c>true</c>, if <paramref name="key"/> is defined any of the configurations in this <see cref="ConfigurationGroup"/>; otherwise, <c>false</c>.</returns>
        public bool HasKey(string key)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets all the key/value pairs in the configuration.
        /// If a key is defined in multiple configurations, only the first one found is returned.
        /// </summary>
        /// <returns>
        /// A collection of all key/value pairs in the configuration.
        /// </returns>
        public IEnumerable<KeyValuePair<string, string>> GetAll()
        {
            throw new NotImplementedException();
        }
    }
}
